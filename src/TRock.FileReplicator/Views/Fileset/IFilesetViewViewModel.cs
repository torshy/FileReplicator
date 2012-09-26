using System.Collections.Generic;
using System.Windows.Input;

using TRock.FileReplicator.Services;
using TRock.FileReplicator.ViewModels;

namespace TRock.FileReplicator.Views.Fileset
{
    public interface IFilesetViewViewModel
    {
        #region Properties

        FilesetViewModel Fileset
        {
            get;
        }

        IEnumerable<ActivityLogEvent> ActivityLog
        {
            get;
        }

        ICommand ManualCopyCommand
        {
            get;
        }

        ICommand BrowseDestinationFolderCommand
        {
            get;
        }

        ICommand BrowseSourceFolderCommand
        {
            get;
        }

        ICommand AddIncludeFileCommand
        {
            get;
        }

        ICommand RemoveIncludeFileCommand
        {
            get;
        }

        ICommand AddExcludeFileCommand
        {
            get;
        }

        ICommand RemoveExcludeFileCommand
        {
            get;
        }

        ICommand ClearActivityLogCommand
        {
            get;
        }

        #endregion Properties
    }
}