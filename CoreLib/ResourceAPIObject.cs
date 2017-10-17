using System.Diagnostics.CodeAnalysis;

namespace CoreLib
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ResourceApiObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ResourceType { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public bool Featured { get; set; }
        public string Slug { get; set; }
        public int Score { get; set; }
        public string ContentType { get; set; }

        public bool HasUser()
        {
            return UserId != 0;
        }
    }
}
