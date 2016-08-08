using PonySFM_Workshop.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonySFM_Workshop
{
    public class SettingsPresenter : BasePresenter
    {
        ConfigHandler _config;
        ConfigFile _file;

        public string SFMDirectory
        {
            get
            {
                return _file.SFMDirectoryPath;
            }
            set
            {
                _file.SFMDirectoryPath = value;
                NotifyPropertyChange("SFMDirectory");
            }
        }

        public string SaveError { get; private set; }

        public SettingsPresenter(ConfigHandler config)
        {
            _config = config;
            Reset();
        }

        public void Reset()
        {
            _file = _config.Read();
            NotifyPropertyChange("SFMDirectory");
        }

        public bool Save()
        {
            var parser = new SFMDirectoryParser(SFMDirectory, WindowsFileSystem.Instance);
            var error = parser.Validate();

            if(error == SFMDirectoryParserError.NotExists)
            {
                SaveError = "SFM Directory does not exist.";
                Reset();
                return false;
            }
            else if(error == SFMDirectoryParserError.NotLikely)
            {
                SaveError = "SFM Directory is not valid.";
                Reset();
                return false;
            }


            _config.Write(_file);

            return true;
        }
    }
}
