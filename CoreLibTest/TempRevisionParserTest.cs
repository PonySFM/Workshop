using NUnit.Framework;
using CoreLib;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestFixture]
    public class TempRevisionParserTest
    {
        [Test]
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
