using System;
using System.IO;
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
        RevisionDatabase _db;
        SFMDirectoryParser _dirParser;

        public RevisionManager(ConfigFile configFile, IFileSystem fs)
        {
            _configFile = configFile;
            _fs = fs;
            _path = configFile.SFMDirectoryPath;
            _dirParser = new SFMDirectoryParser(_path, fs);
            _db = new RevisionDatabase(Path.Combine(_dirParser.InstallationPath, "ponysfm.xml"), _fs);

            CreateDataFolder();
        }

        public void CreateDataFolder()
        {
            if (!_fs.DirectoryExists(_path))
                _fs.CreateDirectory(_path);

            _dirParser.CreateDirectories();
        }

        public void InstallRevision(Revision revision, string topDir)
        {
            _db.AddToDB(revision);
            /* Copy files and blahblah */
            var directoryCopier = new DirectoryCopier(_fs, topDir, _dirParser.InstallationPath, true);
            /* TODO: delegate */
            directoryCopier.Execute();

            _db.WriteDBDisk();
        }

        public void UninstallRevision(int id)
        {
            var revision = _db.Revisions.Find(r => r.ID == id);
            if (revision == null)
                throw new ArgumentException("Cannot find that revision!");

            _db.RemoveRevision(id);

            /* FIXME: totally guranteed to be sorted by directory! */
            foreach (var file in revision.Files)
            {
                if (_fs.FileExists(file))
                    _fs.DeleteFile(file);
                else
                    _fs.DeleteDirectory(file);
            }

            _db.WriteDBDisk();
        }
    }
}
