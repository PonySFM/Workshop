using System.ComponentModel;
using System.Windows;

namespace PonySFM_Workshop.Base
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
