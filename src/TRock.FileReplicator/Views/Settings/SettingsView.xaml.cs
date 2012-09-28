using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace TRock.FileReplicator.Views.Settings
{
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class SettingsView : UserControl, ISettingsView
    {
        public SettingsView(ISettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
