using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace PonySFM_Desktop
{
    public static class SFM
    {
        public static string SFMDirectory;
        public static string SFMDConfigLocation;
        public static string SFMUsermodDirectory;
        public static string InstallationDirectory;
        static string ConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PonySFM";
        static string ConfigFileLocation = ConfigLocation + "\\config.xml";

        public static string ShowDirectoryDialog(string currentDir = null)
        {
            var dialog = new WinForms.FolderBrowserDialog();
            if (!string.IsNullOrEmpty(currentDir))
                dialog.SelectedPath = currentDir;
            if (dialog.ShowDialog() == WinForms.DialogResult.OK)
                return dialog.SelectedPath;
            else
                return "";
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
