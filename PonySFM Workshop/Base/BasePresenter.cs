using System.Windows;

namespace PonySFM_Workshop.Base
{
    public class BasePresenter : BaseNotifyPropertyChanged, IPresenter
    {
        private FrameworkElement _view;

        public FrameworkElement View
        {
            get => _view;

            set
            {
                _view = value;
                _view.DataContext = this;
            }
        }
    }
}
