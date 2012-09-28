using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

using TRock.FileReplicator.Views.Settings;

namespace TRock.FileReplicator
{
    public class ShellViewModel : IShellViewModel
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public ShellViewModel(IRegionManager regionManager, IEnumerable<Flyout> flyouts)
        {
            Flyouts = new ObservableCollection<Flyout>(flyouts);
            _regionManager = regionManager;

            OpenSettingsViewCommand = new DelegateCommand(ExecuteOpenSettingsView);
        }

        #endregion Constructors

        #region Properties

        public ObservableCollection<Flyout> Flyouts
        {
            get; private set;
        }

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