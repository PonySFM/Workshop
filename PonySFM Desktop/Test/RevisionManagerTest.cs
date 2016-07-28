using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PonySFM_Desktop.Test
{
    [TestFixture]
    public class RevisionManagerTest
    {
        private string dir = "C:\\SFM";

        [Test]
        public void CreatesDirectory()
        {
            var configFile = new ConfigFile();
            configFile.SFMDirectoryPath = dir;
            var fs = new MockFileSystem();
            var revisionManager = new RevisionManager(configFile, fs);

            Assert.That(fs.DirectoryExists(dir));
        }

    }
}
