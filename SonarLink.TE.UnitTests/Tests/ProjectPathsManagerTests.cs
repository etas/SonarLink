using Moq;
using NUnit.Framework;
using SonarLink.TE.Utilities;
using System.Collections.Generic;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class ProjectPathsManagerTests
    {
        /// <summary>
        /// Test setup
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            Repository = new Mock<IProjectPathsDataRepository>();

            var data = new ProjectPathData();
            data.ProjectPaths = new Dictionary<string, string>();
            data.ProjectPaths.Add("projectKey", "projectPath");

            Repository.
                Setup(i => i.Data).
                Returns(data);

            ProjectPathsManager = new ProjectPathsManager(Repository.Object);
        }

        /// <summary>
        /// Project paths manager instance under test
        /// </summary>
        private IProjectPathsManager ProjectPathsManager { get; set; }

        /// <summary>
        /// Mock project paths data repository 
        /// </summary>
        private Mock<IProjectPathsDataRepository> Repository { get; set; }

        /// <summary>
        /// Assert that: New project and path pairs are correctly added and
        /// the repository's save method is invoked.
        /// </summary>
        [Test]
        public void AddNewProjetPath()
        {
            var projectPaths = Repository.Object.Data.ProjectPaths;
            Assert.That(projectPaths.Count, Is.EqualTo(1));

            ProjectPathsManager.Add("newProjectKey", "newProjectPath");

            Assert.That(projectPaths.Count, Is.EqualTo(2));

            Assert.That(projectPaths.ContainsKey("projectKey"), Is.True);
            Assert.That(projectPaths["projectKey"], Is.EqualTo("projectPath"));

            Assert.That(projectPaths.ContainsKey("newProjectKey"), Is.True);
            Assert.That(projectPaths["newProjectKey"], Is.EqualTo("newProjectPath"));

            Repository.Verify(Repository => Repository.Save(), Times.Once());
        }

        /// <summary>
        /// Assert that: In case of adding an already exsting project-path pair, the
        /// newly added pair overrides the already existing one.
        /// </summary>
        [Test]
        public void AddExistingProjectPath()
        {
            var projectPaths = Repository.Object.Data.ProjectPaths;
            Assert.That(projectPaths.Count, Is.EqualTo(1));

            ProjectPathsManager.Add("projectKey", "newProjectPath");

            Assert.That(projectPaths.Count, Is.EqualTo(1));

            Assert.That(projectPaths.ContainsKey("projectKey"), Is.True);
            Assert.That(projectPaths["projectKey"], Is.EqualTo("newProjectPath"));
            Repository.Verify(Repository => Repository.Save(), Times.Once());
        }

        /// <summary>
        /// Assert that: In case of an invalid project or path value, the existing
        /// data repositry is unchanged and the save method is not invoked.
        /// </summary>
        [Test]
        public void AddInvalidProjectPath()
        {
            var projectPaths = Repository.Object.Data.ProjectPaths;
            Assert.That(projectPaths.Count, Is.EqualTo(1));

            ProjectPathsManager.Add("newProjectKey", null);

            Assert.That(projectPaths.Count, Is.EqualTo(1));
            Repository.Verify(Repository => Repository.Save(), Times.Never());

            ProjectPathsManager.Add(null, "newProjectPath");

            Assert.That(projectPaths.Count, Is.EqualTo(1));
            Repository.Verify(Repository => Repository.Save(), Times.Never());
        }

        /// <summary>
        /// Assert that: The project path mananger returns the expected
        /// path given a valid and existing project key.
        /// </summary>
        [Test]
        public void ValidTryGetValue()
        {
            ProjectPathsManager.TryGetvalue("projectKey", out string path);
            Assert.That(path, Is.EqualTo("projectPath"));
        }

        /// <summary>
        /// Assert that: The project path mananger returns a null
        /// path given an invalid or non-existing project key.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        [TestCase("newProjectKey")]
        public void InvalidTryGetValue(string project)
        {
            ProjectPathsManager.TryGetvalue(project, out string path);
            Assert.That(path, Is.Null);
        }
    }
}
