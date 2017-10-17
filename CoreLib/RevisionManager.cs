using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreLib.Interface;

namespace CoreLib
{
    public class RevisionManager
    {
        private readonly ConfigFile _configFile;
        private readonly IFileSystem _fs;
        private readonly string _path;
        private readonly SfmDirectoryParser _dirParser;

        public delegate void FileExistsHandler(object sender, DirectoryCopierFileExistsEventArgs e);
        public event FileExistsHandler OnFileExists;

        public RevisionDatabase Database { get; }

        public RevisionManager(ConfigFile configFile, IFileSystem fs)
        {
            _configFile = configFile;
            _fs = fs;
            _path = configFile.SfmDirectoryPath;
            _dirParser = new SfmDirectoryParser(_path, fs);
            CreateDataFolder();
            Database = new RevisionDatabase(Path.Combine(_dirParser.InstallationPath, "ponysfm.xml"), _fs);
        }

        public void CreateDataFolder()
        {
            if (!_fs.DirectoryExists(_path))
                _fs.CreateDirectory(_path);

            _dirParser.CreateDirectories();
        }

        public async Task<InstallationResult> InstallRevision(Revision revision, string topDir, IProgress<int> progress, CancellationToken cancel = default(CancellationToken))
        {
            /* Copy files and blahblah */
            var directoryCopier = new DirectoryCopier(_fs, topDir, _dirParser.InstallationPath, true);

            directoryCopier.OnProgress += (s, e) =>
                progress?.Report(e.Progress);

            directoryCopier.OnFileExists += (s, e) =>
                OnFileExists?.Invoke(s, e);

            try
            {
                await directoryCopier.Execute(cancel);
            }
            catch(OperationCanceledException)
            {
                return InstallationResult.Cancelled;
            }

            revision.ChangeTopDirectory(topDir, _dirParser.InstallationPath);
            revision.Metadata["InstallationTime"] = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            Database.AddToDb(revision);
            Database.WriteDbDisk();

            return InstallationResult.Success;
        }

        public async Task UninstallRevision(int id, IProgress<int> progress)
        {
            var revision = Database.Revisions.Find(r => r.ID == id);

            if (revision == null)
                return;

            var totalCount = revision.Files.Count;
            var i = 0;

            await Task.Factory.StartNew(() =>
            {
                Database.RemoveRevision(id);

                /* FIXME: totally guranteed to be sorted by directory! */
                foreach (var file in revision.Files)
                {
                    i++;

                    if (_fs.FileExists(file.Path))
                        _fs.DeleteFile(file.Path);
                    else if(_fs.DirectoryExists(file.Path))
                        _fs.DeleteDirectory(file.Path);

                    progress?.Report(i / totalCount * 100);
                }

                Database.WriteDbDisk();
            });
        }

        public bool VerifyInstalled(Revision revision, IProgress<int> progress)
        {
            var i = 0;
            foreach (var file in revision.Files)
            {
                if (!_fs.FileExists(file.Path))
                    return false;

                if (_fs.GetChecksum(file.Path) != file.Sha512)
                    return false;

                var p = i / (double)revision.Files.Count * 100;
                progress?.Report((int)p);
                i++;
            }

            return true;
        }

        public bool VerifyInstalled(int id, IProgress<int> progress)
        {
            var revision = Database.Revisions.Find(x => x.ID == id);

            return revision != null && VerifyInstalled(revision, progress);
        }

        public bool IsInstalled(int id)
        {
            return Database.Revisions.Find(x => x.ID == id) != null;
        }
    }
}
