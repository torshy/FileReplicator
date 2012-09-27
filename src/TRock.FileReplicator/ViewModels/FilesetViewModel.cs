using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

using Microsoft.Practices.Prism.ViewModel;

using TRock.FileReplicator.Models;

namespace TRock.FileReplicator.ViewModels
{
    public class FilesetViewModel : NotificationObject, IWeakEventListener, IDataErrorInfo
    {
        #region Fields

        private readonly Fileset _fileset;

        #endregion Fields

        #region Constructors

        public FilesetViewModel(Fileset fileset)
        {
            _fileset = fileset;

            PropertyChangedEventManager.AddListener(_fileset, this, string.Empty);

            OnCopyScript = new ScriptViewModel(_fileset.OnCopyScripts) { Title = "On copy-event scripts" };
            OnCopyErrorScript = new ScriptViewModel(_fileset.OnCopyErrorScripts) { Title = "On copy-error-event scripts" };
            OnCopySuccessScript = new ScriptViewModel(_fileset.OnCopySuccessScripts) { Title = "On copy-success-event scripts" };
        }

        #endregion Constructors

        #region Properties

        public ScriptViewModel OnCopySuccessScript
        {
            get;
            private set;
        }

        public ScriptViewModel OnCopyErrorScript
        {
            get;
            private set;
        }

        public ScriptViewModel OnCopyScript
        {
            get;
            private set;
        }

        public Guid Id
        {
            get { return _fileset.Id; }
        }

        public string Name
        {
            get
            {
                return _fileset.Name;
            }
            set
            {
                _fileset.Name = value;
                RaisePropertyChanged("Name");
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _fileset.IsEnabled;
            }
            set
            {
                if (_fileset.IsEnabled != value)
                {
                    _fileset.IsEnabled = value;
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public bool KillLockingProcess
        {
            get
            {
                return _fileset.KillLockingProcess;
            }
            set
            {
                if (_fileset.KillLockingProcess != value)
                {
                    _fileset.KillLockingProcess = value;
                    RaisePropertyChanged("KillLockingProcess");
                }
            }
        }

        public string SourcePath
        {
            get
            {
                return _fileset.SourcePath;
            }
            set
            {
                _fileset.SourcePath = value;
                RaisePropertyChanged("SourcePath");
            }
        }

        public string DestinationPath
        {
            get { return _fileset.DestinationPath; }
            set
            {
                _fileset.DestinationPath = value;
                RaisePropertyChanged("DestinationPath");
            }
        }

        public string Category
        {
            get
            {
                if (string.IsNullOrEmpty(_fileset.Category))
                {
                    return "Filesets";
                }

                return _fileset.Category;
            }
            set
            {
                _fileset.Category = value;
                RaisePropertyChanged("Category");
            }
        }

        public IEnumerable<FilesetItem> Includes
        {
            get { return _fileset.Includes; }
        }

        public IEnumerable<FilesetItem> Excludes
        {
            get { return _fileset.Excludes; }
        }

        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion Properties

        #region Indexers

        public string this[string columnName]
        {
            get
            {
                string result = null;

                if (columnName == "DestinationPath")
                    if (string.IsNullOrEmpty(DestinationPath))
                        result = "Please enter a destination path";
                    else if (!Directory.Exists(DestinationPath))
                        result = "Destination path doesn't exist";

                if (columnName == "SourcePath")
                    if (string.IsNullOrEmpty(SourcePath))
                        result = "Please enter a source path";
                    else if (!Directory.Exists(SourcePath))
                        result = "Source path doesn't exist";

                return result;
            }
        }

        #endregion Indexers

        #region Methods

        public void AddInclude(string fullPath)
        {
            const Int32 maxPath = 260;
            var builder = new StringBuilder(maxPath);
            var result = PathRelativePathTo(
                 builder,
                 SourcePath, FileAttributes.Directory,
                 fullPath, FileAttributes.Normal);

            if (result)
            {
                _fileset.Includes.Add(new FilesetItem { RelativePath = builder.ToString() });
            }
        }

        public void AddExclude(string fullPath)
        {
            const Int32 maxPath = 260;
            var builder = new StringBuilder(maxPath);
            var result = PathRelativePathTo(
                 builder,
                 SourcePath, FileAttributes.Directory,
                 fullPath, FileAttributes.Normal);

            if (result)
            {
                _fileset.Excludes.Add(new FilesetItem { RelativePath = builder.ToString() });
            }
        }

        public void RemoveInclude(FilesetItem item)
        {
            _fileset.Includes.Remove(item);
        }

        public void RemoveExclude(FilesetItem item)
        {
            _fileset.Excludes.Remove(item);
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            var propEventArgs = e as PropertyChangedEventArgs;

            if (propEventArgs != null)
            {
                RaisePropertyChanged(propEventArgs.PropertyName);
            }

            return true;
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathRelativePathTo(
            [Out] StringBuilder pszPath,
            [In] string pszFrom,
            [In] FileAttributes dwAttrFrom,
            [In] string pszTo,
            [In] FileAttributes dwAttrTo
            );

        #endregion Methods
    }
}