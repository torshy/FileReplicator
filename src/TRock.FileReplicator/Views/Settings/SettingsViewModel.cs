using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;
using System.Linq;

namespace TRock.FileReplicator.Views.Settings
{
    public class SettingsViewModel : NotificationObject, ISettingsViewModel, INavigationAware
    {
        #region Fields

        private readonly string AppName = "File Replicator";
        private readonly RegistryKey _autostartKey;

        private IRegionNavigationService _navigationService;
        private string _applicationVersion;
        private string _activeAccent;
        private Theme _activeTheme;

        #endregion Fields

        #region Constructors

        public SettingsViewModel()
        {
            _autostartKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            _activeAccent = "Blue";

            BackCommand = new DelegateCommand(ExecuteNavigateBack);
            ApplicationVersion = GetInformationalVersion(Assembly.GetExecutingAssembly());
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
                        _autostartKey.SetValue(AppName, "\"" + filePath + "\" /hidden");
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

        public string ApplicationVersion
        {
            get { return _applicationVersion; }
            set
            {
                if (value == _applicationVersion) return;
                _applicationVersion = value;
                RaisePropertyChanged("ApplicationVersion");
            }
        }

        public Theme ActiveTheme
        {
            get { return _activeTheme; }
            set
            {
                if (value == _activeTheme) return;
                _activeTheme = value;
                RaisePropertyChanged("ActiveTheme");

                var accent = ThemeManager.DefaultAccents.FirstOrDefault(a => a.Name == _activeAccent);

                if (accent != null)
                {
                    ThemeManager.ChangeTheme(Application.Current, accent, ActiveTheme);
                }
            }
        }

        public string ActiveAccent
        {
            get { return _activeAccent; }
            set
            {
                if (value == _activeAccent) return;
                _activeAccent = value;
                RaisePropertyChanged("ActiveAccent");

                var accent = ThemeManager.DefaultAccents.FirstOrDefault(a => a.Name == _activeAccent);

                if (accent != null)
                {
                    ThemeManager.ChangeTheme(Application.Current, accent, ActiveTheme);
                }
            }
        }

        public ICollectionView Accents
        {
            get
            {
                return CollectionViewSource.GetDefaultView(ThemeManager.DefaultAccents);
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

        public string GetInformationalVersion(Assembly assembly)
        {
            return FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
        }


        #endregion Methods
    }
}