using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Autofac;

using Hardcodet.Wpf.TaskbarNotification;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

using TRock.Extensions;
using TRock.FileReplicator.Controls;
using TRock.FileReplicator.Events;
using TRock.FileReplicator.Models;
using TRock.FileReplicator.Services;

namespace TRock.FileReplicator
{
    public class AppTaskbarIconHandler : IStartable
    {
        #region Fields

        private readonly ICommandBar _commandBar;
        private readonly IEventAggregator _eventAggregator;
        private readonly IFilesetService _filesetService;
        private readonly IFileReplicationService _replicationService;
        private readonly TaskbarIcon _taskbarIcon;

        #endregion Fields

        #region Constructors

        public AppTaskbarIconHandler(
            IFilesetService filesetService,
            IFileReplicationService replicationService,
            IEventAggregator eventAggregator)
        {
            _filesetService = filesetService;
            _replicationService = replicationService;
            _eventAggregator = eventAggregator;
            _commandBar = new CommandBar();
            _taskbarIcon = Application.Current.TryFindResource("App_TaskbarIcon") as TaskbarIcon;
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            _eventAggregator.GetEvent<CopiedEvent>().Subscribe(OnItemCopied, ThreadOption.UIThread, true);
            _eventAggregator.GetEvent<CopyErrorEvent>().Subscribe(OnItemErrorCopy, ThreadOption.UIThread, true);
            _taskbarIcon.DoubleClickCommand = new DelegateCommand(ExecuteDoubleClick);
            _taskbarIcon.ContextMenu = new CommandBarContextMenu();
            _taskbarIcon.ContextMenu.Opened += (sender, args) =>
            {
                _commandBar.Clear();

                _filesetService.Filesets.ForEach(fs =>
                {
                    _commandBar
                        .AddSubmenu(fs.Name)
                        .AddCommand(new CommandModel
                        {
                            Content = "Execute file copy",
                            Command = new DelegateCommand<Fileset>(model => _replicationService.Execute(model)),
                            CommandParameter = fs,
                            Icon = new Image
                            {
                                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/light/appbar.layer.png"))
                            }
                        });
                });
            };
            _taskbarIcon.ContextMenu.DataContext = _commandBar;
        }

        private void OnItemErrorCopy(Tuple<ReplicationItem, Exception> payload)
        {
            _taskbarIcon.HideBalloonTip();
            _taskbarIcon.ShowBalloonTip(
                payload.Item1.Fileset.Name,
                "Unable to copy " + payload.Item1.FileName + ".\n" + payload.Item2.Message,
                BalloonIcon.Error);
        }

        private void OnItemCopied(ReplicationItem item)
        {
            _taskbarIcon.HideBalloonTip();
            _taskbarIcon.ShowBalloonTip(
                item.Fileset.Name,
                item.FileName + " copied",
                BalloonIcon.Info);
        }

        private void ExecuteDoubleClick()
        {
            var shell = Application.Current.MainWindow;

            if (shell.IsVisible)
            {
                shell.Hide();
            }
            else
            {
                shell.Show();
                shell.Activate();
            }
        }

        #endregion Methods
    }
}