using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class DeinstallationPresenter : BasePresenter
    {
        private RevisionManager _revisionManager;
        private List<int> _uninstallList;
        private Dictionary<int, int> _uninstallProgress = new Dictionary<int, int>();

        public string InstallationLog { get; private set; }

        public int Progress { get { return _uninstallProgress.Sum(x => x.Value); } }
        public int MaxProgress { get { return _uninstallProgress.Count * 100; } }

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
                Progress<int> progress = new Progress<int>(i => _uninstallProgress[id] = i);

                LogInstallation(string.Format("Uninstalling revision {0}\n", id));
                await _revisionManager.UninstallRevision(id, progress);
            }
        }

        private void LogInstallation(string msg)
        {
            InstallationLog += msg;
        }
    }
}
