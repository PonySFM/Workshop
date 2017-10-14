using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreLib.Interface;

namespace CoreLib
{
    public enum GameinfoHandlerError
    {
        AlreadyAdded,
        FileInvalid,
        Success
    }

    public class GameinfoHandler
    {
        private readonly string _filepath;
        private readonly IFileSystem _fs;

        public static readonly string GameinfoLine = "\t\t\tGame\t\t\t\tponysfm";

        public GameinfoHandler(string filepath, IFileSystem fs)
        {
            _filepath = filepath;
            _fs = fs;
        }

        public void CreateBackup()
        {
            _fs.CopyFile(_filepath, _filepath + ".bak");
        }

        public void RestoreBackup()
        {
            _fs.CopyFile(_filepath + ".bak", _filepath);
        }

        public GameinfoHandlerError Execute()
        {
            var data = Encoding.UTF8.GetString(_fs.ReadFile(_filepath));

            // Fill a List<string> with the lines from the txt file.
            var txtLines = data.Split('\n').ToList();

            if(txtLines.Any(s => s.Contains("ponysfm") && s.Contains("Game")))
            {
                return GameinfoHandlerError.AlreadyAdded;
            }

            var lineInserted = false;

            foreach(var str in txtLines)
            {
                if (!str.Contains("SearchPaths")) continue;

                var n = txtLines.IndexOf(str);
                while (!txtLines[n].Contains("}"))
                    n++;
                txtLines.Insert(n, GameinfoLine);
                lineInserted = true;
                break;
            }

            if(!lineInserted)
                return GameinfoHandlerError.FileInvalid;

            var builder = new StringBuilder();

            //Add the lines including the new one.
            foreach (var str in txtLines)
            {
                builder.Append(str + Environment.NewLine);
            }

            /* Only at this point do we actually touch the file */
            _fs.DeleteFile(_filepath);
            _fs.CreateFile(_filepath, Encoding.UTF8.GetBytes(builder.ToString()));

            return GameinfoHandlerError.Success;
        }
    }
}
