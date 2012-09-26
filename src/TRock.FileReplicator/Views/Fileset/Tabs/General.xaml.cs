using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace TRock.FileReplicator.Views.Fileset.Tabs
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class General : UserControl
    {
        public General()
        {
            typeof (Xceed.Wpf.Toolkit.MessageBox).ToString();

            InitializeComponent();
        }

        private void DropIncludeFiles(object sender, DragEventArgs e)
        {
            
        }

        private void DropExcludeFiles(object sender, DragEventArgs e)
        {
            
        }
    }
}
