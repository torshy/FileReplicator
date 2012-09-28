using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;

namespace TRock.FileReplicator.Views.Welcome
{
    [RegionMemberLifetime(KeepAlive = false)]
    public partial class WelcomeView : UserControl, IWelcomeView
    {
        #region Constructors

        public WelcomeView(IWelcomeViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public IWelcomeViewModel Model
        {
            get
            {
                return DataContext as IWelcomeViewModel;
            }
            set
            {
                DataContext = value;
            }
        }

        #endregion Properties
    }
}