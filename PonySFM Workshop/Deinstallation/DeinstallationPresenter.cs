﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLib;

namespace PonySFM_Workshop
{
    public class DeinstallationPresenter : BasePresenter
    {
        RevisionManager _revisionManager;
        List<int> _uninstallList;
        Dictionary<int, int> _uninstallProgress = new Dictionary<int, int>();
        int _progress;
        string _installationLog;
        string _currentStatus;

        public int MaxProgress { get { return 100; } }

        public int Progress
        {
            get
            {
                return _progress;
            }

            set
            {
                _progress = value;
                NotifyPropertyChange("Change");
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
                return _installationLog;
            }

            set
            {
                _installationLog = value;
                NotifyPropertyChange("InstallationLog");
            }
        }

        public DeinstallationPresenter(RevisionManager revisionManager, List<int> uninstallList)
        {
            _revisionManager = revisionManager;
            _uninstallList = uninstallList;
        }

        public async Task Execute()
        {
            foreach (var id in _uninstallList)
            {
                _uninstallProgress[id] = 0;
            }

            foreach (var id in _uninstallList)
            {
                Progress<int> progress = new Progress<int>(i => SetProgress(id, i));

                LogInstallation(string.Format("Uninstalling revision {0}\n", id));
                await _revisionManager.UninstallRevision(id, progress);

                NotifyPropertyChange("Progress");
            }
        }

        private void LogInstallation(string msg)
        {
            CurrentStatus = msg;
            InstallationLog += msg;
        }

        private void SetProgress(int id, int p)
        {
            _uninstallProgress[id] = p;
            Progress = _uninstallProgress.Sum(x => x.Value);
        }
    }
}
