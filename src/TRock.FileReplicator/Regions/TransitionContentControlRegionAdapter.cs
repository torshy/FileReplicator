using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

using MahApps.Metro.Controls;

using Microsoft.Practices.Prism.Regions;

namespace TRock.FileReplicator.Regions
{
    public class TransitionContentControlRegionAdapter : RegionAdapterBase<TransitioningContentControl>
    {
        #region Constructors

        public TransitionContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        #endregion Constructors

        #region Methods

        protected override void Adapt(IRegion region, TransitioningContentControl regionTarget)
        {
            if (regionTarget == null)
            {
                throw new ArgumentNullException("regionTarget");
            }

            if ((regionTarget.Content != null) || (BindingOperations.GetBinding(regionTarget, ContentControl.ContentProperty) != null))
            {
                throw new InvalidOperationException("ContentControlHasContentException");
            }

            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.Content = region.ActiveViews.FirstOrDefault<object>();
            };

            region.Views.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
            {
                if ((e.Action == NotifyCollectionChangedAction.Add) && (!region.ActiveViews.Any()))
                {
                    region.Activate(e.NewItems[0]);
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new TransitionSingleActiveRegion();
        }

        #endregion Methods
    }
}