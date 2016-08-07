using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PonySFM_Workshop
{
    interface IPresenter
    {
        FrameworkElement View { get; set; }
    }
}
