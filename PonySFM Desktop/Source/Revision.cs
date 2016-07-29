using System;
using System.Collections.Generic;
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

        public Revision(int id, List<RevisionFileEntry> files)
        {
            ID = id;
            Files = files;
        }

        public static Revision CreateFromXML(XmlElement elem)
        {
            List<RevisionFileEntry> files = new List<RevisionFileEntry>();
            int id = Convert.ToInt32(elem.GetAttribute("ID"));

            foreach(XmlElement file in elem.ChildNodes)
            {
                files.Add(new RevisionFileEntry(elem.GetAttribute("Location"), elem.GetAttribute("SHA512")));
            }

            return new Revision(id, files);
        }

        public XmlElement ToXML(XmlDocument doc, IFileSystem fs)
        {
            var elem = doc.CreateElement("Revision");
            elem.SetAttribute("ID", ID.ToString());
            foreach (var file in Files)
            {
                var fileElem = doc.CreateElement("File");
                fileElem.SetAttribute("SHA512", file.Sha512);
                fileElem.SetAttribute("Location", file.Path);
                elem.AppendChild(fileElem);
            }

            return elem;
        }
    }
}