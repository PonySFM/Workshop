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
        private readonly string _dir = "C:\\SFM";

        [Test]
        public void CreatesDirectory()
        {
            var configFile = new ConfigFile(_dir);
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            Assert.IsTrue(fs.DirectoryExists(_dir));
        }

        [Test]
        public async Task InstallsRevisionCorrectly()
        {
            var configFile = new ConfigFile(_dir);
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            fs.CreateDirectory("C:\\SFM");
            fs.CreateDirectory("C:\\SFM\\ponysfm");

            fs.CreateDirectory("C:\\tmp");
            fs.CreateDirectory("C:\\tmp\\models");
            fs.CreateFile("C:\\tmp\\models\\pony.vtf");

            fs.CreateDirectory("C:\\tmp\\materials");
            fs.CreateFile("C:\\tmp\\materials\\pony.vmt");

            var files = new List<RevisionFileEntry>
            {
                RevisionFileEntry.FromFile("C:\\tmp\\models\\pony.vtf", fs),
                RevisionFileEntry.FromFile("C:\\tmp\\materials\\pony.vmt", fs)
            };

            var revision = new Revision(1, files);

            await revisionManager.InstallRevision(revision, "C:\\tmp", null);
            Assert.IsTrue(revisionManager.VerifyInstalled(revision, null));
        }

        [Test]
        public async Task UninstallsRevisionCorrectly()
        {
            var configFile = new ConfigFile(_dir);
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            fs.CreateDirectory("C:\\SFM");
            fs.CreateDirectory("C:\\SFM\\ponysfm");

            fs.CreateDirectory("C:\\tmp");
            fs.CreateDirectory("C:\\tmp\\models");
            fs.CreateFile("C:\\tmp\\models\\pony.vtf");

            fs.CreateDirectory("C:\\tmp\\materials");
            fs.CreateFile("C:\\tmp\\materials\\pony.vmt");

            var files = new List<RevisionFileEntry>
            {
                RevisionFileEntry.FromFile("C:\\tmp\\models\\pony.vtf", fs),
                RevisionFileEntry.FromFile("C:\\tmp\\materials\\pony.vmt", fs)
            };

            var revision = new Revision(1, files);

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
