using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PonySFM_Workshop.Source
{
    public class ConfigHandler
    {
        private ConfigParser _configParser;
        private IFileSystem _fs;
        private string _filepath;

        public ConfigHandler(string filepath, IFileSystem fs)
        {
            _configParser = new ConfigParser(fs);
            _fs = fs;
            _filepath = filepath;
        }

        public bool Exists()
        {
            return _fs.FileExists(_filepath);
        }

        public ConfigFile Read()
        {
            return _configParser.Read(_fs.OpenXML(_filepath));
        }

        public void Write(ConfigFile file)
        {
            var doc = new XmlDocument();
            _configParser.Write(file, doc);
            _fs.SaveXML(doc, _filepath);
        }
    }
}
