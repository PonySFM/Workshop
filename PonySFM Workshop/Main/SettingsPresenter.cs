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

        public SettingsPresenter(ConfigHandler config)
        {
            _config = config;
            _file = config.Read();
        }
    }
}
