using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PonySFM_Desktop.Test
{
    [TestFixture]
    public class SFMDirectoryParserTest
    {
        [Test]
        public void IdentifyInvalidDirectory()
        {
            string baseDir = "C:\\SecretPlace\\";
            var fs = new MockFileSystem();
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.That(parser.Validate() == SFMDirectoryParserError.NOT_EXISTS);
        }

        [Test]
        public void IdentifyNotLikelyDirectory()
        {
            string baseDir = "C:\\AwesomeStuff\\";
            var fs = new MockFileSystem();
            fs.AddFile(new MockFile(baseDir, MockFileType.DIRECTORY));
            string[] subDirs = { "Ponies", "NSFWStuff" };
            foreach (var d in subDirs)
            {
                fs.AddFile(new MockFile(Path.Combine(baseDir, d), MockFileType.DIRECTORY));
            }
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.That(parser.Validate() == SFMDirectoryParserError.NOT_LIKELY);
        }

        [Test]
        public void IdentifyCorrectDirectory()
        {
            string baseDir = "C:\\SourceFilmmaker\\game";
            var fs = new MockFileSystem();
            /* Create a fake default SFM installation */
            fs.AddFile(new MockFile(baseDir, MockFileType.DIRECTORY));
            string[] subDirs = { "bin", "hl2", "left4dead2_movies", "platform", "sdktools", "tf", "tf_movies", "usermod" };
            foreach (var d in subDirs)
            {
                fs.AddFile(new MockFile(Path.Combine(baseDir, d), MockFileType.DIRECTORY));
            }
            var parser = new SFMDirectoryParser(baseDir, fs);
            Assert.That(parser.Validate() == SFMDirectoryParserError.OK);
        }
    }
}
