using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
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

        public void Execute()
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
                return;
            }

            bool lineInserted = false;

            foreach(string str in txtLines)
            {
                if(str.Contains("SearchPaths"))
                {
                    txtLines.Insert(txtLines.IndexOf(str) + 2, GameinfoLine);
                    lineInserted = true;
                    break;
                }
            }

            if(!lineInserted)
            {
                /*
                Console.WriteLine("Failed to add custom gameinfo.txt entry!");
                Console.WriteLine("How to do it manually:");
                Console.WriteLine("1. Open the gameinfo.txt in the usermod directory");
                Console.WriteLine("2. Search for the line that says 'SearchPaths'");
                Console.WriteLine("3. Add an entry with the foldername 'ponysfm'");
                */
                return;
            }

            StringBuilder builder = new StringBuilder();

            //Add the lines including the new one.
            foreach (string str in txtLines)
            {
                builder.Append(str + Environment.NewLine);
            }

            _fs.DeleteFile(_filepath);
            _fs.CreateFile(_filepath, Encoding.UTF8.GetBytes(builder.ToString()));
        }
    }
}
