// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using System;
using System.IO;
using System.Text.Json;

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Loads and saves repository data items from/to a .json file.
    /// </summary>
    public class JsonRepository<T> : IRepository<T> where T : new()
    {
        /// <summary>
        /// File path of JSON file
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Repository data item
        /// </summary>
        public T Data { get; private set; } = new T();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">File path of JSON file</param>
        public JsonRepository(string filePath)
        {
            _filePath = filePath;

            EnsureJsonFileExists(_filePath);
            ReadFromJsonFile();
        }

        /// <summary>
        /// Persist repository data item to JSON file
        /// </summary>
        public void Save()
        {
           WriteToJsonFile();
        }

        /// <summary>
        /// Creates JSON file at the specified path if the former does not exist
        /// </summary>
        /// <param name="filePath">File path of JSON file</param>
        private void EnsureJsonFileExists(string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(filePath))
            {
                WriteToJsonFile();
            }
        }

        /// <summary>
        /// Write repository data item to JSON file
        /// </summary>
        private void WriteToJsonFile()
        {
            var json = JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        /// <summary>
        /// Read repository data item from JSON file
        /// </summary>
        private void ReadFromJsonFile()
        {
            try
            {
                Data = JsonSerializer.Deserialize<T>(File.ReadAllText(_filePath));
            }
            catch (Exception)
            {
                File.Delete(_filePath);
                EnsureJsonFileExists(_filePath);
            }
        }
    }
}
