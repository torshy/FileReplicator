using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TRock.FileReplicator.Models
{
    public class Fileset : INotifyPropertyChanged
    {
        #region Fields

        private bool _isEnabled;

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
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string SourcePath
        {
            get;
            set;
        }

        public string DestinationPath
        {
            get;
            set;
        }

        public bool KillLockingProcess
        {
            get;
            set;
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

        protected virtual void OnPropertyChanged(string propertyName)
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