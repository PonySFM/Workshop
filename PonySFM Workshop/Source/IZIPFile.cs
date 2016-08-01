using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
    public interface IZIPFile
    {
        Task Extract(string dir, IProgress<int> progress);
    }
}
