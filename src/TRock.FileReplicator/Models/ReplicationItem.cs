using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace TRock.FileReplicator.Models
{
    public class ReplicationItem
    {
        #region Fields

        private int _retries;

        #endregion Fields

        #region Constructors

        public ReplicationItem()
        {
            Data = new ExpandoObject();
        }

        #endregion Constructors

        #region Properties

        public string FullSourcePath
        {
            get;
            set;
        }

        public string FullDestinationPath
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public Fileset Fileset
        {
            get;
            set;
        }

        public int Retries
        {
            get { return _retries; }
        }

        public dynamic Data
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public string[] KillLockingProcesses()
        {
            var lockingProcessPaths = new List<string>();
            var fileName = Path.Combine(Environment.CurrentDirectory, "Resources", "handle.exe");

            if (File.Exists(fileName))
            {
                var tool = new Process();
                tool.StartInfo.FileName = fileName;
                tool.StartInfo.Arguments = "\"" + FullDestinationPath + "\"";
                tool.StartInfo.UseShellExecute = false;
                tool.StartInfo.RedirectStandardOutput = true;
                tool.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                tool.Start();
                tool.WaitForExit();
                string outputTool = tool.StandardOutput.ReadToEnd();
                const string matchPattern = @"(?<=\s+pid:\s+)\b(\d+)\b(?=\s+)";

                foreach (Match match in Regex.Matches(outputTool, matchPattern))
                {
                    Process process = Process.GetProcessById(int.Parse(match.Value));
                    lockingProcessPaths.Add(process.MainModule.FileName);
                    process.Kill();
                }
            }

            return lockingProcessPaths.ToArray();
        }

        public int IncrementRetries()
        {
            return Interlocked.Increment(ref _retries);
        }

        #endregion Methods
    }
}