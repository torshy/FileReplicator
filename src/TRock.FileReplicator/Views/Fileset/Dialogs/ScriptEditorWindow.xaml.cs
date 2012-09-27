using System.IO;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MahApps.Metro.Controls;

namespace TRock.FileReplicator.Views.Fileset.Dialogs
{
    public partial class ScriptEditorWindow : MetroWindow
    {
        #region Constructors

        public ScriptEditorWindow()
        {
            InitializeComponent();

            using(var fileStream = new FileStream(Path.Combine("Resources", "Ruby.xshd"), FileMode.Open, FileAccess.Read))
            {
                XmlReader def = XmlReader.Create(fileStream);
                _textEditor.SyntaxHighlighting = HighlightingLoader.Load(def, null);
            }
        }

        #endregion Constructors

        #region Properties

        public string Expression
        {
            get
            {
                return _textEditor.Text;
            }
            set
            {
                _textEditor.Text = value;
            }
        }

        #endregion Properties

        #region Methods

        private void OkClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion Methods
    }
}