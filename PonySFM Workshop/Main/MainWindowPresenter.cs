using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CoreLib;
using CoreLib.Impl;
using PonySFM_Workshop.Base;

namespace PonySFM_Workshop.Main
{
    public class MainWindowPresenter : BasePresenter
    {
        private readonly ObservableCollection<RevisionListItem> _items = new ObservableCollection<RevisionListItem>();
        private readonly RevisionDatabase _db;
        private readonly RevisionManager _revisionManager;

        public ObservableCollection<RevisionListItem> InstalledRevisions => _items;

        public MainWindowPresenter(RevisionManager revisionManager)
        {
            _revisionManager = revisionManager;
            _db = _revisionManager.Database;
            FixMissingInfo();
            PopulateListData();
        }

        public void OnUninstall()
        {
            var toUninstall = _items.Where(x => x.Checked);
            var ids = toUninstall.Select(rev => rev.ID).ToList();

            if (ids.Count == 0) return;

            var w = new Deinstallation.DeinstallationWindow(_revisionManager, ids, true);
            w.ShowDialog();
            PopulateListData();
        }

        public void OnVerify()
        {
            var toVerify = _items.Where(x => x.Checked);
            var ids = toVerify.Select(rev => rev.ID).ToList();

            if (ids.Count == 0) return;

            var w = new Verification.VerificationWindow(ids, _revisionManager);
            w.ShowDialog();
            PopulateListData();
        }

        private void FixMissingInfo()
        {
            foreach (var revision in _db.Revisions)
            {
                if (revision.MissingMetadata())
                {
                    Task.Run(async () => { await PonySFMAPIConnector.Instance.FetchMetadata(revision); } ).Wait();
                }
            }
            _revisionManager.Database.WriteDbDisk();
            NotifyPropertyChange("InstalledRevisions");
        }

        public void PopulateListData()
        {
            /* HACK: Have to force refresh here because the instances here and the one used in App.StartInstallation are actually different */
            _db.RefreshDataDisk();
            _items.Clear();
            foreach (var revision in _db.Revisions)
            {
                _items.Add(new RevisionListItem(revision));
            }
            NotifyPropertyChange("InstalledRevisions");
        }

        public void CheckAll(bool check)
        {
            foreach (var item in _items)
            {
                item.Checked = check;
            }
        }
    }
}
