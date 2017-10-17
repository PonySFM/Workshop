using NUnit.Framework;
using System.IO;
using CoreLib;
using CoreLib.Impl;

namespace CoreLibTest
{
    [TestFixture]
    public class SFMDirectoryParserTest
    {
        [Test]
        public void IdentifyInvalidDirectory()
        {
            const string baseDir = "C:\\SecretPlace\\";
            var fs = new MockFileSystem();
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.AreEqual(SFMDirectoryParserError.NotExists, parser.Validate());
        }

        [Test]
        public void ShouldUseGameDirIfItExists()
        {
            const string baseDir = "C:\\SFM\\";
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

        [Test]
        public void IdentifyNotLikelyDirectory()
        {
            const string baseDir = "C:\\AwesomeStuff\\";
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

        [Test]
        public void IdentifyCorrectDirectory()
        {
            const string baseDir = "C:\\SourceFilmmaker\\game";
            var fs = new MockFileSystem();
            /* Create a fake default SFM installation */
            fs.AddFile(new MockFile(baseDir, MockFileType.Directory));
            string[] subDirs = { "bin", "hl2", "left4dead2_movies", "platform", "sdktools", "tf", "tf_movies", "usermod" };
            foreach (var d in subDirs)
            {
                fs.AddFile(new MockFile(Path.Combine(baseDir, d), MockFileType.Directory));
            }
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.AreEqual(SFMDirectoryParserError.Ok, parser.Validate());
        }
    }
}
