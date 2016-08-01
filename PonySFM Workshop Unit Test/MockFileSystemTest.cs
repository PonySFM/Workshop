using Microsoft.VisualStudio.TestTools.UnitTesting;
using PonySFM_Workshop;

namespace PonySFM_Desktop.Test
{
    [TestClass]
    public class MockFileSystemTest
    {
        [TestMethod]
        [TestCategory("MockFileSystem")]
        public void GetFilesReturnsCorrectFiles()
        {
            MockFileSystem fs = new MockFileSystem();

            fs.CreateFile("C:\\someFolder\\myFile.txt");
            fs.CreateFile("C:\\someFolder\\gaben.txt");

            var files = fs.GetFiles("C:\\someFolder");
            Assert.AreEqual(files.Count, 2);
        }

        [TestMethod]
        [TestCategory("MockFileSystem")]
        public void CopyFile()
        {
            MockFileSystem fs = new MockFileSystem();
            fs.CreateFile("C:\\someFolder\\myFile.txt"); /* TODO: set data */
            fs.CopyFile("C:\\someFolder\\myFile.txt", "C:\\someFolder\\copy.txt");

            Assert.IsTrue(fs.FileExists("C:\\someFolder\\myFile.txt"));
            Assert.IsTrue(fs.FileExists("C:\\someFolder\\copy.txt"));
        }
    }
}
