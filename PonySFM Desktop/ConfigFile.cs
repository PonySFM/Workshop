using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PonySFM_Desktop
{
    public class ConfigFile
    {
        public string SFMDirectoryPath { get; set; }

        public static ConfigFile FromXML(XmlElement elem)
        {
            ConfigFile config = new ConfigFile();
            config.SFMDirectoryPath = elem.GetAttribute("SFMDirectoryPath");
            return config;
        }

        public XmlElement ToXML(XmlDocument doc)
        {
            XmlElement elem = doc.CreateElement("Config");
            elem.SetAttribute("SFMDirectoryPath", SFMDirectoryPath);
            return elem;
        }

        public bool Equals(ConfigFile other)
        {
            return other.SFMDirectoryPath == this.SFMDirectoryPath;
        }
    }
}
