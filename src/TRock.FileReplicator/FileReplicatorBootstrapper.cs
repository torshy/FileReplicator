using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using Autofac;

using IronRuby;

using MahApps.Metro.Controls;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Scripting.Hosting;

using Prism.AutofacExtension;

using TRock.Extensions;
using TRock.FileReplicator.Regions;
using TRock.FileReplicator.Services;

namespace TRock.FileReplicator
{
    public class FileReplicatorBootstrapper : AutofacBootstrapper
    {
        #region Constructors

        public FileReplicatorBootstrapper()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        }

        #endregion Constructors

        #region Properties

        public bool StartMinimized
        {
            get;
            set;
        }

        public bool StartHidden
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<Shell>().AsSelf();
            builder.RegisterType<TransitionContentControlRegionAdapter>();
            builder.RegisterModule<FileReplicatorModuleConfiguration>();
            builder.RegisterInstance(Logger);
            builder.RegisterInstance(ModuleCatalog);
            builder.RegisterInstance(Application.Current.Dispatcher);

            RegisterTypeIfMissing(builder, typeof(IServiceLocator), typeof(CustomAutofacAdapter), true);
            RegisterTypeIfMissing(builder, typeof(IModuleInitializer), typeof(ModuleInitializer), true);
            RegisterTypeIfMissing(builder, typeof(IModuleManager), typeof(ModuleManager), true);
            RegisterTypeIfMissing(builder, typeof(RegionAdapterMappings), typeof(RegionAdapterMappings), true);
            RegisterTypeIfMissing(builder, typeof(IRegionManager), typeof(RegionManager), true);
            RegisterTypeIfMissing(builder, typeof(SelectorRegionAdapter), typeof(SelectorRegionAdapter), true);
            RegisterTypeIfMissing(builder, typeof(ItemsControlRegionAdapter), typeof(ItemsControlRegionAdapter), true);
            RegisterTypeIfMissing(builder, typeof(ContentControlRegionAdapter), typeof(ContentControlRegionAdapter), true);
            RegisterTypeIfMissing(builder, typeof(DelayedRegionCreationBehavior), typeof(DelayedRegionCreationBehavior), false);
            RegisterTypeIfMissing(builder, typeof(BindRegionContextToDependencyObjectBehavior), typeof(BindRegionContextToDependencyObjectBehavior), false);
            RegisterTypeIfMissing(builder, typeof(AutoPopulateRegionBehavior), typeof(AutoPopulateRegionBehavior), false);
            RegisterTypeIfMissing(builder, typeof(RegionActiveAwareBehavior), typeof(RegionActiveAwareBehavior), false);
            RegisterTypeIfMissing(builder, typeof(SyncRegionContextWithHostBehavior), typeof(SyncRegionContextWithHostBehavior), false);
            RegisterTypeIfMissing(builder, typeof(RegionManagerRegistrationBehavior), typeof(RegionManagerRegistrationBehavior), false);
            RegisterTypeIfMissing(builder, typeof(RegionMemberLifetimeBehavior), typeof(RegionMemberLifetimeBehavior), false);
            RegisterTypeIfMissing(builder, typeof(ClearChildViewsRegionBehavior), typeof(ClearChildViewsRegionBehavior), false);
            RegisterTypeIfMissing(builder, typeof(IEventAggregator), typeof(EventAggregator), true);
            RegisterTypeIfMissing(builder, typeof(IRegionViewRegistry), typeof(RegionViewRegistry), true);
            RegisterTypeIfMissing(builder, typeof(IRegionBehaviorFactory), typeof(RegionBehaviorFactory), true);
            RegisterTypeIfMissing(builder, typeof(IRegionNavigationJournalEntry), typeof(RegionNavigationJournalEntry), false);
            RegisterTypeIfMissing(builder, typeof(IRegionNavigationJournal), typeof(RegionNavigationJournal), false);
            RegisterTypeIfMissing(builder, typeof(IRegionNavigationService), typeof(RegionNavigationService), false);
            RegisterTypeIfMissing(builder, typeof(IRegionNavigationContentLoader), typeof(RegionNavigationContentLoader), true);

            CreateScriptRuntime(builder);
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();
            mappings.RegisterMapping(typeof(TransitioningContentControl), Container.Resolve<TransitionContentControlRegionAdapter>());
            return mappings;
        }

        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => new CustomAutofacAdapter(Container));
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeModules()
        {
            Directory.CreateDirectory(AppConstants.AppDataFolder);
            Directory.CreateDirectory(AppConstants.FileSetsFolder);
            Directory.CreateDirectory(AppConstants.LogFolder);

            Task.Factory.StartNew(() =>
            {
                var engine = Container.Resolve<ScriptEngine>();
                var scope = engine.CreateScope();

                scope.SetVariable("filesetService", Container.Resolve<IFilesetService>());
                scope.SetVariable("replicatorService", Container.Resolve<IFileReplicationService>());
                scope.SetVariable("activityLogService", Container.Resolve<IActivityLogService>());

                foreach (var script in Directory.EnumerateFiles(@"Scripts\Modules", "*.rb"))
                {
                    engine.ExecuteFile(script, scope);
                }
            })
            .ContinueWith(task =>
            {
                Trace.WriteLineIf(task.IsFaulted, task.Exception);
            });

            base.InitializeModules();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            var shell = (Shell)Shell;

            WindowSettings.SetSave(shell, true);
            Application.Current.MainWindow = shell;

            if (!StartHidden)
            {
                Application.Current.MainWindow.Show();
            }

            Application.Current.MainWindow.WindowState = StartMinimized ? WindowState.Minimized : WindowState.Normal;
        }

        protected void CreateScriptRuntime(ContainerBuilder builder)
        {
            var runtime = Ruby.CreateRuntime();
            AppDomain.CurrentDomain.GetAssemblies().ForEach(runtime.LoadAssembly);
            var engine = runtime.GetEngine("rb");

            var searchPaths = new List<string>(engine.GetSearchPaths())
            {
                @".\Scripts\Lib",
                @".\Scripts\Lib\IronRuby",
                @".\Scripts\Lib\Ruby\1.9.1"
            };

            engine.SetSearchPaths(searchPaths);

            builder.RegisterInstance(runtime);
            builder.RegisterInstance(engine);

            Directory.CreateDirectory(@"Scripts");
            Directory.CreateDirectory(@"Scripts\Modules");
            Directory.CreateDirectory(@"Scripts\Events");
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            // register prism module
            Type coreModule = typeof(FileReplicatorModule);
            ModuleCatalog.AddModule(new ModuleInfo(coreModule.Name, coreModule.AssemblyQualifiedName));
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            using (var stream = new FileStream(Path.Combine(AppConstants.LogFolder, "Crash.log"), FileMode.Create, FileAccess.Write))
            {
                using(var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(e.ExceptionObject);
                }
            }
        }

        private void RegisterTypeIfMissing(ContainerBuilder builder, Type fromType, Type toType, bool registerAsSingleton)
        {
            if (fromType == null)
            {
                throw new ArgumentNullException("fromType");
            }

            if (toType == null)
            {
                throw new ArgumentNullException("toType");
            }

            if (registerAsSingleton)
            {
                builder.RegisterType(toType).As(new[] { fromType }).SingleInstance();
            }
            else
            {
                builder.RegisterType(toType).As(new[] { fromType });
            }
        }

        #endregion Methods
    }
}