using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

using Autofac;

using Microsoft.Practices.Prism.Events;
using Microsoft.Scripting.Hosting;

using TRock.FileReplicator.Events;
using TRock.FileReplicator.Models;

namespace TRock.FileReplicator.Services
{
    public class FileReplicationService : IFileReplicationService, IStartable
    {
        #region Fields

        private readonly IActivityLogService _activityLogService;
        private readonly string _copyEventScriptFile = @"Scripts\Events\Copy.rb";
        private readonly CancellationTokenSource _cts;
        private readonly IEventAggregator _eventAggregator;

        //private readonly string _copyErrorEventScriptFile = @"Scripts\Events\CopyError.rb";
        //private readonly string _copySuccessEventScriptFile = @"Scripts\Events\CopySuccess.rb";
        private readonly IFilesetService _filesetService;
        private readonly BlockingCollection<ReplicationItem> _replicationQueue;
        private readonly ScriptEngine _scriptEngine;
        private readonly ConcurrentDictionary<Fileset, List<IDisposable>> _watchers;

        private Thread _replicationThread;

        #endregion Fields

        #region Constructors

        public FileReplicationService(
            IFilesetService filesetService,
            IActivityLogService activityLogService,
            IEventAggregator eventAggregator,
            ScriptEngine scriptEngine)
        {
            _filesetService = filesetService;
            _activityLogService = activityLogService;
            _eventAggregator = eventAggregator;
            _scriptEngine = scriptEngine;
            _watchers = new ConcurrentDictionary<Fileset, List<IDisposable>>();
            _replicationQueue = new BlockingCollection<ReplicationItem>();
            _cts = new CancellationTokenSource();
        }

        #endregion Constructors

        #region Methods

        public void Start()
        {
            var filesets = _filesetService.Filesets.ToArray();
            _filesetService.FilesetAdded.Subscribe(FilesetAdded);
            _filesetService.FilesetRemoved.Subscribe(FilesetRemoved);

            foreach (var fileset in filesets)
            {
                FilesetAdded(fileset);
            }

            _replicationThread = new Thread(ReplicationRun);
            _replicationThread.IsBackground = true;
            _replicationThread.Start();
        }

        public void Execute(Fileset fileset)
        {
            _activityLogService.Log(fileset.Id, "Executing manual copy");

            foreach (var item in fileset.Includes)
            {
                var fileName = Path.GetFileName(item.RelativePath);
                var filePath = FilesetItem.GetFullPath(fileset.SourcePath, item.RelativePath);

                var replicationItem = new ReplicationItem
                {
                    Fileset = fileset,
                    FileName = fileName,
                    FullSourcePath = filePath,
                    FullDestinationPath = Path.Combine(fileset.DestinationPath, fileName)
                };
                replicationItem.Data.LockingProcesses = new string[0];
                _replicationQueue.TryAdd(replicationItem);
            }

            if (fileset.Excludes.Any())
            {
                var relativePath = fileset.Excludes.FirstOrDefault().RelativePath;
                var fullPath = FilesetItem.GetFullPath(fileset.SourcePath, relativePath);
                var directoryName = Path.GetDirectoryName(fullPath);

                if (!string.IsNullOrEmpty(directoryName))
                {
                    IEnumerable<string> files = Directory.GetFiles(directoryName);
                    files = files.Except(fileset.Excludes.Select(f =>
                    {
                        return FilesetItem.GetFullPath(fileset.SourcePath, f.RelativePath);
                    }));

                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        var replicationItem = new ReplicationItem
                        {
                            Fileset = fileset,
                            FileName = fileName,
                            FullSourcePath = file,
                            FullDestinationPath = Path.Combine(fileset.DestinationPath, fileName)
                        };
                        replicationItem.Data.LockingProcesses = new string[0];
                        _replicationQueue.TryAdd(replicationItem);
                    }
                }
            }
        }

        public void ExecuteScripts(ScriptEngine engine, ScriptScope scope, IEnumerable<Script> scripts)
        {
            if (scripts != null)
            {
                foreach (var scriptPiece in scripts)
                {
                    scriptPiece.Execute(engine, scope);
                }
            }
        }

        private void FilesetAdded(Fileset fileset)
        {
            fileset.IsEnabledChanged += OnFilesetIsEnabledChanged;

            if (fileset.IsEnabled)
            {
                ActivateFileset(fileset);
            }
        }

        private void FilesetRemoved(Fileset fileset)
        {
            fileset.IsEnabledChanged -= OnFilesetIsEnabledChanged;

            if (fileset.IsEnabled)
            {
                DeactivateFileset(fileset);
            }
        }

