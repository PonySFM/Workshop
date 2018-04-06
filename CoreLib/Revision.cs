using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CoreLib.Interface;

namespace CoreLib
{
    public class Revision
    {
        public int ID { get; private set; }
        public List<RevisionFileEntry> Files { get; private set; }
        public Dictionary<string, string> Metadata { get; private set; }

        public Revision(int id, List<RevisionFileEntry> files)
        {
            ID = id;
            Files = files;
            Metadata = new Dictionary<string, string>();
        }

        public static Revision CreateFromXml(XmlElement elem)
        {
            var id = Convert.ToInt32(elem.GetAttribute("ID"));
            var metaData = new Dictionary<string, string>();

            foreach (XmlAttribute attr in elem.Attributes)
            {
                if (attr.Name == "ID")
                    id = Convert.ToInt32(attr.Value);
                else
                    metaData[attr.Name] = attr.Value;
            }

            var files = (from XmlElement file in elem.ChildNodes select new RevisionFileEntry(file.GetAttribute("Location"), file.GetAttribute("SHA512"))).ToList();

            return new Revision(id, files) {Metadata = metaData};
        }

        public XmlElement ToXml(XmlDocument doc, IFileSystem fs)
        {
            var elem = doc.CreateElement("Revision");
            elem.SetAttribute("ID", ID.ToString());

            foreach (var data in Metadata)
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
                if (!file.Path.StartsWith(currentDir)) continue;

                var newPath = file.Path.Replace(currentDir, "");
                newPath = Path.Combine(topDir, newPath);
                file.Path = newPath;
            }
        }

        public string GetMetadataValue(string key)
        {
            return Metadata.ContainsKey(key) ? Metadata[key] : "N/A";
        }

        /// <summary>
        /// Check if we're missing some metadata key
        /// </summary>
        /// <returns></returns>
        public bool MissingMetadata()
        {
            var keys = new List<string> { "UserName", "ResourceName", "Size" };
            return keys.Any(key => !Metadata.ContainsKey(key));
        }

        public long CalculateSizeOnDisk(IFileSystem fs)
        {
            return Files.Sum(file => fs.GetFileSize(file.Path));
        }

        public static Revision CreateTemporaryRevisionFromFolder(int id, string dir, IFileSystem fs)
        {
            var fileEntries = new List<RevisionFileEntry>();

            GetFileEntriesFromDirectory(fileEntries, dir, fs);
            return new Revision(id, fileEntries);
        }

        private static void GetFileEntriesFromDirectory(ICollection<RevisionFileEntry> list, string dir, IFileSystem fs)
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