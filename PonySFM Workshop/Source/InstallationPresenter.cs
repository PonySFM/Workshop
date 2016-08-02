using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PonySFM_Workshop
{
    /* TODO: I have a feeling this class does way too much for just being a presenter */
    /* May encapsulate the whole installion-process and adapters to progress event in a way */
    /* that actually looks nice */
    public class InstallationPresenter : BasePresenter
    {
        /// <summary>
        /// Private enumeration representing the progress state.
        /// </summary>
        enum ProgressState { Download, Extraction, Installation }

        int _progress;
        StringBuilder _installationLog = new StringBuilder();
        Dictionary<ProgressState, int> _progresses = new Dictionary<ProgressState, int>();

        IAPIConnector _api;
        IFileSystem _fs;
        RevisionManager _revisionMgr;
        ProgressState _currentProgressState;
        List<int> _ids;

        public int MaxProgress => _progresses.Count * 100;

        public string Status { get; private set; }

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
                return _installationLog.ToString();
            }
        }

        public InstallationPresenter(IAPIConnector api, IFileSystem fs, RevisionManager revisionMgr, List<int> ids)
        {
            _api = api;
            _fs = fs;
            _revisionMgr = revisionMgr;
            _ids = ids;
            _progress = 0;
            _progresses.Add(ProgressState.Download, 0);
            _progresses.Add(ProgressState.Extraction, 0);
            _progresses.Add(ProgressState.Installation, 0);
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

            LogInstallation("Installing revision " + id + "\n");

            _currentProgressState = ProgressState.Download;
            LogInstallation("Downloading file...\n");
            await _api.DownloadRevisionZIP(id, zipTmp, progress);

            _currentProgressState = ProgressState.Extraction;
            LogInstallation("Extracting zip file...\n");
            var zip = _fs.OpenZIP(zipTmp);
            await zip.Extract(tempDir, progress);

            var parser = new TempRevisionParser(tempDir, _fs);
            var modDir = parser.FindModFolder();

            if (string.IsNullOrWhiteSpace(modDir))
                throw new NotImplementedException();

            _currentProgressState = ProgressState.Installation;
            LogInstallation("Installing files to SFM...\n");
            Revision tmpRev = Revision.CreateTemporaryRevisionFromFolder(id, modDir, _fs);
            await _api.DownloadRevisionAdditionalInformation(tmpRev);
            await _revisionMgr.InstallRevision(tmpRev, modDir, progress);

            /* If we don't do this the directory deletion crashes because the handle created in zip.Extract is not released properly? */
            GC.Collect();
            GC.WaitForPendingFinalizers();

            LogInstallation("Cleaning up...\n");
            if (zip is MockZIPFile)
                _fs.DeleteDirectory(zipTmp);
            else
                _fs.DeleteFile(zipTmp);

            _fs.DeleteDirectory(tempDir);
        }

        private void LogInstallation(string msg)
        {
            _installationLog.Append(msg);
            NotifyPropertyChange("InstallationLog");
        }

        private void LogLineInstallation(string msg)
        {
            LogInstallation(msg + '\n');
        }

        private void SetProgress(ProgressState name, int p)
        {
            _progresses[name] = p;
            Progress = _progresses.Sum(x => x.Value);
        }
    }
}
