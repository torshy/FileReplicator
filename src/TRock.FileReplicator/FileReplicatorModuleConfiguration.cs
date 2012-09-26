using Autofac;
using TRock.FileReplicator.Services;
using TRock.FileReplicator.Views.Fileset;
using TRock.FileReplicator.Views.Filesets;
using TRock.FileReplicator.Views.Settings;

namespace TRock.FileReplicator
{
    public class FileReplicatorModuleConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<AppTaskbarIconHandler>()
                .As<IStartable>()
                .SingleInstance();

            builder.RegisterType<FilesetService>()
                .As<IFilesetService>()
                .SingleInstance()
                .OnActivating(a => a.Instance.Initialize());

            builder.RegisterType<ActivityLogService>()
                .As<IActivityLogService>()
                .SingleInstance()
                .OnActivating(a => a.Instance.MaxNumberOfActivityEventsPerLog = 250);

            builder.RegisterType<FileReplicationService>()
                .As<IFileReplicationService>()
                .As<IStartable>()
                .SingleInstance();

            builder.RegisterType<ShellViewModel>()
                .As<IShellViewModel>();

            builder.RegisterType<SettingsView>().As<ISettingsView>().Named<object>(typeof(ISettingsView).Name);
            builder.RegisterType<SettingsViewModel>().As<ISettingsViewModel>();

            builder.RegisterType<FileReplicatorModule>();
            builder.RegisterType<FilesetListViewModel>().As<IFilesetListViewModel>();
            builder.RegisterType<FilesetListView>().As<IFilesetListView>().Named<object>(typeof(IFilesetListView).Name);

            builder.RegisterType<FilesetViewViewModel>().As<IFilesetViewViewModel>();
            builder.RegisterType<FilesetView>().As<IFilesetView>().Named<object>(typeof(IFilesetView).Name);
        }
    }
}