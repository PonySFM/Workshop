using System.Windows;

namespace PonySFM_Workshop
{
    interface IPresenter
    {
        FrameworkElement View { get; set; }
    }
}
