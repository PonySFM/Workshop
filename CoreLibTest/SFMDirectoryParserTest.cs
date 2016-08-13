using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoreLib;
using CoreLib.Impl;
using CoreLib.Interface;

namespace CoreLibTest
{
    [TestClass]
    public class SFMDirectoryParserTest
    {
        [TestMethod]
        [TestCategory("SFMDirectoryParser")]
        public void IdentifyInvalidDirectory()
        {
            string baseDir = "C:\\SecretPlace\\";
            var fs = new MockFileSystem();
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.AreEqual(SFMDirectoryParserError.NotExists, parser.Validate());
        }

        [TestMethod]
        [TestCategory("SFMDirectoryParser")]
        public void ShouldUseGameDirIfItExists()
        {
            string baseDir = "C:\\SFM\\";
            var fs = new MockFileSystem();
            fs.CreateDirectory(baseDir);
            fs.CreateDirectory(baseDir + "game");
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.AreEqual("C:\\SFM\\game", parser.Path);

            /* But not in this case */
            fs.CreateDirectory("C:\\SFM2");
            fs.CreateDirectory("C:\\SFM2\\gaben");
            var otherParser = new SFMDirectoryParser("C:\\SFM2", fs);
            Assert.AreEqual("C:\\SFM2", otherParser.Path);
        }

        [TestMethod]
        [TestCategory("SFMDirectoryParser")]
        public void IdentifyNotLikelyDirectory()
        {
            string baseDir = "C:\\AwesomeStuff\\";
            var fs = new MockFileSystem();
            fs.AddFile(new MockFile(baseDir, MockFileType.Directory));
            string[] subDirs = { "Ponies", "NSFWStuff" };
            foreach (var d in subDirs)
            {
                fs.AddFile(new MockFile(Path.Combine(baseDir, d), MockFileType.Directory));
            }
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.AreEqual(SFMDirectoryParserError.NotLikely, parser.Validate());
        }

        [TestMethod]
        [TestCategory("SFMDirectoryParser")]
        public void IdentifyCorrectDirectory()
        {
            string baseDir = "C:\\SourceFilmmaker\\game";
            var fs = new MockFileSystem();
            /* Create a fake default SFM installation */
            fs.AddFile(new MockFile(baseDir, MockFileType.Directory));
            string[] subDirs = { "bin", "hl2", "left4dead2_movies", "platform", "sdktools", "tf", "tf_movies", "usermod" };
            foreach (var d in subDirs)
            {
                fs.AddFile(new MockFile(Path.Combine(baseDir, d), MockFileType.Directory));
            }
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.AreEqual(SFMDirectoryParserError.OK, parser.Validate());
        }
    }
}
