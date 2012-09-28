using System.Collections.ObjectModel;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace TRock.FileReplicator
{
    public interface IShellViewModel
    {
        ICommand OpenSettingsViewCommand { get; }

        ObservableCollection<Flyout> Flyouts { get; }
    }
}