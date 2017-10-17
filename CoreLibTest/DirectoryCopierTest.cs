using NUnit.Framework;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using CoreLib;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestFixture]
    public class DirectoryCopierTest
    {
        [Test]
        public async Task CopiesDirectoryCorrectlyOnlyFiles()
        {
            var fs = new MockFileSystem();
            var directoryCopier = new DirectoryCopier(fs, "C:\\fakeDir", "C:\\SFM", false);

            fs.CreateDirectory("C:\\fakeDir");
            fs.CreateFile("C:\\fakeDir\\file1.txt");
            fs.CreateFile("C:\\fakeDir\\file2.txt");

            await directoryCopier.Execute();

            Assert.IsTrue(fs.FileExists("C:\\SFM\\file1.txt"));
            Assert.IsTrue(fs.FileExists("C:\\SFM\\file2.txt"));
        }

        [Test]
        public async Task CopiesDirectoryCorrectlyWithSubDirs()
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

            await directoryCopier.Execute();

            Assert.IsTrue(fs.FileExists("C:\\SFM\\file1.txt"));
            Assert.IsTrue(fs.FileExists("C:\\SFM\\file2.txt"));
            Assert.IsTrue(fs.DirectoryExists("C:\\SFM\\folder"));
            Assert.IsTrue(fs.FileExists("C:\\SFM\\folder\\file3.txt"));
            Assert.IsTrue(fs.DirectoryExists("C:\\SFM\\folder\\folder2"));
            Assert.IsTrue(fs.FileExists("C:\\SFM\\folder\\folder2\\file4.txt"));
        }

        [Test]
        public async Task FiresEventCorrectly()
        {
            var fs = new MockFileSystem();
            var directoryCopier = new DirectoryCopier(fs, "C:\\fakeDir", "C:\\SFM", true);
            var copiedFiles = new List<Tuple<string,string>>();
            var progressHistory = new List<int>();

            fs.CreateFile("C:\\fakeDir\\file1.txt");
            fs.CreateFile("C:\\SFM\\file1.txt");

            DirectoryCopierFileExistsEventArgs eventArgs = null;
            directoryCopier.OnFileExists += delegate (object sender, DirectoryCopierFileExistsEventArgs e)
            {
                eventArgs = e;
            };

            directoryCopier.OnFileCopy += delegate (object sender, DirectoryCopierCopyEventArgs e)
            {
                copiedFiles.Add(new Tuple<string, string>(e.File1, e.File2));
            };

            directoryCopier.OnProgress += delegate (object sender, DirectoryProgressEventArgs e)
            {
                progressHistory.Add(e.Progress);
            };

            await directoryCopier.Execute();

            Assert.AreNotEqual(null, eventArgs);
            Assert.AreEqual(eventArgs.File1, "C:\\fakeDir\\file1.txt");
            Assert.AreEqual(eventArgs.File2, "C:\\SFM\\file1.txt");

            Assert.IsTrue(copiedFiles.Contains(new Tuple<string, string>("C:\\fakeDir\\file1.txt", "C:\\SFM\\file1.txt")));

            Assert.IsTrue(progressHistory.Contains(100));
        }

        [Test]
        public async Task FiresEventCorrectlyWithOverwrite()
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
                e.FileCopyMode = DirectoryCopierFileCopyMode.Copy;
            };

            await directoryCopier.Execute();

            Assert.AreNotEqual(null, eventArgs);

            Assert.AreEqual(eventArgs.File1, "C:\\fakeDir\\file1.txt");
            Assert.AreEqual(fs.ReadFile("C:\\fakeDir\\file1.txt"), d1);

            Assert.AreEqual(eventArgs.File2, "C:\\SFM\\file1.txt");
            Assert.AreEqual(fs.ReadFile("C:\\SFM\\file1.txt"), d1);
        }

        [Test]
        public async Task CopyAllShouldSkipEvents()
        {
            var fs = new MockFileSystem();
            var directoryCopier = new DirectoryCopier(fs, "C:\\fakeDir", "C:\\SFM", true);

            var d1 = Encoding.UTF8.GetBytes("Hello");
            var d2 = Encoding.UTF8.GetBytes("Konnichiwa");

            fs.CreateFile("C:\\fakeDir\\file1.txt", d1);
            fs.CreateFile("C:\\fakeDir\\file2.txt", d2);

            fs.CreateFile("C:\\SFM\\file1.txt");
            fs.CreateFile("C:\\SFM\\file2.txt");

            var timesEventCalled = 0;

            directoryCopier.OnFileExists += delegate (object sender, DirectoryCopierFileExistsEventArgs e)
            {
                e.FileCopyMode = DirectoryCopierFileCopyMode.CopyAll;
                timesEventCalled++;
            };

            await directoryCopier.Execute();

            /* Setting FileCopyMode to CopyAll should disable the event */
            Assert.AreEqual(1, timesEventCalled);

            Assert.AreEqual(fs.ReadFile("C:\\SFM\\file1.txt"), d1);
            Assert.AreEqual(fs.ReadFile("C:\\SFM\\file2.txt"), d2);
        }
    }
}
