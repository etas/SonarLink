using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using NUnit.Framework;
using SonarLink.API.Clients;
using SonarLink.API.Models;
using SonarLink.TE.ErrorList;
using SonarLink.TE.UnitTests.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class IssueCategoriesTests
    {
        /// <summary>
        /// Assert that: a given SonarQubes issue is mapped to the correct error list 
        /// category, based on its type and severity.
        /// </summary>
        [TestCase("BUG", "BLOCKER", "Blocker Bug", __VSERRORCATEGORY.EC_ERROR)]
        [TestCase("BUG", "CRITICAL", "Critical Bug", __VSERRORCATEGORY.EC_ERROR)]
        [TestCase("BUG", "MAJOR", "Major Bug", __VSERRORCATEGORY.EC_ERROR)]
        [TestCase("BUG", "MINOR", "Minor Bug", __VSERRORCATEGORY.EC_ERROR)]
        [TestCase("BUG", "INFO", "Bug", __VSERRORCATEGORY.EC_MESSAGE)]
        [TestCase("VULNERABILITY", "BLOCKER", "Blocker Vulnerability", __VSERRORCATEGORY.EC_ERROR)]
        [TestCase("VULNERABILITY", "CRITICAL", "Critical Vulnerability", __VSERRORCATEGORY.EC_ERROR)]
        [TestCase("VULNERABILITY", "MAJOR", "Major Vulnerability", __VSERRORCATEGORY.EC_ERROR)]
        [TestCase("VULNERABILITY", "MINOR", "Minor Vulnerability", __VSERRORCATEGORY.EC_ERROR)]
        [TestCase("VULNERABILITY", "INFO", "Vulnerability", __VSERRORCATEGORY.EC_MESSAGE)]
        [TestCase("CODE_SMELL", "BLOCKER", "Blocker Code Smell", __VSERRORCATEGORY.EC_WARNING)]
        [TestCase("CODE_SMELL", "CRITICAL", "Critical Code Smell", __VSERRORCATEGORY.EC_WARNING)]
        [TestCase("CODE_SMELL", "MAJOR", "Major Code Smell", __VSERRORCATEGORY.EC_WARNING)]
        [TestCase("CODE_SMELL", "MINOR", "Minor Code Smell", __VSERRORCATEGORY.EC_WARNING)]
        [TestCase("CODE_SMELL", "INFO", "Code Smell", __VSERRORCATEGORY.EC_MESSAGE)]
        public async Task SonarQubeIssueToErrorListCategoryMap(string issueType, 
            string issueSeverity, 
            string expectedErrorCategopry, 
            __VSERRORCATEGORY expectedErrorSeverity)
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
                }));

            var issuesClient = new Mock<IIssuesClient>();
            issuesClient.
                Setup(i => i.GetProjectIssues(It.Is<string>(key => key == "A"))).
                Returns(Task.FromResult(new List<SonarQubeIssue>()
                {
                    new SonarQubeIssue()
                    {
                        Rule = "other:QACPP.3030",
                        Severity = issueSeverity,
                        Component = "A:git/proj-a/src/a.cpp",
                        Line = 182,
                        Message = "QACPP[1:3030]  This expression casts between two pointer types.",
                        Type = issueType
                    },
                }));

            var client = new Mock<ISonarQubeClient>();
            client.SetupGet(i => i.SonarQubeApiUrl).Returns(new Uri("https://server.com/"));
            client.SetupGet(i => i.Components).Returns(componentsClient.Object);
            client.SetupGet(i => i.Issues).Returns(issuesClient.Object);

            var provider = new ErrorListProvider(client.Object);
            var errors = await provider.GetErrorsAsync("A", null);

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
                        ErrorCategory = expectedErrorCategopry,
                        Severity = expectedErrorSeverity
                    },
                };

            Assert.That(errors, Is.EqualTo(expected).Using(new ErrorListItemComparer()));
        }
    }
}
