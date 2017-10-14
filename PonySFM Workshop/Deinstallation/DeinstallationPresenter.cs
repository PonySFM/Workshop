using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLib;
using PonySFM_Workshop.Base;

namespace PonySFM_Workshop.Deinstallation
{
    public class DeinstallationPresenter : BasePresenter
    {
        private readonly RevisionManager _revisionManager;
        private readonly List<int> _uninstallList;
        private readonly Dictionary<int, int> _uninstallProgress = new Dictionary<int, int>();
        int _progress;
        private string _installationLog;
        private string _currentStatus;

        public int MaxProgress => 100;

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
                var progress = new Progress<int>(i => SetProgress(id, i));

                LogInstallation($"Uninstalling revision {id}\n");
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
