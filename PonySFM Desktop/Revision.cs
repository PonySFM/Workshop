using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class Revision
    {
        public int ID { get; set; }
        public List<string> Files { get; set; }

        public Revision(int id, List<string> files)
        {
            ID = id;
            Files = files;
        }
    }
}