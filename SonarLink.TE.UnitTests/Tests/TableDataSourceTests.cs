// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableManager;
using Moq;
using NUnit.Framework;
using SonarLink.TE.ErrorList;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class TableDataSourceTests
    {
        /// <summary>
        /// Test setup
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            Sink = new StubTableDataSink();
            Table = new SonarLinkIssueDataSource();

            SubscribeToken = Table.Subscribe(Sink);
        }

        /// <summary>
        /// Test tear down
        /// </summary>
        [TearDown]
        public void TestTearDown()
        {
            SubscribeToken.Dispose();
        }

        /// <summary>
        /// Stub data sink which is notified of new table entries
        /// </summary>
        private StubTableDataSink Sink { get; set; }

        /// <summary>
        /// Table instance under test
        /// </summary>
        private SonarLinkIssueDataSource Table { get; set; }

        /// <summary>
        /// Data sink subscribe token
        /// </summary>
        private IDisposable SubscribeToken { get; set; }

        /// <summary>
        /// Prototype error items
        /// </summary>
        private IEnumerable<ErrorListItem> Errors { get; } = new[]
        {
            new ErrorListItem()
            {
                ProjectName = "",
                FileName = "git\\proj-a\\src\\a.cpp",
                Line = 182,
                Message = "This expression casts between two pointer types.",
                ErrorCode = "QACPP3030",
                ErrorCodeToolTip = "Get help for 'QACPP3030'",
                ErrorCategory = "Vulnerability",
                Severity = __VSERRORCATEGORY.EC_MESSAGE,
                HelpLink = "https://www.google.com"
            },

            new ErrorListItem()
            {
                ProjectName = "",
                FileName = "git\\proj-a\\src\\b.cpp",
                Line = 59,
                Message = "This operation is redundant. The value of the result is always that of the left-hand operand.",
                ErrorCode = "QACPP2985",
                ErrorCodeToolTip = "Get help for 'QACPP2985'",
                ErrorCategory ="Minor Bug",
                Severity = __VSERRORCATEGORY.EC_MESSAGE,
                HelpLink = "https://www.google.com"
            }
        };

        /// <summary>
        /// Assert that: It is possible to add errors to a table
        /// </summary>
        [Test]
        public void AddErrors()
        {
            Table.AddErrors("A", Errors);

            Assert.That(Sink.Snapshots.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Assert that: It is possible to replace a (set of) errors by filename from a table
        /// </summary>
        [Test]
        public void ReplaceError()
        {
            Table.AddErrors("A", Errors);

            Assert.That(Sink.Snapshots.Count, Is.EqualTo(2));

            Table.AddErrors("A", new[]
            {
                new ErrorListItem()
                {
                    ProjectName = "",
                    FileName = "git\\proj-a\\src\\a.cpp",
                    Line = 5,
                    Message = "This expression casts between three pointer types.",
                    ErrorCode = "QACPP3031",
                    ErrorCodeToolTip = "Get help for 'QACPP3031'",
                    ErrorCategory = "Vulnerability",
                    Severity = __VSERRORCATEGORY.EC_MESSAGE,
                    HelpLink = "https://www.google.com"
                }
            });

            Assert.That(Sink.Snapshots.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Assert that: It is possible to clear a table from all accumulated errors
        /// </summary>
        [Test]
        public void CleanAllErrors()
        {
            Table.AddErrors("A", Errors);

            Assert.That(Sink.Snapshots.Count, Is.EqualTo(2));

            Table.CleanAllErrors();

            Assert.That(Sink.Snapshots, Is.Empty);
        }
    }
}
