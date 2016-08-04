using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;

namespace PonySFM_Workshop
{
    public class RevisionListItem
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
            }
        }

        public string UserName
        {
            get
            {
                return Revision.AdditionalData["UserName"];
            }
        }

        public string ResourceName
        {
            get
            {
                return Revision.AdditionalData["ResourceName"];
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
        List<RevisionListItem> _items = new List<RevisionListItem>();
        RevisionDatabase _db;
        RevisionManager _revisionManager;

        public List<RevisionListItem> InstalledRevisions
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
                var w = new DeinstallationWindow(_revisionManager, ids);
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

        private void PopulateListData()
        {
            _items.Clear();
            foreach (var revision in _db.Revisions)
            {
                _items.Add(new RevisionListItem(revision));
            }
            NotifyPropertyChange("InstalledRevisions");
        }
    }
}
