using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
    internal class NativeMethods
    {
        [DllImport("user32.dll", CharSet=CharSet.Unicode)]
        public static extern int SetWindowText(IntPtr hWnd, string text);
    }
}
