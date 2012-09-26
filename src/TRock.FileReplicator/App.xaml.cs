using System.Windows;

namespace TRock.FileReplicator
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var bootstrapper = new FileReplicatorBootstrapper();
            bootstrapper.Run();
        }
    }
}
