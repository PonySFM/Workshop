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
        string _filepath;
        IFileSystem _fs;

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
            List<string> txtLines = new List<string>();
            var data = Encoding.UTF8.GetString(_fs.ReadFile(_filepath));

            //Fill a List<string> with the lines from the txt file.
            foreach (string str in data.Split('\n'))
            {
                txtLines.Add(str);
            }

            if(txtLines.Any(s => s.Contains("ponysfm") && s.Contains("Game")))
            {
                return GameinfoHandlerError.AlreadyAdded;
            }

            bool lineInserted = false;

            foreach(string str in txtLines)
            {
                if(str.Contains("SearchPaths"))
                {
                    int n = txtLines.IndexOf(str);
                    while (!txtLines[n].Contains("}"))
                        n++;
                    txtLines.Insert(n, GameinfoLine);
                    lineInserted = true;
                    break;
                }
            }

            if(!lineInserted)
                return GameinfoHandlerError.FileInvalid;

            StringBuilder builder = new StringBuilder();

            //Add the lines including the new one.
            foreach (string str in txtLines)
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
