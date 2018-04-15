using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreLib;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestFixture]
    public class RevisionManagerTest
    {
        private const string Dir = "C:\\SFM";

        [Test]
        public void CreatesDirectory()
        {
            var configFile = new ConfigFile(Dir);
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            Assert.IsTrue(fs.DirectoryExists(Dir));
        }

        [Test]
        public async Task InstallsRevisionCorrectly()
        {
            var configFile = new ConfigFile(Dir);
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            Util.CreateSFMDirectory(fs);
            var revision = Util.CreateFakeTempRevision(fs);

            await revisionManager.InstallRevision(revision, "C:\\tmp", null);
            Assert.IsTrue(revisionManager.VerifyInstalled(revision, null));
        }

        [Test]
        public async Task UninstallsRevisionCorrectly()
        {
            var configFile = new ConfigFile(Dir);
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            Util.CreateSFMDirectory(fs);
            var revision = Util.CreateFakeTempRevision(fs);

            await revisionManager.InstallRevision(revision, "C:\\tmp", null);
            Assert.IsTrue(revisionManager.VerifyInstalled(revision, null));

            await revisionManager.UninstallRevision(revision.ID, null);
            Assert.IsFalse(revisionManager.VerifyInstalled(revision, null));

            foreach (var file in revision.Files)
            {
                Assert.IsFalse(fs.FileExists(file.Path));
            }
        }
    }
}
