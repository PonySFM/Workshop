using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Xml;

namespace PonySFM_Desktop.Test
{
    [TestFixture]
    public class ConfigFileTest
    {
        [Test]
        public void EnsureCorrectXMLParsing()
        {
            var configFile = new ConfigFile();
            configFile.SFMDirectoryPath = "C:\\Gaben";
            var fakeDoc = new XmlDocument();

            var xml = configFile.ToXML(fakeDoc);
            var parsedFromXML = ConfigFile.FromXML(xml);

            /* Converting to XML and then correctly deconverting should return same result */
            Assert.That(parsedFromXML.Equals(configFile));
        }
    }
}
