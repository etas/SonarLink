// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using NUnit.Framework;
using SonarLink.TE.Utilities.Repository;
using System.IO;
using System.Reflection;

namespace SonarLink.TE.UnitTests.Tests
{
    class ProjectPathsDataRepositoryTests
    {
        /// <summary>
        /// Test setup
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            DeleteTempFile();
        }

        /// <summary>
        /// Test tear-down
        /// </summary>
        [TearDown]
        public void TestTearDown()
        {
            DeleteTempFile();
        }

        /// <summary>
        /// Test suite temporary file path
        /// </summary>
        private readonly string TempFilePath = GetTempFilePath();

        /// <summary>
        /// Get the test suite temporary file path
        /// </summary>
        /// <returns></returns>
        private static string GetTempFilePath()
        {
            var appDataFolder = Path.GetTempPath();
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            return Path.Combine(appDataFolder, assemblyName, "tempfile.json");
        }

        /// <summary>
        /// Delete the temporary file used for tests
        /// </summary>
        private void DeleteTempFile()
        {
            if (File.Exists(TempFilePath))
            {
                File.Delete(TempFilePath);
            }
        }

        /// <summary>
        /// Assert that: An empty JSON file is created in the expected
        /// directory upon instantiation of the repository.
        /// </summary>
        [Test]
        public void JsonFileCreation()
        {
            _ = new ProjectPathsRepository(TempFilePath);
            Assert.That(Directory.GetFiles(Path.GetDirectoryName(TempFilePath), "*.json").Length, Is.EqualTo(1));
        }

        /// <summary>
        /// Assert that: Data is correctly persisted to file and is loaded
        /// to data model upon next instantiation of the repository.
        /// </summary>
        [Test]
        public void VerifyProjectPathsDataPersistence()
        {
            var repository = new ProjectPathsRepository(TempFilePath);

            Assert.That(repository.Data.ProjectToPathMap, Is.Empty);

            repository.Data.ProjectToPathMap.Add("projectKey1", "projectPath1");
            repository.Data.ProjectToPathMap.Add("projectKey2", "projectPath2");

            Assert.That(repository.Data.ProjectToPathMap.Count, Is.EqualTo(2));

            // Persist data to JSON file
            repository.Save();

            // Clear data model
            repository.Data.ProjectToPathMap.Clear();
            Assert.That(repository.Data.ProjectToPathMap, Is.Empty);

            // Instantiate new repository and assert it reads
            // JSON file and populates data model
            repository = new ProjectPathsRepository(TempFilePath);
            Assert.That(repository.Data.ProjectToPathMap.Count, Is.EqualTo(2));

            // Verify content of data model
            repository.Data.ProjectToPathMap.TryGetValue("projectKey1", out string path);
            Assert.That(path, Is.EqualTo("projectPath1"));

            repository.Data.ProjectToPathMap.TryGetValue("projectKey2", out path);
            Assert.That(path, Is.EqualTo("projectPath2"));
        }
    }
}
