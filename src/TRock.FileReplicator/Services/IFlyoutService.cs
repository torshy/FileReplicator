using System;
using System.Windows.Threading;

using MahApps.Metro.Controls;

namespace TRock.FileReplicator.Services
{
    public interface IFlyoutService
    {
        #region Methods

        void RegisterFlyout(Flyout flyout);

        void UnregisterFlyout(Flyout flyout);

        #endregion Methods
    }
}