using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class RevisionManager
    {
        ConfigFile _configFile;
        IFileSystem _fs;
        string _path;

        public RevisionManager(ConfigFile configFile, IFileSystem fs)
        {
            _configFile = configFile;
            _fs = fs;
            _path = configFile.SFMDirectoryPath;

            CreateDataFolder();
        }

        public void CreateDataFolder()
        {
            if (!_fs.DirectoryExists(_path))
                _fs.CreateDirectory(_path);
        }
    }
}
