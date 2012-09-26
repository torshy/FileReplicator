using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace TRock.FileReplicator.Views.Settings
{
    public class SettingsViewModel : NotificationObject, ISettingsViewModel, INavigationAware
    {
        private IRegionNavigationService _navigationService;

        public SettingsViewModel()
        {
            BackCommand = new DelegateCommand(ExecuteNavigateBack);
        }

        private void ExecuteNavigateBack()
        {
            if (_navigationService != null)
            {
                _navigationService.Journal.GoBack();
            }
        }

        public ICommand BackCommand
        {
            get; 
            private set;
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}