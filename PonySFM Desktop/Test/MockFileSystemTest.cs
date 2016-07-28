using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PonySFM_Desktop.Test
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

            Assert.True(fs.FileExists("C:\\someFolder\\myFile.txt"));
            Assert.True(fs.FileExists("C:\\someFolder\\copy.txt"));
        }
    }
}
