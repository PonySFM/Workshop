using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreLib;
using CoreLib.Impl;
using PonySFM_Workshop;

namespace UITest
{
    [TestClass]
    public class InstallationPresenterTest
    {
        [TestMethod]
        public async Task ExecutesCorrectly()
        {
            var fs = new MockFileSystem();
            var api = new MockAPIConnector(fs);
            var revisionMgr = new RevisionManager(new ConfigFile("C:\\SFM"), fs);
            var ids = new List<int>() { 1337 };
            var installationPresenter = new InstallationPresenter(api, fs, revisionMgr, ids);

            /* Create fake revision to download */
            fs.CreateDirectory("C:\\tmp");
            fs.CreateDirectory("C:\\tmp\\models");
            fs.CreateFile("C:\\tmp\\models\\model.ext");
            fs.CreateDirectory("C:\\tmp\\materials");
            fs.CreateFile("C:\\tmp\\materials\\material.ext");
            var revision = Revision.CreateTemporaryRevisionFromFolder(1337, "C:\\tmp", fs);
            api.AddRevision("C:\\tmp", revision);

            await installationPresenter.Execute();

            Assert.IsTrue(revisionMgr.VerifyInstalled(revision, null));
        }
    }
}
