using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

using TRock.FileReplicator.Views.Settings;
using System.Linq;

namespace TRock.FileReplicator
{
    public class ShellViewModel : IShellViewModel
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            OpenSettingsViewCommand = new DelegateCommand(ExecuteOpenSettingsView);
        }

        #endregion Constructors

        #region Properties

        public ICommand OpenSettingsViewCommand
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        private void ExecuteOpenSettingsView()
        {
            var region = _regionManager.Regions[AppRegions.MainRegion];

            if (!region.ActiveViews.Any(v => v is ISettingsView))
            {
                region.RequestNavigate(typeof (ISettingsView).Name);
            }
        }

        #endregion Methods
    }
}