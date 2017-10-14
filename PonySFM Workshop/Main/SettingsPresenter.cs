using System;
using CoreLib;
using CoreLib.Impl;
using PonySFM_Workshop.Base;

namespace PonySFM_Workshop.Main
{
    public class SettingsPresenter : BasePresenter
    {
        private readonly ConfigHandler _config;
        private ConfigFile _file;

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

            switch (error)
            {
                case SFMDirectoryParserError.NotExists:
                    SaveError = "SFM Directory does not exist.";
                    Reset();
                    return false;
                case SFMDirectoryParserError.NotLikely:
                    SaveError = "SFM Directory is not valid.";
                    Reset();
                    return false;
                case SFMDirectoryParserError.OK:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _file.SFMDirectoryPath = parser.Path;

            _config.Write(_file);
            MainWindow.Instance.RefreshListData();

            return true;
        }
    }
}
