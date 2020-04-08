// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using Moq;
using NUnit.Framework;
using SonarLink.API.Clients;
using SonarLink.API.Models;
using SonarLink.TE.ErrorList;
using SonarLink.TE.Utilities;
using SonarLink.TE.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class ProjectViewModelTests
    {
        /// <summary>
        /// Test setup
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            var teamExplorer = new Mock<ITeamExplorer>();
            var projectPathsManager = new Mock<IProjectPathsManager>();

            Client = new Mock<ISonarQubeClient>();
            Client.SetupGet(i => i.SonarQubeApiUrl).Returns(new Uri("https://server.com/"));
            ErrorSink = new StubTableDataSink();

            var table = new SonarLinkIssueDataSource();
            SubscribeToken = table.Subscribe(ErrorSink);

            Model = new ProjectViewModel(teamExplorer.Object, table, projectPathsManager.Object)
            {
                Client = Client.Object
            };
        }

        /// <summary>
        /// Test tear-down
        /// </summary>
        [TearDown]
        public void TestTearDown()
        {
            SubscribeToken.Dispose();
        }

        /// <summary>
        /// view-model instance under test
        /// </summary>
        private ProjectViewModel Model { get; set; }

        /// <summary>
        /// Mock SonarQube WEB service
        /// </summary>
        private Mock<ISonarQubeClient> Client { get; set; }

        /// <summary>
        /// Error List sink
        /// </summary>
        private StubTableDataSink ErrorSink { get; set; }

        /// <summary>
        /// Error sink subscribe token
        /// </summary>
        private IDisposable SubscribeToken { get; set; }

        /// <summary>
        /// Assert that: Requesting to 'View Issues' of a SonarQube project
        ///              lists the errors in the error list window.
        /// </summary>
        [Test]
        public void ListProjectIssues()
        {
            var issuesClient = new Mock<IIssuesClient>();
            issuesClient.
                Setup(i => i.GetProjectIssues(It.IsAny<string>())).
                Returns(Task.FromResult(new List<SonarQubeIssue>()
                {
                    new SonarQubeIssue()
                    {
                        Rule = "other:QACPP.3030",
                        Severity = "INFO",
                        Component = "A:git/proj-a/src/a.cpp",
                        Line = 182,
                        Message = "QACPP[1:3030]  This expression casts between two pointer types.",
                        Type ="VULNERABILITY"
                    },

                    new SonarQubeIssue()
                    {
                        Rule = "other:QACPP.2985",
                        Severity = "MINOR",
                        Component = "A:git/proj-a/src/b.cpp",
                        Line = 59,
                        Message = "QACPP[1:2985]  This operation is redundant. The value of the result is always that of the left-hand operand.",
                        Type = "BUG"
                    }
                }));

            Client.SetupGet(i => i.Issues).Returns(issuesClient.Object);

            Model.ItemSelectCommand.Execute(new SonarQubeProject() { Key = "A", Name = "A" });

            Assert.That(ErrorSink.Snapshots.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Assert that: Requesting to 'View Issues' of a non-existing SonarQube project lists no errors in the error list window.
        /// </summary>
        [Test]
        public void ListNonExistingProjectIssues()
        {
            var issuesClient = new Mock<IIssuesClient>();
            issuesClient.
                Setup(i => i.GetProjectIssues(It.IsAny<string>())).
                Returns(Task.FromResult(new List<SonarQubeIssue>()));

            Client.SetupGet(i => i.Issues).Returns(issuesClient.Object);

            Model.ItemSelectCommand.Execute(new SonarQubeProject() { Key = "A", Name = "A" });

            Assert.That(ErrorSink.Snapshots, Is.Empty);
        }

        /// <summary>
        /// Assert that: SonarQube project listings can be filtered
        /// </summary>
        /// <param name="filter">Project name filter</param>
        /// <returns>Project name listings following filtering</returns>
        [TestCase("", ExpectedResult = new string[] { "A", "ABCD", "BAAB", "CD", "EF" }, Description = "The empty filter is equivalent to 'accept all'")]
        [TestCase("A", ExpectedResult = new string[] { "A", "ABCD", "BAAB" })]
        [TestCase("AB", ExpectedResult = new string[] { "ABCD", "BAAB" })]
        [TestCase("EFG", ExpectedResult = new string[] { })]
        [TestCase("1", ExpectedResult = new string[] { }, Description = "It is not possible to filter via the project key")]
        public IEnumerable<string> ProjectFilter(string filter)
        {
            var componentsClient = new Mock<IComponentsClient>();
            componentsClient.
                Setup(i => i.GetAllProjects()).
                Returns(Task.FromResult(new List<SonarQubeProject>()
                {
                        new SonarQubeProject() { Key = "1", Name = "A" },
                        new SonarQubeProject() { Key = "2", Name = "ABCD" },
                        new SonarQubeProject() { Key = "3", Name = "BAAB" },
                        new SonarQubeProject() { Key = "4", Name = "CD" },
                        new SonarQubeProject() { Key = "5", Name = "EF" },
                }));

            Client.SetupGet(i => i.Components).Returns(componentsClient.Object);

            Model.RefreshAsync().Wait();

            Model.Filter = filter;

            return Model.Projects.Cast<SonarQubeProject>().Select(project => project.Name);
        }
    }
}
