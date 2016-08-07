using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;

namespace PonySFM_Workshop
{
    public class BasePresenter : IPresenter, INotifyPropertyChanged
    {
        protected FrameworkElement _view;

        public FrameworkElement View
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
