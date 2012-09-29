using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;

using TRock.FileReplicator.Services;
using TRock.FileReplicator.Views.Filesets;
using TRock.FileReplicator.Views.Welcome;

namespace TRock.FileReplicator
{
    public class FileReplicatorModule : IModule
    {
        #region Fields

        private readonly IFilesetService _filesetService;
        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public FileReplicatorModule(
            IFilesetService filesetService,
            IRegionManager regionManager)
        {
            _filesetService = filesetService;
            _regionManager = regionManager;
        }

        #endregion Constructors

        #region Methods

        public void Initialize()
        {
            if (!_filesetService.Filesets.Any())
            {
                _regionManager.RequestNavigate(AppRegions.MainRegion, new Uri(typeof(IWelcomeView).Name, UriKind.RelativeOrAbsolute));
            }

            _regionManager.RequestNavigate(AppRegions.LeftRegion, new Uri(typeof(IFilesetListView).Name, UriKind.RelativeOrAbsolute));

            Application.Current.MainWindow.Closing += (sender, args) =>
            {
                _filesetService.Save().ContinueWith(t => { Trace.WriteLineIf(t.IsFaulted, t.Exception); }).Wait();
            };
        }

        #endregion Methods
    }
}