        private void ActivateFileset(Fileset fileset)
        {
            List<IDisposable> watchers;
            if (!_watchers.TryGetValue(fileset, out watchers))
            {
                watchers = new List<IDisposable>();
                _watchers.TryAdd(fileset, watchers);
            }

            DeactivateFileset(fileset);

            var distinctIncludeFolders = fileset.Includes.Select(f =>
            {
                var fullPath = FilesetItem.GetFullPath(fileset.SourcePath, f.RelativePath);
                return new FileInfo(fullPath).DirectoryName;
            }).Distinct();

            var distinctExcludeFolders = fileset.Excludes.Select(f =>
            {
                var fullPath = FilesetItem.GetFullPath(fileset.SourcePath, f.RelativePath);
                return new FileInfo(fullPath).DirectoryName;
            }).Distinct();

            var foldersToWatch = distinctIncludeFolders.Union(distinctExcludeFolders);

            foreach (var folderToWatch in foldersToWatch)
            {
                if (Directory.Exists(folderToWatch))
                {
                    IDisposable observable = GetObservable(folderToWatch)
                        .Buffer(TimeSpan.FromSeconds(2))
                        .Subscribe(t =>
                    {
                        if (t.Any())
                        {
                            var fileNames = t.Distinct();

                            foreach (var filePath in fileNames)
                            {
                                var relativePath = FilesetItem.GetRelativePath(fileset.SourcePath, filePath);

                                if (fileset.Includes.Any(f => f.RelativePath == relativePath))
                                {
                                    var fileName = Path.GetFileName(filePath);
                                    var replicationItem = new ReplicationItem
                                    {
                                        Fileset = fileset,
                                        FileName = fileName,
                                        FullSourcePath = filePath,
                                        FullDestinationPath = Path.Combine(fileset.DestinationPath, fileName)
                                    };
                                    replicationItem.Data.LockingProcesses = new string[0];
                                    _replicationQueue.TryAdd(replicationItem);
                                }

                                if (fileset.Excludes.Any())
                                {
                                    string directoryName = Path.GetDirectoryName(filePath);

                                    if (!string.IsNullOrEmpty(directoryName))
                                    {
                                        IEnumerable<string> files = Directory.GetFiles(directoryName);
                                        files = files.Except(fileset.Excludes.Select(f =>
                                        {
                                            return FilesetItem.GetFullPath(fileset.SourcePath, f.RelativePath);
                                        }));

                                        foreach (var file in files)
                                        {
                                            var fileName = Path.GetFileName(file);
                                            var replicationItem = new ReplicationItem
                                            {
                                                Fileset = fileset,
                                                FileName = fileName,
                                                FullSourcePath = filePath,
                                                FullDestinationPath = Path.Combine(fileset.DestinationPath, fileName)
                                            };
                                            replicationItem.Data.LockingProcesses = new string[0];

                                            _replicationQueue.TryAdd(replicationItem);
                                        }
                                    }
                                }
                            }
                        }
                    });

                    watchers.Add(observable);
                }
            }
        }

        private void DeactivateFileset(Fileset fileset)
        {
            List<IDisposable> watchers;
            if (!_watchers.TryGetValue(fileset, out watchers))
            {
                watchers = new List<IDisposable>();
                _watchers.TryAdd(fileset, watchers);
            }

            watchers.ForEach(watcher => watcher.Dispose());
            watchers.Clear();
        }

        private void OnFilesetIsEnabledChanged(object sender, EventArgs e)
        {
            Fileset fileset = (Fileset)sender;

            if (fileset.IsEnabled)
            {
                ActivateFileset(fileset);
            }
            else
            {
                DeactivateFileset(fileset);
            }
        }

        private IObservable<string> GetObservable(string path)
        {
            return Observable.Create<string>(observer =>
            {
                FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(path) { EnableRaisingEvents = true };
                FileSystemEventHandler created = (s, e) => observer.OnNext(e.FullPath);
                FileSystemEventHandler changed = (s, e) => observer.OnNext(e.FullPath);
                ErrorEventHandler error = (_, errorArg) => observer.OnError(errorArg.GetException());
                fileSystemWatcher.Created += created;
                fileSystemWatcher.Changed += changed;
                fileSystemWatcher.Error += error;
                return () =>
                {
                    fileSystemWatcher.Created -= created;
                    fileSystemWatcher.Changed -= changed;
                    fileSystemWatcher.Error -= error;
                    fileSystemWatcher.Dispose();
                };
            });
        }

        private void ReplicationRun()
        {
            try
            {
                foreach (var item in _replicationQueue.GetConsumingEnumerable(_cts.Token))
                {
                    ReplicationItem currentItem = item;

                    ScriptScope scope = _scriptEngine.CreateScope();
                    scope.SetVariable("fileInfo", currentItem);
                    scope.SetVariable("fileset", currentItem.Fileset);
                    scope.SetVariable("retryCopy", new Action(() =>
                    {
                        currentItem.IncrementRetries();
                        _replicationQueue.Add(currentItem);
                    }));
                    scope.SetVariable("log", new Action<string>(message =>
                    {
                        _activityLogService.Log(currentItem.Fileset.Id, message);
                    }));

                    if (File.Exists(_copyEventScriptFile))
                    {
                        try
                        {
                            ExecuteScripts(_scriptEngine, scope, currentItem.Fileset.OnCopyScripts);
                            ExecuteScripts(_scriptEngine, scope, currentItem.Fileset.OnCopySuccessScripts);

                            _eventAggregator.GetEvent<CopiedEvent>().Publish(currentItem);
                        }
                        catch (Exception e)
                        {
                            scope.SetVariable("exception", e);
                            ExecuteScripts(_scriptEngine, scope, currentItem.Fileset.OnCopyErrorScripts);

                            _eventAggregator.GetEvent<CopyErrorEvent>().Publish(Tuple.Create(currentItem, e));
                        }
                        finally
                        {
                            ExecuteScripts(_scriptEngine, scope, currentItem.Fileset.OnCopyFinallyScripts);
                        }
                    }
                }
            }
            catch (OperationCanceledException oce)
            {
                Trace.WriteLine(oce);
            }
        }

        #endregion Methods
    }
}