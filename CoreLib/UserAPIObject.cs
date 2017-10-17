using System.Diagnostics.CodeAnalysis;

namespace CoreLib
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class UserApiObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
