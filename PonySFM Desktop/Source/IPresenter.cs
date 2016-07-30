using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PonySFM_Desktop
{
    interface IPresenter
    {
        Control View { get; set; }
    }
}
