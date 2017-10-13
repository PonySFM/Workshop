﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CoreLib;
using CoreLib.Impl;
using CoreLib.Interface;

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
        string _currentStatus;
        StringBuilder _installationLog = new StringBuilder();
        Dictionary<ProgressState, int> _progresses = new Dictionary<ProgressState, int>();
        CancellationTokenSource _cancellationSource = new CancellationTokenSource();

        IAPIConnector _api;
        IFileSystem _fs;
        RevisionManager _revisionMgr;
        ProgressState _currentProgressState;
        List<int> _ids;

        public delegate void FileExistsHandler(object sender, DirectoryCopierFileExistsEventArgs e);
        public event FileExistsHandler OnFileExists;

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

        public string CurrentStatus
        {
            get
            {
                return _currentStatus;
            }
            set
            {
                _currentStatus = value;
                NotifyPropertyChange("CurrentStatus");
            }
        }

        public string InstallationLog
        {
            get
            {
                return _installationLog.ToString();
            }
        }

        public CancellationTokenSource CancellationSource
        {
            get
            {
                return _cancellationSource;
            }

            set
            {
                _cancellationSource = value;
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
            IZIPFile zip = null;

            var progress = new Progress<int>(i => SetProgress(_currentProgressState, i));
            _revisionMgr.OnFileExists += (s,e) => OnFileExists(s, e);

            if (!_fs.DirectoryExists(tempDir))
                _fs.CreateDirectory(tempDir);

            LogInstallation("Installing revision " + id + "\n");

            _currentProgressState = ProgressState.Download;
            LogInstallation("Downloading file...\n");

            Action cleanup = delegate ()
            {
                if (zip != null)
                    if (zip is MockZIPFile)
                        _fs.DeleteDirectory(zipTmp);
                    else
                        _fs.DeleteFile(zipTmp);
                _fs.DeleteDirectory(tempDir);
            };

            try
            {
                await _api.DownloadRevisionZIP(id, zipTmp, progress);
            }
            catch(WebException e)
            {
                MessageBox.Show("Failed to download: " + e.Message);
                cleanup();
                return;
            }

            _currentProgressState = ProgressState.Extraction;
            LogInstallation("Extracting zip file...\n");
            zip = _fs.OpenZIP(zipTmp);
            await zip.Extract(tempDir, progress);

            var parser = new TempRevisionParser(tempDir, _fs);
            var modDir = parser.FindModFolder();

            if (string.IsNullOrWhiteSpace(modDir))
            {
                string s = "This mod cannot be installed because it does not have the appropriate file-structure.";
                LogInstallation(s);
                MessageBox.Show(s);
                cleanup();
                return;
            }

            _currentProgressState = ProgressState.Installation;
            LogInstallation("Installing files to SFM...\n");
            Revision tmpRev = Revision.CreateTemporaryRevisionFromFolder(id, modDir, _fs);
            await _api.DownloadRevisionAdditionalInformation(tmpRev);
            await _revisionMgr.InstallRevision(tmpRev, modDir, progress, _cancellationSource.Token);

            /* If we don't do this the directory deletion crashes because the handle created in zip.Extract is not released properly? */
            GC.Collect();
            GC.WaitForPendingFinalizers();

            LogInstallation("Cleaning up...\n");
            cleanup();

            LogInstallation("Done!\n");
        }

        private void LogInstallation(string msg)
        {
            CurrentStatus = msg;
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
