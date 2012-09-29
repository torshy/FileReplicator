using System.Windows;

using MahApps.Metro.Controls;

namespace TRock.FileReplicator.Services
{
    public class FlyoutService : IFlyoutService
    {
        #region Methods

        public void RegisterFlyout(Flyout flyout)
        {
            var mainWindow = Application.Current.MainWindow as MetroWindow;

            if (mainWindow != null)
            {
                mainWindow.Flyouts.Add(flyout);
            }
        }

        public void UnregisterFlyout(Flyout flyout)
        {
            var mainWindow = Application.Current.MainWindow as MetroWindow;

            if (mainWindow != null)
            {
                mainWindow.Flyouts.Remove(flyout);
            }
        }

        #endregion Methods
    }
}