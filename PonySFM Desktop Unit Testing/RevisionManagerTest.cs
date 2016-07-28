using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PonySFM_Desktop.Test
{
    [TestClass]
    public class RevisionManagerTest
    {
        private string dir = "C:\\SFM";

        [TestMethod]
        [TestCategory("RevisionManager")]
        public void CreatesDirectory()
        {
            var configFile = new ConfigFile();
            configFile.SFMDirectoryPath = dir;
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            Assert.IsTrue(fs.DirectoryExists(dir));
        }
    }
}
