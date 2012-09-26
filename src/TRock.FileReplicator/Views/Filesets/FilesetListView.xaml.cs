using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using TRock.FileReplicator.ViewModels;

namespace TRock.FileReplicator.Views.Filesets
{
    public class DisableEnterDataGrid : DataGrid
    {
        #region Methods

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key != Key.Enter || e.Key != Key.Return)
            {
                base.OnKeyDown(e);
            }
            else
            {
                var currentCell = CurrentCell;
                var currentIndex = SelectedIndex;
                base.OnKeyDown(e);
                SelectedIndex = currentIndex;
                CurrentCell = currentCell;
            }
        }

        #endregion Methods
    }

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

        private void DataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var multiSelector = (MultiSelector)sender;
            Model.RefreshCommandBar(multiSelector.SelectedItems.OfType<FilesetViewModel>());
        }

        #endregion Methods
    }
}