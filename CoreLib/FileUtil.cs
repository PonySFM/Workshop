using System;
using System.IO;
using System.Security.Cryptography;

namespace CoreLib
{
    public class FileUtil
    {
        public static string GetChecksum(Stream stream)
        {
            var sha = new SHA256Managed();
            var checksum = sha.ComputeHash(stream);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }
    }
}
