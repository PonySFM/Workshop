using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Desktop
{
    public class ResourceAPIObject
    {
        public int id;
        public string name;
        public string resource_type;
        public string url;
        public string description;
        public int user_id;
        // Nuke: thanks to my Rails-skills this bool may actually be null and therefore cause an error when parsing
        // public bool featured;
        public string slug;
        public int score;
        public string content_type;
    }
}
