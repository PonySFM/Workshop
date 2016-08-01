using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using PonySFM_Workshop;

namespace PonySFM_Desktop_Unit_Testing
{
    [TestClass]
    public class RevisionTest
    {
        [TestMethod]
        [TestCategory("RevisionTest")]
        public void CreatesTempRevisionCorrectly()
        {
            var fs = new MockFileSystem();
            var tmp = Path.GetTempFileName();

            fs.CreateDirectory(Path.Combine(tmp, "models"));
            fs.CreateDirectory(Path.Combine(tmp, "materials"));

            fs.CreateFile(Path.Combine(tmp, "materials", "material.ext"));
            fs.CreateFile(Path.Combine(tmp, "models", "model.ext"));

            var revision = Revision.CreateTemporaryRevisionFromFolder(1337, tmp, fs);

            Assert.AreEqual(2, revision.Files.Count);

            Assert.IsTrue(revision.Files.Exists(r => r.Path == Path.Combine(tmp, "materials", "material.ext")));
            Assert.IsTrue(revision.Files.Exists(r => r.Path == Path.Combine(tmp, "models", "model.ext")));
        }
    }
}
