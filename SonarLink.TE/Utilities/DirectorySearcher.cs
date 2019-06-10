// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Lists 'leave' directory entries for the specified root location
    /// </summary>
    public class DirectorySearcher
    {
        /// <summary>
        /// Recursively identifies all directories under the specified
        /// root folder which do not contain further sub-directories
        /// </summary>
        /// <param name="folder">Root folder</param>
        /// <returns>Descendant directories of the root directory</returns>
        public static IEnumerable<DirectoryInfo> GetDirectories(string folder)
        {
            try
            {
                return GetDirectories(new DirectoryInfo(folder));
            }
            catch (Exception)
            {
            }

            return Enumerable.Empty<DirectoryInfo>();
        }

        /// <summary>
        /// Recursively identifies all directories under the specified
        /// root folder which do not contain further sub-directories
        /// </summary>
        /// <param name="folder">Root folder</param>
        /// <returns>Descendant directories of the root directory</returns>
        public static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo folder)
        {
            var directories = new List<DirectoryInfo>();
            GetDirectories(folder, directories);
            return directories;
        }

        /// <summary>
        /// Recursively identifies all directories under the specified
        /// root folder which do not contain further sub-directories
        /// </summary>
        /// <param name="folder">Root folder</param>
        /// <param name="dirs">Target collection which will host sub-directory results</param>
        private static void GetDirectories(DirectoryInfo folder, ICollection<DirectoryInfo> dirs)
        {
            IEnumerable<DirectoryInfo> directories = null;
    
            try
            {
                directories = folder.EnumerateDirectories();
            }
            catch (Exception)
            {
            }
    
            if (directories == null)
            {
                return;
            }
    
            if (!directories.Any())
            {
                dirs.Add(folder);
            }
            else
            {
                foreach (var d in directories)
                {
                    GetDirectories(d, dirs);
                }
            }
        }
        
        /// <summary>
        /// Recursively identifies all sub-directories under the specified root folder 
        /// which do not contain further sub-directories in an optimized manner
        /// </summary>
        /// <param name="folder">Root folder</param>
        /// <returns>Task which results in the listing of sub-directories of the root directory</returns>
        public static Task<IEnumerable<DirectoryInfo>> GetDirectoriesFastAsync(string folder)
        {
            return Task.Run(() =>
            {
                return GetDirectoriesFast(folder);
            });
        }

        /// <summary>
        /// Recursively identifies all sub-directories under the specified root folder 
        /// which do not contain further sub-directories in an optimized manner
        /// </summary>
        /// <param name="folder">Root folder</param>
        /// <returns>Sub-directories of the root directory</returns>
        public static IEnumerable<DirectoryInfo> GetDirectoriesFast(string folder)
        {
            var directories = new ConcurrentBag<DirectoryInfo>();
    
            GetStartDirectories(folder, directories).AsParallel().ForAll((d) =>
            {
                foreach (var dir in GetDirectories(d))
                {
                    directories.Add(dir);
                }
            });
    
            return directories;
        }

        /// <summary>
        /// Identifies all sub-directories under the specified root folder in an optimized manner
        /// </summary>
        /// <param name="folder">Root folder</param>
        /// <returns>Sub-directories of the root directory</returns>
        private static IEnumerable<DirectoryInfo> GetStartDirectories(string folder, ConcurrentBag<DirectoryInfo> dirs)
        {
            DirectoryInfo dirInfo = null;
            IEnumerable<DirectoryInfo> directories = null;

            try
            {
                dirInfo = new DirectoryInfo(folder);
                directories = dirInfo.EnumerateDirectories();
            }
            catch (Exception)
            {
                return Enumerable.Empty<DirectoryInfo>();
            }
    
            if (!directories.Any())
            {
                dirs.Add(dirInfo);
            }
    
            return directories;
        }
    }
}
