using System.Windows.Controls;

namespace TRock.FileReplicator.Views.Welcome
{
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