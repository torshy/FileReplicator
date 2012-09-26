using System.Windows;
using MahApps.Metro.Controls;

namespace TRock.FileReplicator.Views.Fileset.Dialogs
{
    public partial class TextInputDialog : MetroWindow
    {
        #region Constructors

        public TextInputDialog()
        {
            InitializeComponent();
            Owner = App.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        #endregion Constructors

        #region Properties

        public string Mask
        {
            get
            {
                return TextBox.Text;
            }
        }

        #endregion Properties

        #region Methods

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion Methods
    }
}