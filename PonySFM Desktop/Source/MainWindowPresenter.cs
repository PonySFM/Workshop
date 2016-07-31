using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;

namespace PonySFM_Desktop
{
    public class MainWindowPresenter : BasePresenter
    {
        RevisionDatabase _db;

        public List<Revision> InstalledRevisions
        {
            get
            {
                return _db.Revisions;
            }
        }

        public MainWindowPresenter(RevisionDatabase db)
        {
            _db = db;
        }
    }
}
