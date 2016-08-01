using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PonySFM_Desktop
{
    public class RevisionFileEntry
    {
        string _path;
        string _sha512;

        public string Path
        {
            get
            {
                return _path;
            }

            set
            {
                _path = value;
            }
        }

        public string Sha512
        {
            get
            {
                return _sha512;
            }

            set
            {
                _sha512 = value;
            }
        }

        public RevisionFileEntry(string path, string sha256)
        {
            Path = path;
            Sha512 = sha256;
        }

        public static RevisionFileEntry FromFile(string path, IFileSystem fs)
        {
            var checksum = fs.GetChecksum(path);
            return new RevisionFileEntry(path, checksum);
        }
    }

    public class Revision
    {
        public int ID { get; set; }
        public List<RevisionFileEntry> Files { get; set; }
        public Dictionary<string, string> AdditionalData { get; set; }

        public Revision(int id, List<RevisionFileEntry> files)
        {
            ID = id;
            Files = files;
            AdditionalData = new Dictionary<string, string>();
        }

        public static Revision CreateFromXML(XmlElement elem)
        {
            List<RevisionFileEntry> files = new List<RevisionFileEntry>();
            int id = Convert.ToInt32(elem.GetAttribute("ID"));
            Dictionary<string, string> additionalData = new Dictionary<string, string>();

            foreach (XmlAttribute attr in elem.Attributes)
            {
                if (attr.Name == "ID")
                    id = Convert.ToInt32(attr.Value);
                else
                    additionalData[attr.Name] = attr.Value;
            }

            foreach(XmlElement file in elem.ChildNodes)
            {
                files.Add(new RevisionFileEntry(file.GetAttribute("Location"), file.GetAttribute("SHA512")));
            }

            var rev = new Revision(id, files);
            rev.AdditionalData = additionalData;

            return rev;
        }

        public XmlElement ToXML(XmlDocument doc, IFileSystem fs)
        {
            var elem = doc.CreateElement("Revision");
            elem.SetAttribute("ID", ID.ToString());

            foreach (var data in AdditionalData)
            {
                elem.SetAttribute(data.Key, data.Value);
            }

            foreach (var file in Files)
            {
                var fileElem = doc.CreateElement("File");
                fileElem.SetAttribute("SHA512", file.Sha512);
                fileElem.SetAttribute("Location", file.Path);
                elem.AppendChild(fileElem);
            }

            return elem;
        }

        /*
         * This method should be refactored as soon as possible. Probably when we start connecting the WPF-side.
         * Because we have to effectively "move" the revision after it's extracted to a temporary directory. All
         * the files in file-list have to as well. We'll have to think about what immediate representation the
         * temporary revision should have. I would suggest making some class ITemporaryRevision and letting it derive
         * from this one somehow.
         */ 
        public void ChangeTopDirectory(string currentDir, string topDir)
        {
            if (!currentDir.EndsWith("\\"))
                currentDir = currentDir + "\\";

            foreach (var file in Files)
            {
                if (file.Path.StartsWith(currentDir))
                {
                    var newPath = file.Path.Replace(currentDir, "");
                    newPath = Path.Combine(topDir, newPath);
                    file.Path = newPath;
                }
            }
        }

        public static Revision CreateTemporaryRevisionFromFolder(int id, string dir, IFileSystem fs)
        {
            List<RevisionFileEntry> fileEntries = new List<RevisionFileEntry>();

            GetFileEntriesFromDirectory(fileEntries, dir, fs);
            return new Revision(id, fileEntries);
        }

        private static void GetFileEntriesFromDirectory(List<RevisionFileEntry> list, string dir, IFileSystem fs)
        {
            var files = fs.GetFiles(dir);
            var dirs = fs.GetDirectories(dir);

            foreach (var file in files)
            {
                list.Add(RevisionFileEntry.FromFile(file.Path, fs));
            }

            foreach (var d in dirs)
            {
                GetFileEntriesFromDirectory(list, d.Path, fs);
            }
        }
    }
}