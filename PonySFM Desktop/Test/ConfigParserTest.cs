using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;
using System.Xml;

namespace PonySFM_Desktop.Test
{
    [TestFixture]
    public class ConfigParserTest
    {
        string configLocation = "C:\\ponysfm\\config.xml";

        [Test]
        public void ChecksForExistenceCorrectly()
        {
            var fs = new MockFileSystem();
            var configParser = new ConfigParser(configLocation, fs);

            Assert.False(configParser.Exists());
            fs.AddFile(new MockFile("C:\\ponysfm\\config.xml", MockFileType.FILE));
            Assert.True(configParser.Exists());
        }

        [Test]
        public void ParsesCorrectly()
        {
            var doc = new XmlDocument();
            var configFile = new ConfigFile();
            var configParser = new ConfigParser(configLocation, new MockFileSystem());
            configFile.SFMDirectoryPath = "C:\\SFM";
            doc.AppendChild(configFile.ToXML(doc));

            var parsedConfig = configParser.Read(doc);

            Assert.That(parsedConfig.Equals(configFile));
            Assert.That(doc.ChildNodes.Count == 1);
        }

        [Test]
        public void WritesCorrectly()
        {
            var configFile = new ConfigFile();
            configFile.SFMDirectoryPath = "C:\\SFM";
            var configParser = new ConfigParser(configLocation, new MockFileSystem());
            var doc = new XmlDocument();

            configParser.Write(configFile, doc);

            Assert.That(doc.ChildNodes.Count == 1);
            Assert.That(configFile.Equals(ConfigFile.FromXML((XmlElement)doc.FirstChild)));
        }
    }
}
