using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PonySFM_Desktop
{
    public class InstallationPresenter : IPresenter
    {
        public Control View
        {
            get
            {
                return View;
            }
            set
            {
                View = value;
                View.DataContext = this;
            }
        }

        public int Progress { get; private set; }
        public string Status { get; private set; }

        public async Task ExecuteInstallation(int id)
        {
        }
    }
}
