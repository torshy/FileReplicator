using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Win32;

using TRock.FileReplicator.Commands;
using TRock.FileReplicator.Core;
using TRock.FileReplicator.Services;
using TRock.FileReplicator.ViewModels;
using TRock.FileReplicator.Views.Fileset;
using TRock.FileReplicator.Views.Welcome;
using TRock.Extensions;

namespace TRock.FileReplicator.Views.Filesets
{
    public class FilesetListViewModel : NotificationObject, IFilesetListViewModel, INavigationAware
    {
        #region Fields

        private readonly ObservableCollection<FilesetViewModel> _filesets;
        private readonly Dispatcher _dispatcher;
        private readonly IFilesetService _filesetService;
        private readonly IFileReplicationService _replicationService;

        private IDisposable _filesetAdded;
        private IDisposable _filesetRemoved;
        private ICollectionView _filesetsIcv;
        private IRegionNavigationService _navigationService;

        #endregion Fields

        #region Constructors

        public FilesetListViewModel(
            Dispatcher dispatcher,
            IFilesetService filesetService, 
            IFileReplicationService replicationService)
        {
            _dispatcher = dispatcher;
            _filesetService = filesetService;
            _replicationService = replicationService;
            _filesets = new ObservableCollection<FilesetViewModel>();

            AddFilesetCommand = new DelegateCommand(ExecuteAddFileset);
            RemoveFilesetCommand = new AutomaticCommand<FilesetViewModel>(ExecuteRemoveFileset, CanExecuteRemoveFileset);
            SaveAllFilesetsCommand = new DelegateCommand(ExecuteSaveAllFilesets);
            ExportFilesetCommand = new AutomaticCommand<FilesetViewModel>(ExecuteExportFileset, CanExecuteExportFileset);
            ImportFilesetCommand = new DelegateCommand(ExecuteImportFileset);
            CommandBar = new CommandBar();
        }

        #endregion Constructors

        #region Properties

        public ICollectionView Filesets
        {
            get
            {
                return _filesetsIcv;
            }
            private set
            {
                if (_filesetsIcv != value)
                {
                    _filesetsIcv = value;
                    RaisePropertyChanged("Filesets");
                }
            }
        }

        public ICommandBar CommandBar
        {
            get;
            private set;
        }

        public DelegateCommand AddFilesetCommand
        {
            get;
            private set;
        }

        public AutomaticCommand<FilesetViewModel> RemoveFilesetCommand
        {
            get;
            private set;
        }

        public DelegateCommand SaveAllFilesetsCommand
        {
            get;
            private set;
        }

        public AutomaticCommand<FilesetViewModel> ExportFilesetCommand
        {
            get;
            private set;
        }

