using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PonySFM_Desktop
{
    public enum SFMDirectoryParserError
    {
        OK,
        NOT_EXISTS,
        NOT_LIKELY,
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

        public SFMDirectoryParser(string filepath, IFileSystem fs)
        {
            _path = filepath;
            _fs = fs;
            _gameinfoPath = System.IO.Path.Combine(_path, "usermod", "gameinfo");
            _installationPath = System.IO.Path.Combine(_path, "ponysfm");
        }

        public SFMDirectoryParserError Validate()
        {
            if ((!_fs.DirectoryExists(_path)))
                return SFMDirectoryParserError.NOT_EXISTS;

            if (!HasCorrectSubDirs())
                return SFMDirectoryParserError.NOT_LIKELY;

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
