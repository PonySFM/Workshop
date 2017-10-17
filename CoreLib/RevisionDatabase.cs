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
                WriteDbDisk();
            else
                RefreshDataDisk();
        }

        public void AddToDb(Revision revision)
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
            var doc = _fs.OpenXml(Filepath);
            RefreshData(doc);
        }

        public XmlDocument WriteDb()
        {
            var doc = new XmlDocument();
            var root = doc.CreateElement("PonySFM");

            foreach (var revision in Revisions)
                root.AppendChild(revision.ToXml(doc, _fs));

            doc.AppendChild(root);
            return doc;
        }

        public void WriteDbDisk()
        {
            var doc = WriteDb();
            _fs.SaveXml(doc, Filepath);
        }
    }
}
