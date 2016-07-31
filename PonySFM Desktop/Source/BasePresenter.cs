using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;

namespace PonySFM_Desktop
{
    public class BasePresenter : IPresenter, INotifyPropertyChanged
    {
        protected Control _view;

        public Control View
        {
            get
            {
                return _view;
            }

            set
            {
                _view = value;
                _view.DataContext = this;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
