using System.Windows.Controls;

namespace TRock.FileReplicator.Views.Fileset
{
    public partial class FilesetView : UserControl, IFilesetView
    {
        #region Constructors

        public FilesetView(IFilesetViewViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public IFilesetViewViewModel Model
        {
            get
            {
                return DataContext as IFilesetViewViewModel;
            }
            private set
            {
                DataContext = value;
            }
        }

        #endregion Properties
    }
}