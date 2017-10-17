using System.Linq;
using CoreLib.Interface;

namespace CoreLib
{
    public class SFMDirectoryParser
    {
        private readonly IFileSystem _fs;

        public string Path { get; }
        public string GameinfoPath { get; }
        public string InstallationPath { get; }

        public SFMDirectoryParser(string filepath, IFileSystem fs)
        {
            Path = filepath;
            _fs = fs;

            if (_fs.DirectoryExists(Path))
            {
                foreach (var dir in fs.GetDirectories(Path))
                {
                    if (dir.Name.ToLower() != "game") continue;

                    Path = dir.Path;
                    break;
                }
            }

            GameinfoPath = System.IO.Path.Combine(Path, "usermod", "gameinfo.txt");
            InstallationPath = System.IO.Path.Combine(Path, "ponysfm");
        }

        public void CreateDirectories()
        {
            if (!_fs.DirectoryExists(InstallationPath))
                _fs.CreateDirectory(InstallationPath);
        }

        public SFMDirectoryParserError Validate()
        {
            if ((!_fs.DirectoryExists(Path)))
                return SFMDirectoryParserError.NotExists;

            if (!HasCorrectSubDirs())
                return SFMDirectoryParserError.NotLikely;

            return SFMDirectoryParserError.Ok;
        }

        private bool HasCorrectSubDirs()
        {
            string[] typicalSubDirs = { "bin", "platform", "tf" };
            return typicalSubDirs.All(d => _fs.DirectoryExists(System.IO.Path.Combine(Path, d)));
        } 
    }
}