        public DelegateCommand ImportFilesetCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public void RefreshCommandBar(IEnumerable<FilesetViewModel> selectedFilesets)
        {
            CommandBar.Clear();

            var filesetViewModels = selectedFilesets as FilesetViewModel[] ?? selectedFilesets.ToArray();

            if (filesetViewModels.Any())
            {
                var filesetViewModel = filesetViewModels.First();

                CommandBar
                    .AddCommand(new CommandModel
                    {
                        Content = "Clone",
                        Command = new DelegateCommand<IEnumerable<FilesetViewModel>>(ExecuteCloneFilesets),
                        CommandParameter = selectedFilesets,
                        Icon = new Image
                        {
                            Source =
                                new BitmapImage(new Uri("pack://application:,,,/Resources/light/appbar.list.two.png"))
                        }
                    })
                    .AddCommand(new CommandModel
                    {
                        Content = filesetViewModel.IsEnabled ? "Deactivate" : "Activate",
                        Command = new DelegateCommand<FilesetViewModel>(model => model.IsEnabled = !model.IsEnabled),
                        CommandParameter = filesetViewModel,
                        Icon = new Image
                        {
                            Source = filesetViewModel.IsEnabled
                                ? new BitmapImage(new Uri("pack://application:,,,/Resources/light/appbar.eye.close.png"))
                                : new BitmapImage(new Uri("pack://application:,,,/Resources/light/appbar.eye.check.png"))
                        }
                    });

                if (filesetViewModel.Includes.Any() || filesetViewModel.Excludes.Any())
                {
                    CommandBar.AddCommand(new CommandModel
                    {
                        Content = "Execute file copy",
                        Command = new DelegateCommand<FilesetViewModel>(model =>
                        {
                            var fs = _filesetService.Filesets.FirstOrDefault(f => f.Id == model.Id);

                            if (fs != null)
                            {
                                _replicationService.Execute(fs);
                            }
                        }),
                        CommandParameter = filesetViewModel,
                        Icon = new Image
                        {
                            Source =
                                new BitmapImage(new Uri("pack://application:,,,/Resources/light/appbar.diagram.png"))
                        }
                    });
                }
            }
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;

            foreach (var fileset in _filesetService.Filesets)
            {
                var viewModel = new FilesetViewModel(fileset);
                viewModel.PropertyChanged += OnFilesetPropertyChanged;
                _filesets.Add(viewModel);
            }

            _filesetAdded = _filesetService
                .FilesetAdded
                .ObserveOnDispatcher()
                .Subscribe(fileset =>
                {
                    var viewModel = new FilesetViewModel(fileset);
                    viewModel.PropertyChanged += OnFilesetPropertyChanged;
                    _filesets.Add(viewModel);
                    Filesets.MoveCurrentTo(viewModel);
                });

            _filesetRemoved = _filesetService
                .FilesetRemoved
                .ObserveOnDispatcher()
                .Subscribe(fileset =>
                {
                    var fs = _filesets.FirstOrDefault(fvm => fvm.Id == fileset.Id);

                    if (fs != null)
                    {
                        fs.PropertyChanged -= OnFilesetPropertyChanged;
                        _filesets.Remove(fs);
                    }

                    if (Filesets.IsEmpty)
                    {
                        _navigationService.Region.RegionManager.Regions[AppRegions.MainRegion]
                            .RequestNavigate(typeof(IWelcomeView).Name);
                    }
                });

            Filesets = CollectionViewSource.GetDefaultView(_filesets);
            Filesets.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            Filesets.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));
            Filesets.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            Filesets.CurrentChanged += CurrentFilesetChanged;
            Filesets.MoveCurrentTo(null);
            Filesets.MoveCurrentToFirst();
        }

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            if (_filesetAdded != null)
            {
                _filesetAdded.Dispose();
            }

            if (_filesetRemoved != null)
            {
                _filesetRemoved.Dispose();
            }
        }

        private void ExecuteImportFileset()
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "Select filesets to import";
            dialog.Multiselect = true;
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Filter = "Fileset (*.fis)|*.fis";

            if (dialog.ShowDialog() == true)
            {
                dialog.FileNames.ForEach(file =>
                {
                    _filesetService
                        .Add(() => new FileStream(file, FileMode.Open, FileAccess.Read))
                        .ContinueWith(task =>
                        {
                            Trace.WriteLineIf(task.IsFaulted, task.Exception);
                        });
                });
            }
        }

        private void ExecuteExportFileset(FilesetViewModel fileset)
        {
            var dialog = new SaveFileDialog();
            dialog.Title = "Select location to export the fileset";
            dialog.FileName = fileset.Id + ".fis";

            if (dialog.ShowDialog() == true)
            {
                var fs = _filesetService.Filesets.FirstOrDefault(fvm => fvm.Id == fileset.Id);

                if (fs != null)
                {
                    _filesetService
                        .Save(fs, () => new FileStream(dialog.FileName, FileMode.Create))
                        .ContinueWith(task =>
                        {
                            Trace.WriteLineIf(task.IsFaulted, task);
                        });
                }
            }
        }

        private bool CanExecuteExportFileset(FilesetViewModel fileset)
        {
            return fileset != null;
        }

        private void OnFilesetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Category" && Filesets != null)
            {
                _dispatcher.BeginInvoke(new Action(Filesets.Refresh));
            }
        }

        private void ExecuteCloneFilesets(IEnumerable<FilesetViewModel> filesets)
        {
            foreach (var filesetViewModel in filesets.ToArray())
            {
                FilesetViewModel model = filesetViewModel;

                _filesetService.Add().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Trace.WriteLine(task.Exception);
                    }
                    else
                    {
                        var clone = task.Result;
                        clone.Name = model.Name + " [Clone]";
                        clone.Category = model.Category;
                        clone.Includes.AddRange(model.Includes);
                        clone.Excludes.AddRange(model.Excludes);
                        clone.KillLockingProcess = model.KillLockingProcess;
                        clone.DestinationPath = model.DestinationPath;
                        clone.SourcePath = model.SourcePath;
                        clone.IsEnabled = false;
                    }
                });
            }
        }

        private void ExecuteSaveAllFilesets()
        {
            _filesetService
                .Save()
                .ContinueWith(task =>
                {
                    Trace.WriteLineIf(task.IsFaulted, task.Exception);
                });
        }

        private bool CanExecuteRemoveFileset(FilesetViewModel model)
        {
            return model != null;
        }

        private void ExecuteRemoveFileset(FilesetViewModel model)
        {
            var fs = _filesetService.Filesets.FirstOrDefault(f => f.Id == model.Id);

            if (fs != null)
            {
                _filesetService
                    .Remove(fs)
                    .ContinueWith(task =>
                    {
                        Trace.WriteLineIf(task.IsFaulted, task.Exception);
                    });
            }
        }

        private void ExecuteAddFileset()
        {
            _filesetService
                .Add()
                .ContinueWith(task =>
                {
                    Trace.WriteLineIf(task.IsFaulted, task.Exception);
                });
        }

        private void CurrentFilesetChanged(object sender, EventArgs e)
        {
            var selected = Filesets.CurrentItem as FilesetViewModel;

            if (selected != null)
            {
                var query = new UriQuery();
                query.Add("id", selected.Id.ToString());
                _navigationService.Region.RegionManager.Regions[AppRegions.MainRegion]
                    .RequestNavigate(typeof(IFilesetView).Name + query);
            }
        }

        #endregion Methods
    }
}