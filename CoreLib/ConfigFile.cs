using System.Xml;

namespace CoreLib
{
    /* TODO: instead of just accepting one string as param this class should hold a list of attributes with a dictionary */
    // There is nothing wrong with holding a path with a string type.
    public class ConfigFile
    {
        public string SfmDirectoryPath { get; set; }

        public ConfigFile(string sfmDirPath)
        {
            SfmDirectoryPath = sfmDirPath;
        }

        public static ConfigFile FromXml(XmlElement elem)
        {
            return new ConfigFile(elem.InnerText);
        }

        public XmlElement ToXml(XmlDocument doc)
        {
            var elem = doc.CreateElement("SFMDirectoryPath");
            elem.InnerText = SfmDirectoryPath;
            return elem;
        }

        public bool Equals(ConfigFile other)
        {
            return other.SfmDirectoryPath == SfmDirectoryPath;
        }
    }
}
