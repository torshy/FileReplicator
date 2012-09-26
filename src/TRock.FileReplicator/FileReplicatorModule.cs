using System;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using TRock.FileReplicator.Events;
using TRock.FileReplicator.Views.Filesets;

namespace TRock.FileReplicator
{
    public class FileReplicatorModule : IModule
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        #endregion Fields

        #region Constructors

        public FileReplicatorModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Methods

        public void Initialize()
        {
            _eventAggregator.GetEvent<CopiedEvent>().Subscribe(item =>
            {

            }, ThreadOption.UIThread, true);

            _regionManager.RequestNavigate(AppRegions.LeftRegion, new Uri(typeof(IFilesetListView).Name, UriKind.RelativeOrAbsolute));
        }

        #endregion Methods
    }
}