using NUnit.Framework;
using System.IO;
using System.Linq;
using CoreLib;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestFixture]
    public class RevisionTest
    {
        [Test]
        public void CreatesTempRevisionCorrectly()
        {
            var fs = new MockFileSystem();

            var rev1 = Util.CreateFakeTempRevision(fs);
            var rev2 = Revision.CreateTemporaryRevisionFromFolder(1337, "C:\\tmp", fs);

            Assert.AreEqual(rev1.Files.Count, rev2.Files.Count);
            foreach (var file in rev1.Files)
            {
                Assert.IsTrue(rev2.Files.Any(x => x.Path == file.Path));
            }
        }
    }
}
