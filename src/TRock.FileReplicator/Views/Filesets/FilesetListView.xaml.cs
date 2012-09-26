using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using TRock.FileReplicator.ViewModels;

namespace TRock.FileReplicator.Views.Filesets
{
    public partial class FilesetListView : UserControl, IFilesetListView
    {
        #region Constructors

        public FilesetListView(IFilesetListViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public IFilesetListViewModel Model
        {
            get
            {
                return DataContext as IFilesetListViewModel;
            }
            private set
            {
                DataContext = value;
            }
        }

        #endregion Properties

        #region Methods

        private void DataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var multiSelector = (MultiSelector)sender;
            Model.RefreshCommandBar(multiSelector.SelectedItems.OfType<FilesetViewModel>());
        }

        #endregion Methods
    }
}