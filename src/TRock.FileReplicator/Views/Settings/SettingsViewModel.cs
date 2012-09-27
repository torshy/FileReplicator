using System;
using System.Diagnostics;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;

namespace TRock.FileReplicator.Views.Settings
{
    public class SettingsViewModel : NotificationObject, ISettingsViewModel, INavigationAware
    {
        #region Fields

        private readonly string AppName = "File Replicator";
        private readonly RegistryKey _autostartKey;

        private IRegionNavigationService _navigationService;

        #endregion Fields

        #region Constructors

        public SettingsViewModel()
        {
            BackCommand = new DelegateCommand(ExecuteNavigateBack);
            _autostartKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        }

        #endregion Constructors

        #region Properties

        public ICommand BackCommand
        {
            get;
            private set;
        }

        public bool IsAutoStartupEnabled
        {
            get
            {

                try
                {
                    return _autostartKey.GetValue(AppName) != null;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                return false;
            }
            set
            {
                try
                {
                    if (value)
                    {
                        var filePath = Process.GetCurrentProcess().MainModule.FileName;
                        _autostartKey.SetValue(AppName, "\"" + filePath + "\"");
                    }
                    else
                    {
                        _autostartKey.DeleteValue(AppName, false);
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                RaisePropertyChanged("IsAutoStartupEnabled");
            }
        }

        #endregion Properties

        #region Methods

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

        private void ExecuteNavigateBack()
        {
            if (_navigationService != null)
            {
                _navigationService.Journal.GoBack();
            }
        }

        #endregion Methods
    }
}