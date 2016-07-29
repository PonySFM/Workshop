using System.Collections.Generic;
using System.Xml;

namespace PonySFM_Desktop
{
    public class RevisionDatabase
    {
        private List<Revision> _revisions = new List<Revision>();
        private string _filepath;
        private IFileSystem _fs;

        public List<Revision> Revisions
        {
            get
            {
                return _revisions;
            }
        }

        public string Filepath
        {
            get
            {
                return _filepath;
            }
            set
            {
                _filepath = value;
            }
        }

        public RevisionDatabase(string filepath, IFileSystem fs)
        {
            _filepath = filepath;
            _fs = fs;

            if (!fs.FileExists(filepath))
                WriteDBDisk();
            else
                RefreshDataDisk();
        }

        public void AddToDB(Revision revision)
        {
            _revisions.Add(revision);
        }

        internal void RemoveRevision(int id)
        {
            var revision = _revisions.Find(r => r.ID == id);
            if (revision != null)
                _revisions.Remove(revision);
        }

        public void RefreshData(XmlDocument doc)
        {
            foreach(XmlElement elem in doc.FirstChild.ChildNodes)
            {
                _revisions.Add(Revision.CreateFromXML(elem));
            }
        }

        public void RefreshDataDisk()
        {
            var doc = _fs.OpenXML(_filepath);
            RefreshData(doc);
        }

        public XmlDocument WriteDB()
        {
            XmlDocument doc = new XmlDocument();
            var root = doc.CreateElement("PonySFM");

            foreach (var revision in _revisions)
                root.AppendChild(revision.ToXML(doc, _fs));

            doc.AppendChild(root);
            return doc;
        }

        public void WriteDBDisk()
        {
            var doc = WriteDB();
            _fs.SaveXML(doc, _filepath);
        }
    }
}
