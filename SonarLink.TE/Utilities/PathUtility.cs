// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;
using System.IO;
using System.Reflection;

namespace SonarLink.TE.Utilities
{
    public static class PathUtility
    {
        /// <summary>
        /// Gets the path for the application data folder.
        /// </summary>
        public static string AppDataPath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var filePath = Path.Combine(appDataFolder, assemblyName);
            return Path.GetFullPath(filePath);
        }
    }
}
