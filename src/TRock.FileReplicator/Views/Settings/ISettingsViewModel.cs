using System.ComponentModel;

namespace TRock.FileReplicator.Views.Settings
{
    public interface ISettingsViewModel
    {
        #region Properties

        bool IsAutoStartupEnabled
        {
            get; set;
        }

        string ApplicationVersion
        {
            get;
        }

        ICollectionView Accents
        {
            get;
        }

        string ActiveAccent
        {
            get; 
            set;
        }

        #endregion Properties
    }
}