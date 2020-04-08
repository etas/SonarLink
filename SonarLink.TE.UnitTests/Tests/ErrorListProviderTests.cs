// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;
using SonarLink.API.Clients;
using SonarLink.API.Models;
using SonarLink.TE.ErrorList;
using SonarLink.TE.UnitTests.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class ErrorListProviderTests
    {
        /// <summary>
        /// Test setup
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            var componentsClient = new Mock<IComponentsClient>();
            componentsClient.
                Setup(i => i.GetAllProjects()).
                Returns(Task.FromResult(new List<SonarQubeProject>()
                {
                    new SonarQubeProject()
                    {
                        Key = "A",
                        Name = "Project A"
                    },

                    new SonarQubeProject()
                    {
                        Key = "B",
                        Name = "Project B"
                    }
                }));

            var issuesClient = new Mock<IIssuesClient>();
            issuesClient.
                Setup(i => i.GetProjectIssues(It.Is<string>(key => key == "A"))).
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

            issuesClient.
                Setup(i => i.GetProjectIssues(It.Is<string>(key => key == "B"))).
                Returns(Task.FromResult(new List<SonarQubeIssue>()));

            var client = new Mock<ISonarQubeClient>();
            client.SetupGet(i => i.Components).Returns(componentsClient.Object);
            client.SetupGet(i => i.Issues).Returns(issuesClient.Object);
            client.SetupGet(i => i.SonarQubeApiUrl).Returns(new Uri("https://server.com/"));

            Provider = new ErrorListProvider(client.Object);
        }

        /// <summary>
        /// ErrorListProvider instance under test
        /// </summary>
        private ErrorListProvider Provider { get; set; } = null;

        /// <summary>
        /// Assert that: Errors are reported for a SonarQube project which has errors
        /// </summary>
        [Test]
        public async Task ErrorEnumeration()
        {
            var errors = await Provider.GetErrorsAsync("A", null);

            var expected = new List<ErrorListItem>()
                {
                    new ErrorListItem()
                    {
                        ProjectName = "",
                        FileName = "git\\proj-a\\src\\a.cpp",
                        Line = 181,
                        Message = "This expression casts between two pointer types.",
                        ErrorCode = "QACPP3030",
                        ErrorCodeToolTip = "Get help for 'QACPP3030'",
                        ErrorCategory = "Vulnerability",
                        Severity = __VSERRORCATEGORY.EC_MESSAGE
                    },

                    new ErrorListItem()
                    {
                        ProjectName = "",
                        FileName = "git\\proj-a\\src\\b.cpp",
                        Line = 58,
                        Message = "This operation is redundant. The value of the result is always that of the left-hand operand.",
                        ErrorCode = "QACPP2985",
                        ErrorCodeToolTip = "Get help for 'QACPP2985'",
                        ErrorCategory ="Minor Bug",
                        Severity = __VSERRORCATEGORY.EC_ERROR
                    }
                };

            Assert.That(errors, Is.EqualTo(expected).Using(new ErrorListItemComparer()));
        }

        /// <summary>
        /// Assert that: No errors are reported for a SonarQube project which does not have any associated errors
        /// </summary>
        [Test]
        public async Task ErrorEnumerationCleanProject()
        {
            var errors = await Provider.GetErrorsAsync("B", null);
            Assert.That(errors, Is.Empty);
        }

        /// <summary>
        /// Assert that: Errors are reported with a resolved file path
        ///              for a SonarQube project which has errors and
        ///              the associated file path can be resolved
        /// </summary>
        [Test]
        public async Task ErrorEnumerationWithResolvedPath()
        {
            using (var git = TemporaryFile.CreateDirectory(Path.Combine(Path.GetTempPath(), "git")))
            using (var proja = TemporaryFile.CreateDirectory(Path.Combine(git.Path, "proj-a")))
            using (var src = TemporaryFile.CreateDirectory(Path.Combine(proja.Path, "src")))
            using (var a = TemporaryFile.CreateFile(Path.Combine(src.Path, "a.cpp")))
            using (var b = TemporaryFile.CreateFile(Path.Combine(src.Path, "b.cpp")))
            {
                var errors = await Provider.GetErrorsAsync("A", git.Path);

                var expected = new List<ErrorListItem>()
                    {
                        new ErrorListItem()
                        {
                            ProjectName = "",
                            FileName = a.Path,
                            Line = 181,
                            Message = "This expression casts between two pointer types.",
                            ErrorCode = "QACPP3030",
                            ErrorCodeToolTip = "Get help for 'QACPP3030'",
                            ErrorCategory = "Vulnerability",
                            Severity = __VSERRORCATEGORY.EC_MESSAGE
                        },

                        new ErrorListItem()
                        {
                            ProjectName = "",
                            FileName = b.Path,
                            Line = 58,
                            Message = "This operation is redundant. The value of the result is always that of the left-hand operand.",
                            ErrorCode = "QACPP2985",
                            ErrorCodeToolTip = "Get help for 'QACPP2985'",
                            ErrorCategory ="Minor Bug",
                            Severity = __VSERRORCATEGORY.EC_ERROR
                        }
                    };

                Assert.That(errors, Is.EqualTo(expected).Using(new ErrorListItemComparer()));
            }
        }
    }
}
