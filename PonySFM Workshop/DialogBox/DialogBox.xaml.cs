using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PonySFM_Workshop
{
    public enum DialogBoxResult
    {
        Yes, YesAll, Ok, Cancel
    }

    [Flags]
    public enum EnableButtons
    {
        Yes, YesAll, Ok, Cancel
    }

    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class DialogBox : MetroWindow
    {
        public DialogBox()
        {
            InitializeComponent();
        }

        public DialogBoxResult Result { get; private set; }
    }
}
