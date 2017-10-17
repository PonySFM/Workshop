using CoreLib.Interface;

namespace CoreLib
{
    public class RevisionFileEntry
    {
        public string Path { get; set; }
        public string Sha512 { get; set; }

        public RevisionFileEntry(string path, string sha256)
        {
            Path = path;
            Sha512 = sha256;
        }

        public static RevisionFileEntry FromFile(string path, IFileSystem fs)
        {
            var checksum = fs.GetChecksum(path);
            return new RevisionFileEntry(path, checksum);
        }
    }
}