using System;
using System.IO;
using System.Security.Cryptography;

namespace PonySFM_Workshop
{
    public class FileUtil
    {
        public static string GetChecksum(Stream stream)
        {
            var sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(stream);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }
    }
}
