using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace PonySFM_Desktop.Test
{
    [TestClass]
    public class ConfigParserTest
    {
        string configLocation = "C:\\ponysfm\\config.xml";

        [TestMethod]
        [TestCategory("ConfigParser")]
        public void ChecksForExistenceCorrectly()
        {
            var fs = new MockFileSystem();
            var configParser = new ConfigParser(configLocation, fs);

            Assert.IsFalse(configParser.Exists());
            fs.AddFile(new MockFile("C:\\ponysfm\\config.xml", MockFileType.File));
            Assert.IsTrue(configParser.Exists());
        }

        [TestMethod]
        [TestCategory("ConfigParser")]
        public void ParsesCorrectly()
        {
            var doc = new XmlDocument();
            var configFile = new ConfigFile();
            var configParser = new ConfigParser(configLocation, new MockFileSystem());
            configFile.SFMDirectoryPath = "C:\\SFM";
            doc.AppendChild(configFile.ToXML(doc));

            var parsedConfig = configParser.Read(doc);

            Assert.IsTrue(parsedConfig.Equals(configFile));
            Assert.IsTrue(doc.ChildNodes.Count == 1);
        }

        [TestMethod]
        [TestCategory("ConfigParser")]
        public void WritesCorrectly()
        {
            var configFile = new ConfigFile();
            configFile.SFMDirectoryPath = "C:\\SFM";
            var configParser = new ConfigParser(configLocation, new MockFileSystem());
            var doc = new XmlDocument();

            configParser.Write(configFile, doc);

            Assert.IsTrue(doc.ChildNodes.Count == 1);
            Assert.IsTrue(configFile.Equals(ConfigFile.FromXML((XmlElement)doc.FirstChild)));
        }
    }
}
