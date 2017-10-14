using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using CoreLib;

namespace CoreLibTest
{
    [TestClass]
    public class ConfigFileTest
    {
        [TestMethod]
        [TestCategory("ConfigFile")]
        public void EnsureCorrectXMLParsing()
        {
            var configFile = new ConfigFile("C:\\Gaben");
            var fakeDoc = new XmlDocument();

            var xml = configFile.ToXml(fakeDoc);
            var parsedFromXML = ConfigFile.FromXml(xml);

            Assert.IsTrue(parsedFromXML.Equals(configFile), "Converting to XML and then deconverting should return same result");
        }
    }
}
