using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace PonySFM_Desktop.Test
{
    [TestClass]
    public class ConfigFileTest
    {
        [TestMethod]
        [TestCategory("ConfigFile")]
        public void EnsureCorrectXMLParsing()
        {
            var configFile = new ConfigFile();
            configFile.SFMDirectoryPath = "C:\\Gaben";
            var fakeDoc = new XmlDocument();

            var xml = configFile.ToXML(fakeDoc);
            var parsedFromXML = ConfigFile.FromXML(xml);

            /* Converting to XML and then correctly deconverting should return same result */
            Assert.IsTrue(parsedFromXML.Equals(configFile));
        }
    }
}
