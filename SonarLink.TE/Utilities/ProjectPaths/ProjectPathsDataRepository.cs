// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System.Reflection;

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Implementation of IProjectPathsDataRepository
    /// </summary>
    public sealed class ProjectPathsDataRepository : IProjectPathsDataRepository
    {
        /// <summary>
        /// Name of the file the project paths data is presisted to
        /// </summary>
        private static readonly string _storageFileName = "projectpaths.json";

        /// <summary>
        /// File path of the persisted project paths data
        /// </summary>
        private readonly string _storageFilePath;

        /// <summary>
        /// Sonar projects and local path associations
        /// </summary>
        public ProjectPathData Data { get; private set; } = new ProjectPathData();

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectPathsDataRepository()
            : this(GetStorageFileDirectory())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">File directory of the presisted project paths data</param>
        internal ProjectPathsDataRepository(string directory)
        {
            _storageFilePath = Path.Combine(directory, _storageFileName);

            Data.ProjectPaths = new Dictionary<string, string>();

            EnsureJsonFileExists(_storageFilePath);
            ReadFromJsonFile();
        }

        /// <summary>
        /// Gets the file directory of the persisted project paths data
        /// </summary>
        /// <returns>The file directory of the persisted project paths data</returns>
        internal static string GetStorageFileDirectory()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            var filePath = Path.Combine(appDataFolder, assemblyName);

            return Path.GetFullPath(filePath);
        }

        /// <summary>
        /// Creates JSON file at the specified path if the former does not exist
        /// </summary>
        /// <param name="filePath">The JSON file path</param>
        private void EnsureJsonFileExists(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);

            if(!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if(!File.Exists(filePath))
            {
                WriteToJsonFile();
            }
        }

        /// <summary>
        /// Write project paths data to JSON file
        /// </summary>
        private void WriteToJsonFile()
        {
            var json = JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_storageFilePath, json);
        }

        /// <summary>
        /// Read project paths data from JSON file
        /// </summary>
        private void ReadFromJsonFile()
        {
            try
            {
                Data = JsonSerializer.Deserialize<ProjectPathData>(File.ReadAllText(_storageFilePath));
            }
            catch (Exception)
            {
                File.Delete(_storageFilePath);
                EnsureJsonFileExists(_storageFilePath);
            }
        }

        /// <summary>
        /// Persist the project paths data
        /// </summary>
        public void Save()
        {
            WriteToJsonFile();
        }
    }
}
