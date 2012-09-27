using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using TRock.Extensions;
using TRock.FileReplicator.Commands;
using TRock.FileReplicator.Models;
using TRock.FileReplicator.Services;
using TRock.FileReplicator.ViewModels;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace TRock.FileReplicator.Views.Fileset
{
    public class FilesetViewViewModel : NotificationObject, IFilesetViewViewModel, INavigationAware
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly IFileReplicationService _fileReplicationService;
        private readonly IFilesetService _filesetService;

        private IEnumerable<ActivityLogEvent> _acivityLog;
        private FilesetViewModel _fileset;
        private ObservableCollection<string> _categories;

        #endregion Fields

        #region Constructors

        public FilesetViewViewModel(
            IFilesetService filesetService,
            IFileReplicationService fileReplicationService,
            IActivityLogService activityLogService)
        {
            _filesetService = filesetService;
            _fileReplicationService = fileReplicationService;
            _activityLogService = activityLogService;
            _categories = new ObservableCollection<string>();

            Categories = CollectionViewSource.GetDefaultView(_categories);

            BrowseDestinationFolderCommand = new AutomaticCommand(ExecuteBrowseDestinationFolder, CanExecuteBrowseDestinationFolder);
            BrowseSourceFolderCommand = new AutomaticCommand(ExecuteBrowseSourceFolder, CanExecuteBrowseSourceFolder);

            AddIncludeFileCommand = new AutomaticCommand(ExecuteAddIncludeFile, CanExecuteAddIncludeFile);
            RemoveIncludeFileCommand = new AutomaticCommand<FilesetItem>(ExecuteRemoveIncludeFile, CanExecuteRemoveIncludeFile);

            AddExcludeFileCommand = new AutomaticCommand(ExecuteAddExcludeFile, CanExecuteAddExcludeFile);
            RemoveExcludeFileCommand = new AutomaticCommand<FilesetItem>(ExecuteRemoveExcludeFile, CanExecuteRemoveExcludeFile);

            ManualCopyCommand = new DelegateCommand(ExecuteManualCopy);
            ClearActivityLogCommand = new DelegateCommand(ExecuteClearActiviyLog);
        }

        #endregion Constructors

        #region Properties

        public FilesetViewModel Fileset
        {
            get { return _fileset; }
            private set
            {
                _fileset = value;
                RaisePropertyChanged("Fileset");
            }
        }

        public IEnumerable<ActivityLogEvent> ActivityLog
        {
            get { return _acivityLog; }
            private set
            {
                _acivityLog = value;
                RaisePropertyChanged("ActivityLog");
            }
        }

        public ICollectionView Categories
        {
            get; 
            private set;
        }

        public ICommand ManualCopyCommand
        {
            get; private set;
        }

        public ICommand BrowseDestinationFolderCommand
        {
            get; private set;
        }

        public ICommand BrowseSourceFolderCommand
        {
            get; private set;
        }

        public ICommand AddIncludeFileCommand
        {
            get; private set;
        }

        public ICommand RemoveIncludeFileCommand
        {
            get; private set;
        }

        public ICommand AddExcludeFileCommand
        {
            get; private set;
        }

        public ICommand RemoveExcludeFileCommand
        {
            get; private set;
        }

        public ICommand ClearActivityLogCommand
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"];

            if (!string.IsNullOrEmpty(id))
            {
                var fileset = _filesetService.Filesets.FirstOrDefault(fs => fs.Id.ToString() == id);

                if (fileset != null)
                {
                    Fileset = new FilesetViewModel(fileset);
                    ActivityLog = _activityLogService.GetEvents(fileset.Id);
                }

                _filesetService.Filesets.Select(fs => fs.Category)
                    .Distinct()
                    .Where(c => !string.IsNullOrEmpty(c))
                    .ForEach(_categories.Add);
            }
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void ExecuteClearActiviyLog()
        {
            if (Fileset != null)
            {
                _activityLogService.ClearLog(Fileset.Id);
            }
        }

        private void ExecuteManualCopy()
        {
            if (Fileset != null)
            {
                var fs = _filesetService.Filesets.FirstOrDefault(f => f.Id == Fileset.Id);

                if (fs != null)
                {
                    _fileReplicationService.Execute(fs);
                }
            }
        }

        private bool CanExecuteRemoveExcludeFile(FilesetItem item)
        {
            return item != null && Fileset != null;
        }

        private void ExecuteRemoveExcludeFile(FilesetItem item)
        {
            Fileset.RemoveExclude(item);
        }

        private bool CanExecuteAddExcludeFile()
        {
            return Fileset != null && !string.IsNullOrEmpty(Fileset.SourcePath);
        }

        private void ExecuteAddExcludeFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Select files to exclude in the fileset";
            dialog.Multiselect = true;
            dialog.InitialDirectory = Fileset.SourcePath;

            if (dialog.ShowDialog() == true)
            {
                dialog.FileNames.ForEach(Fileset.AddExclude);
            }
        }

        private bool CanExecuteRemoveIncludeFile(FilesetItem item)
        {
            return Fileset != null && item != null;
        }

        private void ExecuteRemoveIncludeFile(FilesetItem item)
        {
            Fileset.RemoveInclude(item);
        }

        private bool CanExecuteAddIncludeFile()
        {
            return Fileset != null && !string.IsNullOrEmpty(Fileset.SourcePath);
        }

        private void ExecuteAddIncludeFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Select files to include in the fileset";
            dialog.Multiselect = true;
            dialog.InitialDirectory = Fileset.SourcePath;

            if (dialog.ShowDialog() == true)
            {
                dialog.FileNames.ForEach(Fileset.AddInclude);
            }
        }

        private bool CanExecuteBrowseSourceFolder()
        {
            return true;
        }

        private void ExecuteBrowseSourceFolder()
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Fileset.SourcePath;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Fileset.SourcePath = dialog.SelectedPath;
            }
        }

        private bool CanExecuteBrowseDestinationFolder()
        {
            return Fileset != null;
        }

        private void ExecuteBrowseDestinationFolder()
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Fileset.DestinationPath;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Fileset.DestinationPath = dialog.SelectedPath;
            }
        }

        #endregion Methods
    }
}