using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using CoreLib;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestClass]
    public class ConfigParserTest
    {
        [TestMethod]
        [TestCategory("ConfigParser")]
        public void ParsesCorrectly()
        {
            var doc = new XmlDocument();
            var configFile = new ConfigFile("C:\\SFM");
            var configParser = new ConfigParser(new MockFileSystem());
            doc.AppendChild(configFile.ToXml(doc));

            var parsedConfig = configParser.Read(doc);

            Assert.IsTrue(parsedConfig.Equals(configFile));
            Assert.IsTrue(doc.ChildNodes.Count == 1);
        }

        [TestMethod]
        [TestCategory("ConfigParser")]
        public void WritesCorrectly()
        {
            var configFile = new ConfigFile("C:\\SFM");
            var configParser = new ConfigParser(new MockFileSystem());
            var doc = new XmlDocument();

            configParser.Write(configFile, doc);

            Assert.IsTrue(doc.ChildNodes.Count == 1);
            Assert.IsTrue(configFile.Equals(ConfigFile.FromXml((XmlElement)doc.FirstChild)));
        }
    }
}
