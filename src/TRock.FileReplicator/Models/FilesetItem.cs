using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TRock.FileReplicator.Models
{
    public class FilesetItem
    {
        #region Properties

        public string RelativePath
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public static string GetFullPath(string baseDirectory, string relativePath)
        {
            var combined = Path.Combine(baseDirectory, relativePath);
            return Path.GetFullPath(combined);
        }

        public static string GetRelativePath(string baseDirectory, string fullPath)
        {
            const Int32 maxPath = 260;
            var builder = new StringBuilder(maxPath);
            var result = PathRelativePathTo(
                 builder,
                 baseDirectory, FileAttributes.Directory,
                 fullPath, FileAttributes.Normal);

            if (result)
            {
                return builder.ToString();
            }

            return null;
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathRelativePathTo(
            [Out] StringBuilder pszPath,
            [In] string pszFrom,
            [In] FileAttributes dwAttrFrom,
            [In] string pszTo,
            [In] FileAttributes dwAttrTo
            );

        #endregion Methods
    }
}