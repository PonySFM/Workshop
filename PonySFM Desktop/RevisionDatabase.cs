using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

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

            if(!fs.FileExists(filepath))
                WriteDBDisk();
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
