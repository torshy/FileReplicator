using System.Windows.Input;

namespace TRock.FileReplicator
{
    public interface IShellViewModel
    {
        ICommand OpenSettingsViewCommand { get; }
    }
}