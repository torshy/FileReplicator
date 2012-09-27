using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using TRock.FileReplicator.Commands;
using TRock.FileReplicator.Services;
using TRock.FileReplicator.ViewModels;
using TRock.FileReplicator.Views.Fileset;
using TRock.FileReplicator.Views.Welcome;

namespace TRock.FileReplicator.Views.Filesets
{
    public class FilesetListViewModel : NotificationObject, IFilesetListViewModel, INavigationAware
    {
        #region Fields

        private readonly ObservableCollection<FilesetViewModel> _filesets;
        private readonly IFilesetService _filesetService;
        private readonly IFileReplicationService _replicationService;

        private IDisposable _filesetAdded;
        private IDisposable _filesetRemoved;
        private ICollectionView _filesetsIcv;
        private IRegionNavigationService _navigationService;

        #endregion Fields

        #region Constructors

        public FilesetListViewModel(IFilesetService filesetService, IFileReplicationService replicationService)
        {
            _filesetService = filesetService;
            _replicationService = replicationService;
            _filesets = new ObservableCollection<FilesetViewModel>();

            AddFilesetCommand = new DelegateCommand(ExecuteAddFileset);
            RemoveFilesetCommand = new AutomaticCommand<FilesetViewModel>(ExecuteRemoveFileset, CanExecuteRemoveFileset);
            SaveAllFilesetsCommand = new DelegateCommand(ExecuteSaveAllFilesets);
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
                                new BitmapImage(new Uri("pack://application:,,,/Resources/light/appbar.layer.png"))
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

        private void OnFilesetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Category" && Filesets != null)
            {
                Filesets.Refresh();
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