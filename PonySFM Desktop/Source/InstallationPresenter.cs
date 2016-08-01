using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PonySFM_Desktop
{
    /* TODO: I have a feeling this class does way too much for just being a presenter */
    /* May encapsulate the whole installion-process and adapters to progress event in a way */
    /* that actually looks nice */
    public class InstallationPresenter : BasePresenter
    {
        int _progress;
        string _installationLog;
        Dictionary<string, int> _progresses = new Dictionary<string, int>();

        public int MaxProgress { get { return _progresses.Count * 100;  } }

        public int Progress
        {
            get
            {
                return _progress;
            }
            set
            {
                _progress = value;
                NotifyPropertyChange("Progress");
            }
        }

        public string InstallationLog
        {
            get
            {
                return _installationLog;
            }
            set
            {
                _installationLog = value;
                NotifyPropertyChange("InstallationLog");
            }
        }

        public string Status { get; private set; }

        IAPIConnector _api;
        IFileSystem _fs;
        RevisionManager _revisionMgr;
        string _currentProgressState;
        List<int> _ids;

        public InstallationPresenter(IAPIConnector api, IFileSystem fs, RevisionManager revisionMgr, List<int> ids)
        {
            _api = api;
            _fs = fs;
            _revisionMgr = revisionMgr;
            _ids = ids;
            _progress = 0;
            _progresses.Add("download", 0);
            _progresses.Add("extraction", 0);
            _progresses.Add("installation", 0);
        }

        public async Task Execute()
        {
            foreach (var id in _ids)
            {
                await ExecuteInstallation(id);
            }
        }

        public async Task ExecuteInstallation(int id)
        {
            var zipTmp = _fs.GetTempPath();
            var tempDir = _fs.GetTempPath();

            var progress = new Progress<int>(i => SetProgress(_currentProgressState, i));

            if (!_fs.DirectoryExists(tempDir))
                _fs.CreateDirectory(tempDir);

            LogInstallation("Installing revision "+id+"\n");

            _currentProgressState = "download";
            LogInstallation("Downloading file...\n");
            await _api.DownloadRevisionZIP(id, zipTmp, progress);

            _currentProgressState = "extraction";
            LogInstallation("Extracting zip file...\n");
            var zip = _fs.OpenZIP(zipTmp);
            await zip.Extract(tempDir, progress);

            var parser = new TempRevisionParser(tempDir, _fs);
            var modDir = parser.FindModFolder();

            if (string.IsNullOrEmpty(modDir))
                throw new NotImplementedException();

            _currentProgressState = "installation";
            LogInstallation("Installing files to SFM...\n");
            var tmpRev = Revision.CreateTemporaryRevisionFromFolder(id, modDir, _fs);
            await _api.DownloadRevisionAdditionalInformation(tmpRev);
            await _revisionMgr.InstallRevision(tmpRev, modDir, progress);

            /* If we don't do this the directory deletion crashes because the handle created in zip.Extract is not released properly? */
            System.GC.Collect(); 
            System.GC.WaitForPendingFinalizers();

            LogInstallation("Cleaning up...\n");
            if (zip is MockZIPFile)
                _fs.DeleteDirectory(zipTmp);
            else
                _fs.DeleteFile(zipTmp);

            _fs.DeleteDirectory(tempDir);
        }

        private void LogInstallation(string msg)
        {
            InstallationLog += msg;
        }

        private void SetProgress(string name, int p)
        {
            _progresses[name] = p;
            Progress = _progresses.Sum(x => x.Value);
        }
    }
}
