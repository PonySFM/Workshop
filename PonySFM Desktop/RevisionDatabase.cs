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

        public RevisionDatabase(string filepath)
        {
            _filepath = filepath;

            if(!File.Exists(_filepath))
                WriteDB();
        }

        public void RefreshData()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(_filepath);

            foreach(XmlElement elem in doc.FirstChild.ChildNodes)
            {
                _revisions.Add(Revision.CreateFromXML(elem));
            }
        }

        public void WriteDB()
        {
            XmlDocument doc = new XmlDocument();
            var root = doc.CreateElement("PonySFM");

            foreach (var revision in _revisions)
                root.AppendChild(revision.ToXML(doc));

            doc.AppendChild(root);
            doc.Save(_filepath);
        }
    }
}
