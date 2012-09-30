using System.Windows;

using MahApps.Metro.Controls;

using TRock.FileReplicator.Core;

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
                flyout.Loaded += IsOpenBeforeLoadedWorkaround;

                mainWindow.Flyouts.Add(flyout);
            }
        }

        public void UnregisterFlyout(Flyout flyout)
        {
            var mainWindow = Application.Current.MainWindow as MetroWindow;

            if (mainWindow != null)
            {
                flyout.Loaded -= IsOpenBeforeLoadedWorkaround;

                mainWindow.Flyouts.Remove(flyout);
            }
        }

        private void IsOpenBeforeLoadedWorkaround(object sender, RoutedEventArgs e)
        {
            var flyout = ((Flyout)sender);

            if (flyout.IsOpen)
            {
                flyout.IsOpen = false;
                flyout.IsOpen = true;
            }
        }

        #endregion Methods
    }
}