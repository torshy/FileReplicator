using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TRock.FileReplicator.Models
{
    public class Fileset : INotifyPropertyChanged
    {
        #region Fields

        private bool _isEnabled;
        private string _name;
        private string _category;
        private string _sourcePath;
        private string _destinationPath;
        private bool _killLockingProcess;

        #endregion Fields

        #region Constructors

        public Fileset()
        {
            Includes = new ObservableCollection<FilesetItem>();
            Excludes = new ObservableCollection<FilesetItem>();
            OnCopyErrorScripts = new ObservableCollection<Script>();
            OnCopyScripts = new ObservableCollection<Script>();
            OnCopySuccessScripts = new ObservableCollection<Script>();
            OnCopyFinallyScripts = new ObservableCollection<Script>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsEnabledChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        public Guid Id
        {
            get;
            set;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnIsEnabledChanged();
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    RaisePropertyChanged("Category");
                }
            }
        }

        public string SourcePath
        {
            get { return _sourcePath; }
            set
            {
                if (_sourcePath != value)
                {
                    _sourcePath = value;
                    RaisePropertyChanged("SourcePath");
                }
            }
        }

        public string DestinationPath
        {
            get { return _destinationPath; }
            set
            {
                if (_destinationPath != value)
                {
                    _destinationPath = value;
                    RaisePropertyChanged("DestinationPath");
                }
            }
        }

        public bool KillLockingProcess
        {
            get { return _killLockingProcess; }
            set
            {
                if (_killLockingProcess != value)
                {
                    _killLockingProcess = value;
                    RaisePropertyChanged("KillLockingProcess");
                }
            }
        }

        public ObservableCollection<FilesetItem> Includes
        {
            get;
            set;
        }

        public ObservableCollection<FilesetItem> Excludes
        {
            get;
            set;
        }

        public ObservableCollection<Script> OnCopyScripts
        {
            get;
            set;
        }

        public ObservableCollection<Script> OnCopyErrorScripts
        {
            get;
            set;
        }

        public ObservableCollection<Script> OnCopySuccessScripts
        {
            get;
            set;
        }

        public ObservableCollection<Script> OnCopyFinallyScripts
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        protected void OnIsEnabledChanged()
        {
            var handler = IsEnabledChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion Methods
    }
}