namespace PonySFM_Workshop
{
    public enum SFMDirectoryParserError
    {
        OK,
        NotExists,
        NotLikely,
    }

    public class SFMDirectoryParser
    {
        string _path;
        string _gameinfoPath;
        string _installationPath;
        IFileSystem _fs;

        public string Path
        {
            get
            {
                return _path;
            }
        }

        public string GameinfoPath
        {
            get
            {
                return _gameinfoPath;
            }

            set
            {
                _gameinfoPath = value;
            }
        }

        public string InstallationPath
        {
            get
            {
                return _installationPath;
            }

            set
            {
                _installationPath = value;
            }
        }

        public SFMDirectoryParser(string filepath, IFileSystem fs)
        {
            _path = filepath;
            _fs = fs;
            GameinfoPath = System.IO.Path.Combine(_path, "usermod", "gameinfo");
            InstallationPath = System.IO.Path.Combine(_path, "ponysfm");
        }

        public void CreateDirectories()
        {
            if (!_fs.DirectoryExists(InstallationPath))
                _fs.CreateDirectory(InstallationPath);
        }

        public SFMDirectoryParserError Validate()
        {
            if ((!_fs.DirectoryExists(_path)))
                return SFMDirectoryParserError.NotExists;

            if (!HasCorrectSubDirs())
                return SFMDirectoryParserError.NotLikely;

            return SFMDirectoryParserError.OK;
        }

        private bool HasCorrectSubDirs()
        {
            string[] typicalSubDirs = { "bin", "platform", "tf" };
            foreach (var d in typicalSubDirs)
                if (!_fs.DirectoryExists(System.IO.Path.Combine(_path, d)))
                    return false;
            return true;
        } 
    }
}
