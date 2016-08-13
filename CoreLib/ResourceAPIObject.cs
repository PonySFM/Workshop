namespace CoreLib
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
