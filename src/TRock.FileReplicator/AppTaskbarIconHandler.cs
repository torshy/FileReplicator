using System;
using System.Windows;

using Autofac;

using Hardcodet.Wpf.TaskbarNotification;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;

using TRock.FileReplicator.Events;
using TRock.FileReplicator.Models;

namespace TRock.FileReplicator
{
    public class AppTaskbarIconHandler : IStartable
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly TaskbarIcon _taskbarIcon;

        #endregion Fields

        #region Constructors

        public AppTaskbarIconHandler(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _taskbarIcon = Application.Current.TryFindResource("App_TaskbarIcon") as TaskbarIcon;
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            _eventAggregator.GetEvent<CopiedEvent>().Subscribe(OnItemCopied, ThreadOption.UIThread, true);
            _eventAggregator.GetEvent<CopyErrorEvent>().Subscribe(OnItemErrorCopy, ThreadOption.UIThread, true);
            _taskbarIcon.DoubleClickCommand = new DelegateCommand(ExecuteDoubleClick);
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