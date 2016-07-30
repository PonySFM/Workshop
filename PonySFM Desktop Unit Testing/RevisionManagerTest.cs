using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var configFile = new ConfigFile(dir);
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            Assert.IsTrue(fs.DirectoryExists(dir));
        }

        [TestMethod]
        [TestCategory("RevisionManager")]
        public async Task InstallsRevisionCorrectly()
        {
            var configFile = new ConfigFile(dir);
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            fs.CreateDirectory("C:\\SFM");
            fs.CreateDirectory("C:\\SFM\\ponysfm");

            fs.CreateDirectory("C:\\tmp");
            fs.CreateDirectory("C:\\tmp\\models");
            fs.CreateFile("C:\\tmp\\models\\pony.vtf");

            fs.CreateDirectory("C:\\tmp\\materials");
            fs.CreateFile("C:\\tmp\\materials\\pony.vmt");

            var files = new List<RevisionFileEntry>();
            files.Add(RevisionFileEntry.FromFile("C:\\tmp\\models\\pony.vtf", fs));
            files.Add(RevisionFileEntry.FromFile("C:\\tmp\\materials\\pony.vmt", fs));

            var revision = new Revision(1, files);

            await revisionManager.InstallRevision(revision, "C:\\tmp", null);
            Assert.IsTrue(revisionManager.VerifyInstalled(revision));
        }
    }
}
