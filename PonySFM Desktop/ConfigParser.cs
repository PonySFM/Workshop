using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PonySFM_Desktop
{
    public class ConfigParser
    {
        string _filepath;
        IFileSystem _fs;

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

        public ConfigParser(string filepath, IFileSystem fs)
        {
            _filepath = filepath;
            _fs = fs;
        }

        public bool Exists()
        {
            return _fs.FileExists(Filepath);
        }

        public void Write(ConfigFile file, XmlDocument doc)
        {
            var elem = file.ToXML(doc);
            doc.AppendChild(elem);
        }

        public ConfigFile Read(XmlDocument doc)
        {
            if(doc.FirstChild != null)
            {
                return ConfigFile.FromXML((XmlElement)doc.FirstChild);
            }
            else
            {
                return null;
            }
        }
    }
}
