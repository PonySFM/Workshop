using System;
using System.IO;
using System.Security.Cryptography;

namespace CoreLib
{
    public static class FileUtil
    {
        public static string GetChecksum(Stream stream)
        {
            var sha = new SHA256Managed();
            var checksum = sha.ComputeHash(stream);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }

        public static string GetHumanReadableFileSize(long size)
        {
            string suffix;
            double readable;
            if (size >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (size >> 50);
            }
            else if (size >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (size >> 40);
            }
            else if (size >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (size >> 30);
            }
            else if (size >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (size >> 20);
            }
            else if (size >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (size >> 10);
            }
            else if (size >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = size;
            }
            else
            {
                return size.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.## ") + suffix;
        }
    }
}
