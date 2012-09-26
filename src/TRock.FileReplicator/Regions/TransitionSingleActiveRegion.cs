using System.Linq;

using Microsoft.Practices.Prism.Regions;

namespace TRock.FileReplicator.Regions
{
    public class TransitionSingleActiveRegion : Region
    {
        #region Methods

        public override void Activate(object view)
        {
            object currentActiveView = ActiveViews.FirstOrDefault();

            base.Activate(view);

            if (currentActiveView != null && currentActiveView != view && Views.Contains(currentActiveView))
            {
                Deactivate(currentActiveView);
            }
        }

        #endregion Methods
    }
}