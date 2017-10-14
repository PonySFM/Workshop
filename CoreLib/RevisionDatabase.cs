using System.Collections.Generic;
using System.Xml;
using CoreLib.Interface;

namespace CoreLib
{
    public class RevisionDatabase
    {
        private readonly IFileSystem _fs;

        public List<Revision> Revisions { get; } = new List<Revision>();
        public string Filepath { get; set; }

        public RevisionDatabase(string filepath, IFileSystem fs)
        {
            Filepath = filepath;
            _fs = fs;

            if (!fs.FileExists(filepath))
                WriteDBDisk();
            else
                RefreshDataDisk();
        }

        public void AddToDB(Revision revision)
        {
            Revisions.Add(revision);
        }

        internal void RemoveRevision(int id)
        {
            var revision = Revisions.Find(r => r.ID == id);
            if (revision != null)
                Revisions.Remove(revision);
        }

        public void RefreshData(XmlDocument doc)
        {
            Revisions.Clear();
            foreach(XmlElement elem in doc.FirstChild.ChildNodes)
            {
                Revisions.Add(Revision.CreateFromXml(elem));
            }
        }

        public void RefreshDataDisk()
        {
            var doc = _fs.OpenXML(Filepath);
            RefreshData(doc);
        }

        public XmlDocument WriteDB()
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("PonySFM");

            foreach (var revision in Revisions)
                root.AppendChild(revision.ToXml(doc, _fs));

            doc.AppendChild(root);
            return doc;
        }

        public void WriteDBDisk()
        {
            var doc = WriteDB();
            _fs.SaveXML(doc, Filepath);
        }
    }
}
