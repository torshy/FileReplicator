using MahApps.Metro.Controls;

namespace TRock.FileReplicator.Core
{
    public interface IFlyoutService
    {
        #region Methods

        void RegisterFlyout(Flyout flyout);

        void UnregisterFlyout(Flyout flyout);

        #endregion Methods
    }
}