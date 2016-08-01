using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Windows;
using Ionic.Zip;
using WinForms = System.Windows.Forms;
using System.Net;

namespace PonySFM_Workshop
{
    public static class SFM
    {
        public static string SFMDirectory;
        public static string SFMConfigLocation;
        public static string SFMUsermodDirectory;
        public static string InstallationDirectory;
        static string ConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PonySFM";
        static string ConfigFileLocation = ConfigLocation + "\\config.xml";
        static bool copyAllFiles = false;

        public static string ShowDirectoryDialog(string currentDir = null)
        {
            var dialog = new WinForms.FolderBrowserDialog();
            if (!string.IsNullOrEmpty(currentDir))
                dialog.SelectedPath = currentDir;
            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
                return dialog.SelectedPath;
            else
                return string.Empty;
        }

        static void RelocateSFMDirectory()
        {
            var path = ShowDirectoryDialog();
            if(!Directory.Exists(path))
            {
                MessageBox.Show("The directory does not exist.", "PonySFM", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Directory.CreateDirectory(ConfigLocation);
            var doc = new XmlDocument();
            var sfmDirElem = doc.CreateElement("SFMDirectoryPath");
            sfmDirElem.InnerText = path;

            doc.AppendChild(sfmDirElem);
            doc.Save(ConfigFileLocation);

            SetDirectory(path);
        }

        public static bool LikelyToBeSFMDir(string dir)
        {
            string[] typicalSubDirs = { "bin", "platform", "tf" };
            foreach(var d in typicalSubDirs)
            {
                if (!Directory.Exists(Path.Combine(dir, "game", d)))
                    return false;
            }

            return true;
        } 

        public static void SetDirectory(string dir)
        {
            SFMDirectory = dir;
            InstallationDirectory = Path.Combine(dir, "game", "ponysfm");
            SFMUsermodDirectory = Path.Combine(dir, "game", "usermod");
            if (!Directory.Exists(InstallationDirectory))
            {
                Directory.CreateDirectory(InstallationDirectory);
            }

            AddGameinfoEntry();
        }

        public static bool CheckConfigExists()
        {
            return Directory.Exists(ConfigLocation) && File.Exists(ConfigFileLocation);
        }

        static void InitializeConfig()
        {
            if (!CheckConfigExists())
            {
                RelocateSFMDirectory();
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigFileLocation);

                foreach (var node in doc.ChildNodes)
                {
                    XmlElement elem = (XmlElement)node;

                    if(elem.Name == "SFMDirectoryPath")
                    {
                        SetDirectory(elem.InnerText);
                    }
                }

                if(!Directory.Exists(SFMDirectory))
                {
                    MessageBox.Show("SFM Directory not found!");
                    return;
                }
            }
        }

        

        static void AddInstalledRevision(int id, List<string> files)
        {
            XmlDocument doc = new XmlDocument();

            if (File.Exists(SFMConfigLocation))
            {
                doc.Load(SFMConfigLocation);
            }

            XmlElement revisionElem = null;
            bool exists = false;

            // First check if it already exists, if yes, use that one
            foreach(var node in doc.FirstChild.ChildNodes)
            {
                XmlElement elem = (XmlElement)node;    
                if(elem.GetAttribute("ID") == id.ToString())
                {
                    revisionElem = elem;
                    exists = true;
                    break;
                }
            }

            if (revisionElem == null)
            {
                revisionElem = doc.CreateElement("Revision");
                revisionElem.SetAttribute("ID", id.ToString());
            }

            foreach(var file in files)
            {
                XmlElement fileElem = doc.CreateElement("File");
                fileElem.SetAttribute("Location", file);
                //fileElem.SetAttribute("SHA512", FileUtil.GetChecksum(file));

                revisionElem.AppendChild(fileElem);
            }

            if(!exists)
            {
                doc.DocumentElement.AppendChild(revisionElem);
            }

            doc.Save(SFMConfigLocation);
        }

        static void UninstallRevision(int id)
        {
            XmlDocument doc = new XmlDocument();

            if (File.Exists(SFMConfigLocation))
            {
                doc.Load(SFMConfigLocation);
            }

            XmlElement revisionElem = null;

            // First check if it already exists, if yes, use that one
            foreach(var node in doc.FirstChild.ChildNodes)
            {
                XmlElement elem = (XmlElement)node;    
                if(elem.GetAttribute("ID") == id.ToString())
                {
                    revisionElem = elem;
                    break;
                }
            }

            if(revisionElem == null)
            {
                Console.WriteLine("Not installed");
                return;
            }

            Logger.Log("-- Uninstalling revision " + id + " --");

            Console.WriteLine("Deleting revision " + revisionElem.GetAttribute("ID"));

            foreach (var fileNode in revisionElem.ChildNodes)
            {
                var fileElem = (XmlElement)fileNode;
                string location = fileElem.GetAttribute("Location");

                Logger.Log("Deleting " + location + "\n");
                Console.Write("Deleting " + location + "... ");
                File.Delete(location);
                Console.WriteLine("OK");
            }

            doc.FirstChild.RemoveChild(revisionElem);
            doc.Save(SFMConfigLocation);
        }

        static XmlElement GetInstalledRevision(int id)
        {
            XmlDocument doc = new XmlDocument();

            if (!File.Exists(SFMConfigLocation))
            {
                return null;
            }

            doc.Load(SFMConfigLocation);

            XmlElement revisionElem = null;

            // First check if it already exists, if yes, use that one
            foreach(var node in doc.FirstChild.ChildNodes)
            {
                XmlElement elem = (XmlElement)node;    
                if(elem.GetAttribute("ID") == id.ToString())
                {
                    revisionElem = elem;
                    break;
                }
            }

            return revisionElem;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, int revisionId)
        {
            Logger.Log("Copying "+ sourceDirName + " to "+ destDirName +"\n");
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            List<string> copiedFiles = new List<string>();

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                bool copy = true;
                if(File.Exists(temppath) && !copyAllFiles)
                {
                    Console.Write("File {0} already exists, overwrite? [y/n/a] ", temppath);
                    string answer = Console.ReadLine();

                    if (string.IsNullOrEmpty(answer))
                    {
                        copy = true;
                    }
                    else
                    {
                        switch (answer[0])
                        {
                            case '\n':
                            case 'y':
                            case 'Y':
                                copy = true;
                                break;

                            case 'n':
                            case 'N':
                                copy = false;
                                break;

                            case 'a':
                            case 'A':
                                copy = true;
                                copyAllFiles = true;
                                break;

                            default:
                                copy = false;
                                break;
                        }
                    }
                }

                if(copyAllFiles || copy)
                {
                    file.CopyTo(temppath, true);
                    copiedFiles.Add(temppath);
                }
            }

            AddInstalledRevision(revisionId, copiedFiles);

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, revisionId);
                }
            }

            Logger.Log("Done for: " + sourceDirName + "\n");
        }

        public static void DownloadAndInstall(int id)
        {
            if (GetInstalledRevision(id) != null)
            {
                bool uninstall = false;
                Console.Write("Already installed! Uninstall? [y/n] ");
                switch(Console.ReadLine().ToLower())
                {
                    case "y":
                    case "yes":
                        uninstall = true;
                        break;
                }

                if (uninstall)
                {
                    UninstallRevision(id);
                    Console.WriteLine("Done!");
                    Console.ReadKey();
                }
                return;
            }

            Logger.Log(" -- Installing " + id + "-- \n");

            string tmpFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var webClient = new CookedWebClient();
            webClient.Headers.Add("user-agent", "PSFM_ModManager-"+ModManager.Version);

            Console.WriteLine("Downloading...");

            Logger.Log("Downloading to tmpFile "+tmpFile+"\n");

            try
            {
                webClient.DownloadFile(string.Format("{0}/rev/{1}/internal_download_redirect", ModManager.PonySFMURL, id), tmpFile);
            }
            catch (WebException e)
            {
                Console.WriteLine("Failed to download: "+e.Message);
                Logger.Log("Failed to download " + e.Message +" \n");
                return;
            }

            Logger.Log("Finished downloading \n");

            InstallRevisionFile(tmpFile, id);

            Logger.Log("Removing tmpFile " + tmpFile);
            File.Delete(tmpFile);
        }

        static bool IsModFileDirectory(string dir)
        {
            string onlyName = Path.GetFileName(dir).ToLower();
            return onlyName == "materials" || onlyName == "models" || onlyName == "maps" || onlyName == "sounds" || onlyName == "particles" || onlyName == "scripts";
        }

        static void VerifyRevisionFiles()
        {
            Logger.Log("-- Starting verification --\n");

            XmlDocument doc = new XmlDocument();

            doc.Load(SFMConfigLocation);

            int tested = 0;
            int failures = 0;
            List<int> failedRevisions = new List<int>();

            // First check if it already exists, if yes, use that one
            foreach (var node in doc.FirstChild.ChildNodes)
            {
                XmlElement elem = (XmlElement)node;
                int currentID = Convert.ToInt32(elem.GetAttribute("ID"));
                Console.WriteLine("Testing revision " + currentID);
                Logger.Log("-- Revision " + currentID + " --\n");
                foreach (var node2 in elem.ChildNodes)
                {
                    XmlElement elem2 = (XmlElement)node2;

                    string sha512 = elem2.GetAttribute("SHA512");
                    string location = elem2.GetAttribute("Location");

                    Console.Write("Testing " + location + "... ");
                    Logger.Log("Testing file " + location + "\n");
                    string checksum = null;
                    /*
                    if(File.Exists(location) && (checksum = FileUtil.GetChecksum(location)) == sha512)
                    {
                        Console.WriteLine("OK");
                        Logger.Log("OK\n");
                    }
                    else
                    {
                        Logger.Log("NOPE\n");
                        Console.WriteLine("FAIL");
                        if(!failedRevisions.Contains(currentID))
                        {
                            failedRevisions.Add(currentID);
                        }
                        failures++;
                    }
                    */

                    Logger.Log("Computed hash: " + checksum + "\n");
                    Logger.Log("Expected hash: " + sha512 + "\n");

                    tested++;
                }
            }

            Console.WriteLine("Summary");
            Console.WriteLine("--------------------------------------------------------------------------------");

            Console.WriteLine("Tested: " + tested + " files");
            Console.WriteLine("Failures: " + failures + " files");

            Logger.Log("Tested: "+tested+"\n");
            Logger.Log("Failed: "+failures+"\n");

            if (failedRevisions.Count > 0)
            {
                Console.Write("Reinstall all corrupt revisions? [y/n] ");
                string input = Console.ReadLine();

                bool reinstall = false;

                switch (input)
                {
                    case "Y":
                    case "y":
                        reinstall = true;
                        break;
                }

                if(!reinstall)
                {
                    return;
                }

                foreach(var failed in failedRevisions)
                {
                    UninstallRevision(failed);
                    //DownloadAndInstall(failed);
                }
            }
        }

        static void UninstallAllRevisions()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(SFMConfigLocation);

            foreach (var node in doc.FirstChild.ChildNodes)
            {
                XmlElement elem = (XmlElement)node;
                int currentID = Convert.ToInt32(elem.GetAttribute("ID"));

                UninstallRevision(currentID);
            }
        }

        static void InstallRevisionFile(string filepath, int revisionId)
        {
            Console.WriteLine("Installing archive {0}", filepath);

            string tmp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tmp);

            Console.WriteLine("Extracting...");

            Logger.Log("Extracting archive\n");

            try
            {
                using (ZipFile zip1 = ZipFile.Read(filepath))
                {
                    foreach (ZipEntry e in zip1)
                    {
                        Logger.Log("Extracting: "+e.FileName+"\n");
                        e.Extract(tmp, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to extract archive");
                Console.WriteLine(e.Message);
                Logger.Log(e.Message);
                Directory.Delete(tmp, true);
                Console.ReadKey();
                return;
            }

            string[] files = Directory.GetFiles(tmp, "*.*", SearchOption.AllDirectories);

            // Check top directory
            string topDir = tmp;
            string[] topDirFiles = Directory.GetDirectories(tmp, "*.*", SearchOption.AllDirectories);

            bool foundDir = false;

            foreach(var dir in topDirFiles)
            {
                Logger.Log("Checking if "+dir+" is a mod directory\n");
                if(IsModFileDirectory(dir))
                {
                    topDir = Path.Combine(tmp, dir, "..");
                    foundDir = true;
                    break;
                }
            } 

            if (!foundDir)
            {
                Logger.Log("Couldn't find any mod directory!\n");
                Console.WriteLine("Can't parse archive!");
                Console.ReadKey();
                Directory.Delete(tmp, true);
                return;
            }

            // Remove the top directory from all files
            for(int i = 0; i < files.Length; i++)
            {
                int index = files[i].IndexOf(topDir);
                if(index >= 0)
                {
                    files[i] = files[i].Remove(index, topDir.Length);
                }
            }

            Console.WriteLine("Copying...");
            DirectoryCopy(topDir, InstallationDirectory, true, revisionId);

            copyAllFiles = false;

            Console.WriteLine("Done! Cleaning up...");
            Directory.Delete(tmp, true);
        }

        static void AddGameinfoEntry()
        {
            string lineToAdd = "\t\t\tGame\t\t\t\tponysfm";
            string fileName = Path.Combine(SFMUsermodDirectory, "gameinfo.txt");
            List<string> txtLines = new List<string>();

            //Fill a List<string> with the lines from the txt file.
            foreach (string str in File.ReadAllLines(fileName))
            {
                txtLines.Add(str);
            }

            if(txtLines.Any(s => s.Contains("ponysfm") && s.Contains("Game")))
            {
                return;
            }

            bool lineInserted = false;

            foreach(string str in txtLines)
            {
                if(str.Contains("SearchPaths"))
                {
                    txtLines.Insert(txtLines.IndexOf(str) + 2, lineToAdd);
                    lineInserted = true;
                    break;
                }
            }

            if(!lineInserted)
            {
                /*
                Console.WriteLine("Failed to add custom gameinfo.txt entry!");
                Console.WriteLine("How to do it manually:");
                Console.WriteLine("1. Open the gameinfo.txt in the usermod directory");
                Console.WriteLine("2. Search for the line that says 'SearchPaths'");
                Console.WriteLine("3. Add an entry with the foldername 'ponysfm'");
                */
                return;
            }

            //Clear the file. The using block will close the connection immediately.
            using (File.Create(fileName)) { }

            //Add the lines including the new one.
            foreach (string str in txtLines)
            {
                File.AppendAllText(fileName, str + Environment.NewLine);
            }
        }
    }
}
