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

        #endregion Properties
    }
}