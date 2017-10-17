using System.Linq;
using CoreLib.Interface;

namespace CoreLib
{
    public class SfmDirectoryParser
    {
        private readonly IFileSystem _fs;

        public string Path { get; }
        public string GameinfoPath { get; }
        public string InstallationPath { get; }

        public SfmDirectoryParser(string filepath, IFileSystem fs)
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

        public SfmDirectoryParserError Validate()
        {
            if ((!_fs.DirectoryExists(Path)))
                return SfmDirectoryParserError.NotExists;

            if (!HasCorrectSubDirs())
                return SfmDirectoryParserError.NotLikely;

            return SfmDirectoryParserError.Ok;
        }

        private bool HasCorrectSubDirs()
        {
            string[] typicalSubDirs = { "bin", "platform", "tf" };
            return typicalSubDirs.All(d => _fs.DirectoryExists(System.IO.Path.Combine(Path, d)));
        } 
    }
}
