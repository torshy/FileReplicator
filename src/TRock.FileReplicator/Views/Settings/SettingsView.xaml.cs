using System.Windows.Controls;

namespace TRock.FileReplicator.Views.Settings
{
    public partial class SettingsView : UserControl, ISettingsView
    {
        public SettingsView(ISettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
