using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;

namespace PonySFM_Desktop
{
    public class RevisionListItem
    {
        Revision _revision;
        bool _checked;

        public int ID
        {
            get
            {
                return _revision.ID;
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

        public RevisionListItem(Revision revision)
        {
            _revision = revision;
        }
    }

    public class MainWindowPresenter : BasePresenter
    {
        List<RevisionListItem> _items = new List<RevisionListItem>();
        RevisionDatabase _db;

        public List<RevisionListItem> InstalledRevisions
        {
            get
            {
                return _items;
            }
        }

        public MainWindowPresenter(RevisionDatabase db)
        {
            _db = db;
            PopulateListData();
        }

        private void PopulateListData()
        {
            foreach (var revision in _db.Revisions)
            {
                _items.Add(new RevisionListItem(revision));
            }
        }
    }
}
