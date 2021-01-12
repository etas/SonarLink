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
                var errors = await Provider.GetErrorsAsync("A", Path.GetTempPath());

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

        /// <summary>
        /// Assert that: Errors from files with file paths with similar roots
        ///              are enumerated correctly by simply using the local path
        ///              as reference
        /// </summary>
        /// <remarks>https://github.com/etas/SonarLink/issues/31</remarks>
        [Test]
        public async Task SubstringRootPath()
        {
            const string serviceKey = "MultiService";

            var componentsClient = new Mock<IComponentsClient>();
            componentsClient.
                Setup(i => i.GetAllProjects()).
                Returns(Task.FromResult(new List<SonarQubeProject>()
                {
                    new SonarQubeProject()
                    {
                        Key = serviceKey,
                        Name = "Project MultiService"
                    }
                }));

            var issuesClient = new Mock<IIssuesClient>();
            issuesClient.
                Setup(i => i.GetProjectIssues(It.Is<string>(key => key == serviceKey))).
                Returns(Task.FromResult(new List<SonarQubeIssue>()
                {
                    new SonarQubeIssue()
                    {
                        Rule = "other:QACPP.3030",
                        Severity = "INFO",
                        Component = "serviceKey:Service/Implementation.cs",
                        Line = 182,
                        Message = "QACPP[1:3030]  Service/Implementation.cs",
                        Type ="VULNERABILITY"
                    },

                    new SonarQubeIssue()
                    {
                        Rule = "other:QACPP.3030",
                        Severity = "INFO",
                        Component = "serviceKey:AService/Implementation.cs",
                        Line = 182,
                        Message = "QACPP[1:3030]  AService/Implementation.cs",
                        Type ="VULNERABILITY"
                    },

                    new SonarQubeIssue()
                    {
                        Rule = "other:QACPP.3030",
                        Severity = "INFO",
                        Component = "serviceKey:BService/Implementation.cs",
                        Line = 182,
                        Message = "QACPP[1:3030]  BService/Implementation.cs",
                        Type ="VULNERABILITY"
                    },
                    new
                    SonarQubeIssue()
                    {
                        Rule = "other:QACPP.3030",
                        Severity = "INFO",
                        Component = "serviceKey:Service/Service/Implementation.cs",
                        Line = 182,
                        Message = "QACPP[1:3030]  Service/Service/Implementation.cs",
                        Type ="VULNERABILITY"
                    },
                }));

            var client = new Mock<ISonarQubeClient>();
            client.SetupGet(i => i.Components).Returns(componentsClient.Object);
            client.SetupGet(i => i.Issues).Returns(issuesClient.Object);
            client.SetupGet(i => i.SonarQubeApiUrl).Returns(new Uri("https://server.com/"));

            var provider = new ErrorListProvider(client.Object);

            using (var root = TemporaryFile.CreateDirectory(Path.Combine(Path.GetTempPath(), "root")))
            using (var serviceA = TemporaryFile.CreateDirectory(Path.Combine(root.Path, "AService")))
            using (var serviceAImpl = TemporaryFile.CreateFile(Path.Combine(serviceA.Path, "Implementation.cs")))
            using (var serviceB = TemporaryFile.CreateDirectory(Path.Combine(root.Path, "BService")))
            using (var serviceBImpl = TemporaryFile.CreateFile(Path.Combine(serviceB.Path, "Implementation.cs")))
            using (var service = TemporaryFile.CreateDirectory(Path.Combine(root.Path, "Service")))
            using (var serviceImpl = TemporaryFile.CreateFile(Path.Combine(service.Path, "Implementation.cs")))
            using (var serviceService = TemporaryFile.CreateDirectory(Path.Combine(service.Path, "Service")))
            using (var serviceServiceImpl = TemporaryFile.CreateFile(Path.Combine(serviceService.Path, "Implementation.cs")))
            {
                var errors = await provider.GetErrorsAsync(serviceKey, root.Path);

                var expected = new List<ErrorListItem>()
                    {
                        new ErrorListItem()
                        {
                            ProjectName = "",
                            FileName = serviceImpl.Path,
                            Line = 181,
                            Message = "Service/Implementation.cs",
                            ErrorCode = "QACPP3030",
                            ErrorCodeToolTip = "Get help for 'QACPP3030'",
                            ErrorCategory = "Vulnerability",
                            Severity = __VSERRORCATEGORY.EC_MESSAGE
                        },

                        new ErrorListItem()
                        {
                            ProjectName = "",
                            FileName = serviceAImpl.Path,
                            Line = 181,
                            Message = "AService/Implementation.cs",
                            ErrorCode = "QACPP3030",
                            ErrorCodeToolTip = "Get help for 'QACPP3030'",
                            ErrorCategory = "Vulnerability",
                            Severity = __VSERRORCATEGORY.EC_MESSAGE
                        },

                        new ErrorListItem()
                        {
                            ProjectName = "",
                            FileName = serviceBImpl.Path,
                            Line = 181,
                            Message = "BService/Implementation.cs",
                            ErrorCode = "QACPP3030",
                            ErrorCodeToolTip = "Get help for 'QACPP3030'",
                            ErrorCategory = "Vulnerability",
                            Severity = __VSERRORCATEGORY.EC_MESSAGE
                        },

                        new ErrorListItem()
                        {
                            ProjectName = "",
                            FileName = serviceServiceImpl.Path,
                            Line = 181,
                            Message = "Service/Service/Implementation.cs",
                            ErrorCode = "QACPP3030",
                            ErrorCodeToolTip = "Get help for 'QACPP3030'",
                            ErrorCategory = "Vulnerability",
                            Severity = __VSERRORCATEGORY.EC_MESSAGE
                        }
                    };

                Assert.That(errors, Is.EqualTo(expected).Using(new ErrorListItemComparer()));
            }
        }

        /// <summary>
        /// Assert that: File paths may be incorrectly resolved in case the
        ///              root folder does not host the file in question
        /// </summary>
        /// <remarks>https://github.com/etas/SonarLink/issues/31</remarks>
        [Test]
        public async Task InvalidSubstringRootPath()
        {
            const string serviceKey = "MultiService";

            var componentsClient = new Mock<IComponentsClient>();
            componentsClient.
                Setup(i => i.GetAllProjects()).
                Returns(Task.FromResult(new List<SonarQubeProject>()
                {
                    new SonarQubeProject()
                    {
                        Key = serviceKey,
                        Name = "Project MultiService"
                    }
                }));

            var issuesClient = new Mock<IIssuesClient>();
            issuesClient.
                Setup(i => i.GetProjectIssues(It.Is<string>(key => key == serviceKey))).
                Returns(Task.FromResult(new List<SonarQubeIssue>()
                {
                    new SonarQubeIssue()
                    {
                        Rule = "other:QACPP.3030",
                        Severity = "INFO",
                        Component = "serviceKey:Service/Implementation.cs",
                        Line = 182,
                        Message = "QACPP[1:3030]  Service/Implementation.cs",
                        Type ="VULNERABILITY"
                    }
                }));

            var client = new Mock<ISonarQubeClient>();
            client.SetupGet(i => i.Components).Returns(componentsClient.Object);
            client.SetupGet(i => i.Issues).Returns(issuesClient.Object);
            client.SetupGet(i => i.SonarQubeApiUrl).Returns(new Uri("https://server.com/"));

            var provider = new ErrorListProvider(client.Object);

            using (var root = TemporaryFile.CreateDirectory(Path.Combine(Path.GetTempPath(), "root")))
            using (var serviceA = TemporaryFile.CreateDirectory(Path.Combine(root.Path, "AService")))
            using (var serviceAImpl = TemporaryFile.CreateFile(Path.Combine(serviceA.Path, "Implementation.cs")))
            {
                var errors = await provider.GetErrorsAsync(serviceKey, root.Path);
                var expected = new List<ErrorListItem>()
                    {
                        new ErrorListItem()
                        {
                            ProjectName = "",
                            // Path resolved incorrectly
                            FileName = Path.Combine(root.Path, "Service\\Implementation.cs"),
                            Line = 181,
                            Message = "Service/Implementation.cs",
                            ErrorCode = "QACPP3030",
                            ErrorCodeToolTip = "Get help for 'QACPP3030'",
                            ErrorCategory = "Vulnerability",
                            Severity = __VSERRORCATEGORY.EC_MESSAGE
                        },
                };

                Assert.That(errors, Is.EqualTo(expected).Using(new ErrorListItemComparer()));
            }
        }
    }
}
