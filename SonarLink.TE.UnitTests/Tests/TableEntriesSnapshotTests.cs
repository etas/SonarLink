// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableManager;
using NUnit.Framework;
using SonarLink.TE.ErrorList;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class TableEntriesSnapshotTests
    {
        /// <summary>
        /// Assert that: A TableEntriesSnapshot instance faithfully represents the properties of an ErrorListItem collection 
        /// </summary>
        /// <param name="index">snapshot entry index</param>
        /// <param name="columnName">snapshot entry column name</param>
        /// <returns>string representation of the column value of the specified entry index or null on error</returns>
        // --
        [TestCase(0, StandardTableKeyNames.ProjectName, Result = "A")]
        [TestCase(0, StandardTableKeyNames.DocumentName, Result = @"C:\a.cpp")]
        [TestCase(0, StandardTableKeyNames.Text, Result = "This expression casts between two pointer types.")]
        [TestCase(0, StandardTableKeyNames.Line, Result = 182)]
        [TestCase(0, StandardTableKeyNames.ErrorCategory, Result = "Vulnerability")]
        [TestCase(0, StandardTableKeyNames.ErrorSeverity, Result = __VSERRORCATEGORY.EC_MESSAGE)]
        [TestCase(0, StandardTableKeyNames.ErrorCode, Result = "QACPP3030")]
        [TestCase(0, StandardTableKeyNames.BuildTool, Result = "SonarLink")]
        [TestCase(0, StandardTableKeyNames.HelpLink, Result = "https://www.google.com")]
        [TestCase(0, StandardTableKeyNames.ErrorCodeToolTip, Result = "Get help for 'QACPP3030'")]
        // --
        [TestCase(1, StandardTableKeyNames.ProjectName, Result = "A")]
        [TestCase(1, StandardTableKeyNames.DocumentName, Result = @"C:\a.cpp")]
        [TestCase(1, StandardTableKeyNames.Text, Result = "This operation is redundant. The value of the result is always that of the left-hand operand.")]
        [TestCase(1, StandardTableKeyNames.Line, Result = 59)]
        [TestCase(1, StandardTableKeyNames.ErrorCategory, Result = "Minor Bug")]
        [TestCase(1, StandardTableKeyNames.ErrorSeverity, Result = __VSERRORCATEGORY.EC_MESSAGE)]
        [TestCase(1, StandardTableKeyNames.ErrorCode, Result = "QACPP2985")]
        [TestCase(1, StandardTableKeyNames.BuildTool, Result = "SonarLink")]
        [TestCase(1, StandardTableKeyNames.HelpLink, Result = "https://www.google.com")]
        [TestCase(1, StandardTableKeyNames.ErrorCodeToolTip, Result = "Get help for 'QACPP2985'")]
        // --
        [TestCase(-1, StandardTableKeyNames.ProjectName, Result = null, Description = "Index out of bounds (< 0)")]
        [TestCase(2, StandardTableKeyNames.ProjectName, Result = null, Description = "Index out of bounds (> max)")]
        // --
        [TestCase(0, StandardTableKeyNames.Priority, Result = null, Description = "Unknown 'columnName'")]
        public object GetErrorListItemValue(int index, string columnName)
        {
            var snapshot = new TableEntriesSnapshot("A", @"C:\a.cpp", new[]
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
            });

            object content = null;

            if (snapshot.TryGetValue(index, columnName, out content))
            {
                return content;
            }

            return null;
        }
    }
}
