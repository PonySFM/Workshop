using CoreLib;
using PonySFM_Workshop.Base;

namespace PonySFM_Workshop.Main
{
    public class RevisionListItem : BaseNotifyPropertyChanged
    {
        private bool _checked;

        public int ID => Revision.ID;

        public bool Checked
        {
            get
            {
                return _checked;
            }

            set
            {
                _checked = value;
                NotifyPropertyChange("Checked");
            }
        }

        public Revision Revision { get; }
        public string UserName => Revision.GetMetadataValue("UserName");
        public string ResourceName => Revision.GetMetadataValue("ResourceName");
        public string InstallationTime => Revision.GetMetadataValue("InstallationTime");

        public RevisionListItem(Revision revision)
        {
            Revision = revision;
        }
    }
}