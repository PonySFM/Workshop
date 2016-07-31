using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonySFM_Desktop;

namespace PonySFM_Desktop_Unit_Testing
{
    [TestClass]
    public class TempRevisionParserTest
    {
        [TestMethod]
        public void FindsModFolder()
        {
            var fs = new MockFileSystem();
            fs.CreateDirectory("C:\\temp");
            fs.CreateDirectory("C:\\temp\\myCoolMod");
            fs.CreateDirectory("C:\\temp\\myCoolMod\\materials");
            fs.CreateDirectory("C:\\temp\\myCoolMod\\models");

            var parser = new TempRevisionParser("C:\\temp", fs);
            var dir = parser.FindModFolder();

            Assert.AreEqual("C:\\temp\\myCoolMod", dir);
        }
    }
}
