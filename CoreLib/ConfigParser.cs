using System.Xml;
using CoreLib;
using CoreLib.Interface;

namespace CoreLib
{
    public class ConfigParser
    {
        IFileSystem _fs;

        public ConfigParser(IFileSystem fs)
        {
            _fs = fs;
        }

        public void Write(ConfigFile file, XmlDocument doc)
        {
            var elem = file.ToXML(doc);
            doc.AppendChild(elem);
        }

        public ConfigFile Read(XmlDocument doc)
        {
            if (doc.FirstChild != null)
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
