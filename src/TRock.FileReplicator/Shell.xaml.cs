namespace TRock.FileReplicator
{
    public partial class Shell
    {
        public Shell(IShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Flyouts = viewModel.Flyouts;
        }
    }
}
