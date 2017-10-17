using System.Diagnostics.CodeAnalysis;

namespace CoreLib
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class RevisionApiObject
    {
        public int ID { get; set; }
        public string Changeset { get; set; }
        public int Filesize { get; set; }
        public string Filename { get; set; }
        public int ResourceID { get; set; }
        public int No { get; set; }
        public int MdlCount { get; set; }
        public int VtfCount { get; set; }
        public int VmtCount { get; set; }
        public int BspCount { get; set; }
        public int WavCount { get; set; }
    }
}
