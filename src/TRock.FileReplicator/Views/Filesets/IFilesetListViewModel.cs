using System.Collections.Generic;
using System.ComponentModel;

using Microsoft.Practices.Prism.Commands;

using TRock.FileReplicator.Services;
using TRock.FileReplicator.ViewModels;

namespace TRock.FileReplicator.Views.Filesets
{
    public interface IFilesetListViewModel
    {
        #region Properties

        ICollectionView Filesets
        {
            get;
        }

        ICommandBar CommandBar
        {
            get;
        }

        DelegateCommand AddFilesetCommand
        {
            get;
        }

        DelegateCommand<FilesetViewModel> RemoveFilesetCommand
        {
            get;
        }

        DelegateCommand SaveAllFilesetsCommand
        {
            get;
        }

        #endregion Properties

        #region Methods

        void RefreshCommandBar(IEnumerable<FilesetViewModel> selectedFilesets);

        #endregion Methods
    }
}