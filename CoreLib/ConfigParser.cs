using System.Xml;
using CoreLib;
using CoreLib.Interface;

namespace CoreLib
{
    public class ConfigParser
    {
        private readonly IFileSystem _fs;

        public ConfigParser(IFileSystem fs)
        {
            _fs = fs;
        }

        public void Write(ConfigFile file, XmlDocument doc)
        {
            var elem = file.ToXml(doc);
            doc.AppendChild(elem);
        }

        public ConfigFile Read(XmlDocument doc)
        {
            return doc.FirstChild != null ? ConfigFile.FromXml((XmlElement)doc.FirstChild) : null;
        }
    }
}
