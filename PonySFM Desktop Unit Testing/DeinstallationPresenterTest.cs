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
    public class DeinstallationPresenterTest
    {
        [TestMethod]
        public async Task ExecutesCorrectly()
        {
            var configFile = new ConfigFile("C:\\SFM");
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
            Assert.IsTrue(revisionManager.VerifyInstalled(revision, null));

            var list = new List<int>();
            list.Add(1);

            var deinstallationPresenter = new DeinstallationPresenter(revisionManager, list);

            await deinstallationPresenter.Execute();

            Assert.IsFalse(revisionManager.VerifyInstalled(revision, null));

            foreach (var file in revision.Files)
            {
                Assert.IsFalse(fs.FileExists(file.Path));
            }
        }
    }
}
