using System;
using System.IO;

namespace TRock.FileReplicator
{
    public class AppConstants
    {
        #region Fields

        public static readonly string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FileReplicator");
        public static readonly string LogFolder = Path.Combine(AppDataFolder, "Logs");
        public static readonly string FileSetsFolder = Path.Combine(AppDataFolder, "FileSets");

        #endregion Fields
    }
}