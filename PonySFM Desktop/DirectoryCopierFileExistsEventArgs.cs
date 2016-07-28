using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class DirectoryCopierFileExistsEventArgs
    {
        public string File1 { get; private set; }
        public string File2 { get; private set; }

        public bool ShouldCopy { get; set; }

        public DirectoryCopierFileExistsEventArgs(string file1, string file2)
        {
            File1 = file1;
            File2 = file2;
        }
    }
}
