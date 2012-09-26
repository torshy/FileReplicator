using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using Newtonsoft.Json;

using TRock.FileReplicator.Models;
using TRock.FileReplicator.ViewModels;
using System.Linq;

namespace TRock.FileReplicator.Services
{
    public class FilesetService : IFilesetService
    {
        #region Fields

        private readonly Subject<Fileset> _filesetAdded;
        private readonly Subject<Fileset> _filesetRemoved;
        private readonly List<Fileset> _filesets;

        #endregion Fields

        #region Constructors

        public FilesetService()
        {
            _filesetAdded = new Subject<Fileset>();
            _filesetRemoved = new Subject<Fileset>();
            _filesets = new List<Fileset>();
        }

        #endregion Constructors

        #region Properties

        public IObservable<Fileset> FilesetAdded
        {
            get { return _filesetAdded; }
        }

        public IObservable<Fileset> FilesetRemoved
        {
            get { return _filesetRemoved; }
        }

        public IEnumerable<Fileset> Filesets
        {
            get { return _filesets; }
        }

        #endregion Properties

        #region Methods

        public Task<IEnumerable<Fileset>> Save()
        {
            var filesets = Filesets.ToArray();

            return Task<IEnumerable<Fileset>>.Factory.StartNew(state =>
            {
                var sets = (IEnumerable<Fileset>) state;
               
                foreach (var set in sets)
                {
                    SaveFileset(set);
                }

                return sets;
            }, filesets);
        }

        public Task<Fileset> Save(Fileset fileset)
        {
            return Task.Factory.StartNew(() =>
            {
                SaveFileset(fileset);
                return fileset;
            });
        }

        public Task<Fileset> Add()
        {
            return Task.Factory.StartNew(() =>
            {
                var fs = new Fileset
                {
                    Id = Guid.NewGuid(),
                    Name = "New fileset"
                };

                fs.OnCopyScripts.Add(new ScriptFile
                {
                    Path = Path.Combine("Scripts", "Events", "Copy.rb")
                });

                fs.OnCopyErrorScripts.Add(new ScriptFile
                {
                    Path = Path.Combine("Scripts", "Events", "CopyError.rb")
                });

                fs.OnCopySuccessScripts.Add(new ScriptFile
                {
                    Path = Path.Combine("Scripts", "Events", "CopySuccess.rb")
                });

                _filesets.Add(fs);
                _filesetAdded.OnNext(fs);

                return fs;
            });
        }

        public Task Remove(Fileset fileset)
        {
            return Task.Factory.StartNew(() =>
            {
                string path = GetFilesetPath(fileset);

                if (File.Exists(path))
                {
                    File.Delete(path);

                    if (_filesets.Remove(fileset))
                    {
                        _filesetRemoved.OnNext(fileset);
                    }
                }
            });
        }

        public void Initialize()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var file in Directory.EnumerateFiles(AppConstants.FileSetsFolder, "*.fis"))
                {
                    try
                    {
                        using (var stream = File.Open(file, FileMode.Open))
                        using (var streamReader = new StreamReader(stream))
                        using (var jsonReader = new JsonTextReader(streamReader))
                        {
                            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.Objects };
                            var fileset = serializer.Deserialize<Fileset>(jsonReader);
                            _filesets.Add(fileset);
                            _filesetAdded.OnNext(fileset);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            })
            .ContinueWith(task =>
            {
                Trace.WriteLineIf(task.IsFaulted, task.Exception);
            });
        }

        private static void SaveFileset(Fileset fileset)
        {
            var filePath = GetFilesetPath(fileset);

            using (var stream = File.Create(filePath))
            using (var textWriter = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                var serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.Objects;
                serializer.Serialize(jsonWriter, fileset);
            }
        }

        private static string GetFilesetPath(Fileset fileset)
        {
            string fileName = fileset.Id.ToString() + ".fis";
            string filePath = Path.Combine(AppConstants.FileSetsFolder, fileName);
            return filePath;
        }

        #endregion Methods
    }
}