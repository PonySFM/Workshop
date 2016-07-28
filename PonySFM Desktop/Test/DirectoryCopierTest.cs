using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PonySFM_Desktop.Test
{
    [TestFixture]
    public class DirectoryCopierTest
    {
        [Test]
        public void CopiesDirectoryCorrectlyOnlyFiles()
        {
            var fs = new MockFileSystem();
            var directoryCopier = new DirectoryCopier(fs, "C:\\fakeDir", "C:\\SFM", false);

            fs.CreateDirectory("C:\\fakeDir");
            fs.CreateFile("C:\\fakeDir\\file1.txt");
            fs.CreateFile("C:\\fakeDir\\file2.txt");

            directoryCopier.Execute();

            Assert.True(fs.FileExists("C:\\SFM\\file1.txt"));
            Assert.True(fs.FileExists("C:\\SFM\\file2.txt"));
        }

        [Test]
        public void CopiesDirectoryCorrectlyWithSubDirs()
        {
            var fs = new MockFileSystem();
            var directoryCopier = new DirectoryCopier(fs, "C:\\fakeDir", "C:\\SFM", true);

            fs.CreateDirectory("C:\\fakeDir");
            fs.CreateFile("C:\\fakeDir\\file1.txt");
            fs.CreateFile("C:\\fakeDir\\file2.txt");

            fs.CreateDirectory("C:\\fakeDir\\folder");
            fs.CreateFile("C:\\fakeDir\\folder\\file3.txt");

            fs.CreateDirectory("C:\\fakeDir\\folder\\folder2");
            fs.CreateFile("C:\\fakeDir\\folder\\folder2\\file4.txt");

            directoryCopier.Execute();

            Assert.True(fs.FileExists("C:\\SFM\\file1.txt"));
            Assert.True(fs.FileExists("C:\\SFM\\file2.txt"));

            Assert.True(fs.DirectoryExists("C:\\SFM\\folder"));
            Assert.True(fs.FileExists("C:\\SFM\\folder\\file3.txt"));

            Assert.True(fs.DirectoryExists("C:\\SFM\\folder\\folder2"));
            Assert.True(fs.FileExists("C:\\SFM\\folder\\folder2\\file4.txt"));
        }

        [Test]
        public void FiresEventCorrectly()
        {
            var fs = new MockFileSystem();
            var directoryCopier = new DirectoryCopier(fs, "C:\\fakeDir", "C:\\SFM", true);

            fs.CreateFile("C:\\fakeDir\\file1.txt");
            fs.CreateFile("C:\\SFM\\file1.txt");

            DirectoryCopierFileExistsEventArgs eventArgs = null;
            directoryCopier.OnFileExists += delegate (object sender, DirectoryCopierFileExistsEventArgs e)
            {
                eventArgs = e;
            };

            directoryCopier.Execute();

            Assert.NotNull(eventArgs);

            Assert.AreEqual(eventArgs.File1, "C:\\fakeDir\\file1.txt");
            Assert.AreEqual(eventArgs.File2, "C:\\SFM\\file1.txt");
        }

        [Test]
        public void FiresEventCorrectlyWithOverwrite()
        {
            var fs = new MockFileSystem();
            var directoryCopier = new DirectoryCopier(fs, "C:\\fakeDir", "C:\\SFM", true);

            var d1 = Encoding.UTF8.GetBytes("Hello");
            var d2 = Encoding.UTF8.GetBytes("Konnichiwa");

            fs.CreateFile("C:\\fakeDir\\file1.txt", d1);
            fs.CreateFile("C:\\SFM\\file1.txt", d2);

            DirectoryCopierFileExistsEventArgs eventArgs = null;
            directoryCopier.OnFileExists += delegate (object sender, DirectoryCopierFileExistsEventArgs e)
            {
                eventArgs = e;
                e.ShouldCopy = true;
            };

            directoryCopier.Execute();

            Assert.NotNull(eventArgs);

            Assert.AreEqual(eventArgs.File1, "C:\\fakeDir\\file1.txt");
            Assert.AreEqual(fs.ReadFile("C:\\fakeDir\\file1.txt"), d1);

            Assert.AreEqual(eventArgs.File2, "C:\\SFM\\file1.txt");
            Assert.AreEqual(fs.ReadFile("C:\\SFM\\file1.txt"), d1);
        }
    }
}
