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

        public string SfmDirectory
        {
            get
            {
                return _file.SfmDirectoryPath;
            }
            set
            {
                _file.SfmDirectoryPath = value;
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
            var parser = new SfmDirectoryParser(SfmDirectory, WindowsFileSystem.Instance);
            var error = parser.Validate();

            switch (error)
            {
                case SfmDirectoryParserError.NotExists:
                    SaveError = "SFM Directory does not exist.";
                    Reset();
                    return false;
                case SfmDirectoryParserError.NotLikely:
                    SaveError = "SFM Directory is not valid.";
                    Reset();
                    return false;
                case SfmDirectoryParserError.Ok:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _file.SfmDirectoryPath = parser.Path;

            _config.Write(_file);
            MainWindow.Instance.RefreshListData();

            return true;
        }
    }
}
