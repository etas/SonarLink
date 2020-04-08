// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using Moq;
using NUnit.Framework;
using SonarLink.TE.Utilities;
using System;
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
            DeleteTempDirectory();
        }

        /// <summary>
        /// Test tear-down
        /// </summary>
        [TearDown]
        public void TestTearDown()
        {
            DeleteTempDirectory();
        }

        /// <summary>
        /// Test suite temporary directroy
        /// </summary>
        private readonly string TempDirectory = GetTempDirectory();

        /// <summary>
        /// Get the test suite temporary directory
        /// </summary>
        /// <returns></returns>
        private static string GetTempDirectory()
        {
            var appDataFolder = Path.GetTempPath();
            var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            return Path.Combine(appDataFolder, assemblyName);
        }

        /// <summary>
        /// Delete the temporary directory used for tests
        /// </summary>
        private void DeleteTempDirectory()
        {
            if (Directory.Exists(TempDirectory))
            {
                Directory.Delete(TempDirectory, true);
            }
        }

        /// <summary>
        /// Assert that: An empty JSON file is created in the expected
        /// directory upon instantiation of the repository.
        /// </summary>
        [Test]
        public void JsonFileCreation()
        {
            _ = new ProjectPathsDataRepository(TempDirectory);
            Assert.That(Directory.GetFiles(TempDirectory, "*.json").Length, Is.EqualTo(1));
        }

        /// <summary>
        /// Assert that: Data is correctly persisted to file and is loaded
        /// to data model upon next instantiation of the repository.
        /// </summary>
        [Test]
        public void VerifyProjectPathsDataPersistence()
        {
            var repository = new ProjectPathsDataRepository(TempDirectory);

            Assert.That(repository.Data.ProjectPaths, Is.Empty);

            repository.Data.ProjectPaths.Add("projectKey1", "projectPath1");
            repository.Data.ProjectPaths.Add("projectKey2", "projectPath2");

            Assert.That(repository.Data.ProjectPaths.Count, Is.EqualTo(2));

            // Presist data to JSON file
            repository.Save();

            // Clear data model
            repository.Data.ProjectPaths.Clear();
            Assert.That(repository.Data.ProjectPaths, Is.Empty);

            // Instantiate new repository and assert it reads
            // JSON file and populates data model
            repository = new ProjectPathsDataRepository(TempDirectory);
            Assert.That(repository.Data.ProjectPaths.Count, Is.EqualTo(2));

            // Verify content of data model
            repository.Data.ProjectPaths.TryGetValue("projectKey1", out string path);
            Assert.That(path, Is.EqualTo("projectPath1"));

            repository.Data.ProjectPaths.TryGetValue("projectKey2", out path);
            Assert.That(path, Is.EqualTo("projectPath2"));
        }
    }
}
