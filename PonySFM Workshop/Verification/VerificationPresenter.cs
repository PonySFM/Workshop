using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLib;

namespace PonySFM_Workshop
{
    public class VerificationPresenter : BasePresenter
    {
        private readonly RevisionManager _revisionManager;
        private readonly List<int> _ids;
        private int _progress;
        private readonly Dictionary<int, int> _uninstallProgress = new Dictionary<int, int>();
        private string _installationLog;
        private string _currentStatus;

        public int MaxProgress => _uninstallProgress.Count * 100;

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
                return _installationLog;
            }

            set
            {
                _installationLog = value;
                NotifyPropertyChange("InstallationLog");
            }
        }

        public VerificationPresenter(RevisionManager revisionManager, List<int> ids)
        {
            _revisionManager = revisionManager;
            _ids = ids;
            foreach (var id in _ids)
            {
                _uninstallProgress[id] = 0;
            }
        }

        public async Task<List<int>> Execute()
        {
            var failedIDs = new List<int>();
            foreach (var id in _ids)
            {
                var progress = new Progress<int>(i => SetProgress(id, i));
                var valid = false;

                LogInstallation("Verifying revision " + id + "\n");
                await Task.Factory.StartNew(() =>
                {
                    valid = _revisionManager.VerifyInstalled(id, progress);
                });

                if (!valid)
                {
                    failedIDs.Add(id);
                }
            }

            if(failedIDs.Count > 0)
            {
                LogInstallation("Some revisions failed to validate:\n");
                foreach (var failedID in failedIDs)
                {
                    LogInstallation("    " + failedID + "\n");
                }
            }
            else
            {
                LogInstallation("Ok!\n");
            }

            Progress =  _uninstallProgress.Count * 100;

            return failedIDs;
        }

        private void LogInstallation(string msg)
        {
            CurrentStatus = msg;
            InstallationLog += msg;
        }

        private void SetProgress(int id, int progress)
        {
            _uninstallProgress[id] = progress;
            Progress = _uninstallProgress.Sum(x => x.Value);
        }
    }
}
