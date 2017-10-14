using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLib;
using CoreLib.Impl;
using System.Collections.ObjectModel;

namespace PonySFM_Workshop
{
    /* TODO: BasePresenter is false here because actually only want NotifyPropertyChange. Refactor? */
    public class RevisionListItem : BasePresenter
    {
        Revision _revision;
        bool _checked;

        public int ID
        {
            get
            {
                return Revision.ID;
            }
        }

        public bool Checked
        {
            get
            {
                return _checked;
            }

            set
            {
                _checked = value;
                NotifyPropertyChange("Checked");
            }
        }

        public string UserName
        {
            get
            {
                return Revision.GetAdditionalData("UserName");
            }
        }

        public string ResourceName
        {
            get
            {
                return Revision.GetAdditionalData("ResourceName");
            }
        }

        public string InstallationTime
        {
            get
            {
                return Revision.GetAdditionalData("InstallationTime");
            }
        }

        public Revision Revision
        {
            get
            {
                return _revision;
            }

            set
            {
                _revision = value;
            }
        }

        public RevisionListItem(Revision revision)
        {
            Revision = revision;
        }
    }

    public class MainWindowPresenter : BasePresenter
    {
        ObservableCollection<RevisionListItem> _items = new ObservableCollection<RevisionListItem>();
        RevisionDatabase _db;
        RevisionManager _revisionManager;

        public ObservableCollection<RevisionListItem> InstalledRevisions
        {
            get
            {
                return _items;
            }
        }

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
            List<int> ids = new List<int>();

            foreach (var rev in toUninstall)
            {
                ids.Add(rev.ID);
            }

            if(ids.Count > 0)
            {
                var w = new DeinstallationWindow(_revisionManager, ids, true);
                w.ShowDialog();
                PopulateListData();
            }
        }

        public void OnVerify()
        {
            var toVerify = _items.Where(x => x.Checked);
            List<int> ids = new List<int>();

            foreach (var rev in toVerify)
            {
                ids.Add(rev.ID);
            }

            if(ids.Count > 0)
            {
                var w = new VerificationWindow(ids, _revisionManager);
                w.ShowDialog();
                PopulateListData();
            }
        }

        private void FixMissingInfo()
        {
            foreach (var revision in _db.Revisions)
            {
                if (revision.MissingAdditionalData())
                {
                    Task.Run(async () => { await PonySFMAPIConnector.Instance.FetchMetadata(revision); } ).Wait();
                }
            }
            _revisionManager.Database.WriteDBDisk();
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
