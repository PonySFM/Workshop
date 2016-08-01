﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class VerificationPresenter : BasePresenter
    {
        RevisionManager _revisionManager;
        List<int> _ids;
        int _progress;
        private Dictionary<int, int> _uninstallProgress = new Dictionary<int, int>();
        string _installationLog;

        public int MaxProgress
        {
            get
            {
                return _uninstallProgress.Count * 100;
            }
        }

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

        public VerificationPresenter(RevisionManager revisionManager, List<int> ids)
        {
            _revisionManager = revisionManager;
            _ids = ids;
        }

        public async Task<List<int>> Execute()
        {
            List<int> failedIDs = new List<int>();
            foreach (var id in _ids)
            {
                _uninstallProgress[id] = 0;
            }

            foreach (var id in _ids)
            {
                Progress<int> progress = new Progress<int>(i => _uninstallProgress[id] = i);
                bool v = false;
                LogInstallation("Verifying revision " + id + "\n");
                await Task.Factory.StartNew(() =>
                {
                    v = _revisionManager.VerifyInstalled(id, progress);
                });

                if (!v)
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

            return failedIDs;
        }

        private void LogInstallation(string msg)
        {
            InstallationLog += msg;
        }
    }
}
