using NUnit.Framework;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestFixture]
    public class MockFileSystemTest
    {
        [Test]
        public void GetFilesReturnsCorrectFiles()
        {
            var fs = new MockFileSystem();

            fs.CreateFile("C:\\someFolder\\myFile.txt");
            fs.CreateFile("C:\\someFolder\\gaben.txt");

            var files = fs.GetFiles("C:\\someFolder");
            Assert.AreEqual(files.Count, 2);
        }

        [Test]
        public void CopyFile()
        {
            var fs = new MockFileSystem();
            fs.CreateFile("C:\\someFolder\\myFile.txt"); /* TODO: set data */
            fs.CopyFile("C:\\someFolder\\myFile.txt", "C:\\someFolder\\copy.txt");

            Assert.IsTrue(fs.FileExists("C:\\someFolder\\myFile.txt"));
            Assert.IsTrue(fs.FileExists("C:\\someFolder\\copy.txt"));
        }
    }
}
