// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;

namespace SonarLink.TE.UnitTests
{
    /// <summary>
    /// A temporary file representation which is deleted once disposed.
    /// </summary>
    public class TemporaryFile : IDisposable
    {
        /// <summary>
        /// Private constructor - use available factory methods.
        /// </summary>
        /// <see cref="File(string)"/>
        /// <see cref="CreateFile(string)"/>
        /// <see cref="Directory(string)"/>
        /// <see cref="CreateDirectory(string)"/>
        private TemporaryFile() { }

        /// <summary>
        /// Temporary File
        /// </summary>
        /// <param name="path">The path to the temporary file</param>
        public static TemporaryFile File(string path)
        {
            return new TemporaryFile()
            {
                Path = path,
                Tester = System.IO.File.Exists,
                Deleter = System.IO.File.Delete
            };
        }

        /// <summary>
        /// Touches a Temporary File
        /// </summary>
        /// <param name="path">The path to the temporary file</param>
        public static TemporaryFile CreateFile(string path)
        {
            System.IO.File.Create(path).Close();
            return File(path);
        }

        /// <summary>
        /// Temporary Directory
        /// </summary>
        /// <param name="path">The path to the temporary directory</param>
        /// <param name="recursive">Flag which determines whether deletion is recursive</param>
        public static TemporaryFile Directory(string path, bool recursive = false)
        {
            Action<string> deleter = System.IO.Directory.Delete;
            if (recursive)
            {
                deleter = (x) => System.IO.Directory.Delete(x, true);
            }

            return new TemporaryFile()
            {
                Path = path,
                Tester = System.IO.Directory.Exists,
                Deleter = deleter
            };
        }

        /// <summary>
        /// Creates a new Temporary Directory
        /// </summary>
        /// <param name="path">The path to the temporary directory</param>
        /// <param name="recursive">Flag which determines whether deletion is recursive</param>
        public static TemporaryFile CreateDirectory(string path, bool recursive = false)
        {
            System.IO.Directory.CreateDirectory(path);
            return Directory(path, recursive);
        }

        /// <summary>
        /// Tries to delete the temporary file
        /// </summary>
        /// <returns>true if deletion is successful; false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool Delete()
        {
            try
            {
                if ((Path != null) && Tester(Path))
                {
                    Deleter(Path);
                    return true;
                }
            }
            catch (Exception)
            {
                // Silence Exception
            }

            return false;
        }

        /// <summary>
        /// Temporary file path
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Tester function which determines if the file system item exists
        /// </summary>
        private Func<string, bool> Tester { get; set; }

        /// <summary>
        /// File system item deleter
        /// </summary>
        private Action<string> Deleter { get; set; }

        /// <summary>
        /// Releases the temporary file from resource management
        /// </summary>
        /// <returns>The path to the temporary file</returns>
        public string Release()
        {
            string path = Path;
            Path = null;
            return path;
        }

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty(Path))
            {
                if (disposing)
                {
                    Delete();
                }

                Release();
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}
