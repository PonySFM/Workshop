using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CoreLib.Interface;

namespace CoreLib
{
    public class ConfigHandler
    {
        private readonly ConfigParser _configParser;
        private readonly IFileSystem _fs;
        private readonly string _filepath;

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
            return _configParser.Read(_fs.OpenXml(_filepath));
        }

        public void Write(ConfigFile file)
        {
            var doc = new XmlDocument();
            _configParser.Write(file, doc);
            _fs.SaveXml(doc, _filepath);
        }
    }
}
