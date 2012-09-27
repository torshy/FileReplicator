using System;
using System.IO;
using System.Reflection;
using System.Windows;

using NDesk.Options;

namespace TRock.FileReplicator
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (location != null)
            {
                Environment.CurrentDirectory = location;
            }

            bool minimized = false;
            bool hidden = false;

            OptionSet p = new OptionSet()
                .Add("m|minimized", delegate(string v) { minimized = v != null; })
                .Add("hidden", delegate(string v) { hidden = v != null; });
            p.Parse(e.Args);

            var bootstrapper = new FileReplicatorBootstrapper();
            bootstrapper.StartMinimized = minimized;
            bootstrapper.StartHidden = hidden;
            bootstrapper.Run();
        }
    }
}
