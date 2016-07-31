using System;
using System.IO;
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

        public RevisionDatabase Database
        {
            get
            {
                return _db;
            }
        }

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

        public async Task InstallRevision(Revision revision, string topDir, IProgress<int> progress)
        {
            /* Copy files and blahblah */
            var directoryCopier = new DirectoryCopier(_fs, topDir, _dirParser.InstallationPath, true);

            if (progress != null)
                directoryCopier.OnProgress += delegate (object sender, DirectoryProgressEventArgs e)
                {
                    progress.Report(e.Progress);
                };

            await directoryCopier.Execute();

            revision.ChangeTopDirectory(topDir, _dirParser.InstallationPath);
            _db.AddToDB(revision);

            _db.WriteDBDisk();
        }

        public async Task UninstallRevision(int id, IProgress<int> progress)
        {
            var revision = _db.Revisions.Find(r => r.ID == id);
            if (revision == null)
                throw new ArgumentException("Cannot find that revision!");

            int totalCount = revision.Files.Count;
            int i = 0;

            await Task.Factory.StartNew(() =>
            {
                _db.RemoveRevision(id);

                /* FIXME: totally guranteed to be sorted by directory! */
                foreach (var file in revision.Files)
                {
                    i++;

                    if (_fs.FileExists(file.Path))
                        _fs.DeleteFile(file.Path);
                    else
                        _fs.DeleteDirectory(file.Path);

                    progress?.Report(i / totalCount * 100);
                }

                _db.WriteDBDisk();
            });
        }

        public bool VerifyInstalled(Revision revision)
        {
            foreach (var file in revision.Files)
            {
                if (!_fs.FileExists(file.Path))
                    return false;

                if (_fs.GetChecksum(file.Path) != file.Sha512)
                    return false;
            }

            return true;
        }
    }
}
