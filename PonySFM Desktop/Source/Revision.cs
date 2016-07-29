using System;
using System.Collections.Generic;
using System.Xml;

namespace PonySFM_Desktop
{
    public class Revision
    {
        public int ID { get; set; }
        public List<string> Files { get; set; }

        public Revision(int id, List<string> files)
        {
            ID = id;
            Files = files;
        }

        public static Revision CreateFromXML(XmlElement elem)
        {
            List<string> files = new List<string>();
            int id = Convert.ToInt32(elem.GetAttribute("ID"));

            foreach(XmlElement file in elem.ChildNodes)
            {
                files.Add(file.GetAttribute("Location"));
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
                fileElem.SetAttribute("SHA512", fs.GetChecksum(file));
                fileElem.SetAttribute("Location", file);
                elem.AppendChild(fileElem);
            }

            return elem;
        }
    }
}