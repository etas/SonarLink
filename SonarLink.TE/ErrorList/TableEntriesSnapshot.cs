// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.TableManager;

namespace SonarLink.TE.ErrorList
{
    /// <summary>
    /// Snapshot of a SonarQube project issue listing
    /// </summary>
    public class TableEntriesSnapshot : TableEntriesSnapshotBase
    {
        /// <summary>
        /// Snapshot version number
        /// </summary>
        private readonly int _versionNumber = 1;

        /// <summary>
        /// Error list
        /// </summary>
        private readonly List<ErrorListItem> _errors;
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="projectName">SonarQube project name</param>
        /// <param name="filePath">Associated source code file path</param>
        /// <param name="errors">Errors associated to SonarQube projects</param>
        public TableEntriesSnapshot(string projectName, string filePath, IEnumerable<ErrorListItem> errors)
        {
            ProjectName = projectName;
            FilePath = filePath;
            _errors = errors.ToList();
        }

        #region ITableEntriesSnapshot

        public string ProjectName { get; private set; }
    
        public string FilePath { get; private set; }
    
        public override int Count
        {
            get { return _errors.Count; }
        }
    
        public override int VersionNumber
        {
            get { return _versionNumber; }
        }
    
        public override bool TryGetValue(int index, string columnName, out object content)
        {
            if ((index < 0) || (index >= Count))
            {
                content = null;
                return false;
            }
    
            switch(columnName)
            {
            case StandardTableKeyNames.ProjectName:
                content = ProjectName;
                break;
    
            case StandardTableKeyNames.DocumentName:
                // We return the full file path here. The UI handles displaying only the filename.
                content = FilePath;
                break;
    
            case StandardTableKeyNames.Text:
                content = _errors[index].Message;
                break;
    
            case StandardTableKeyNames.Line:
                content = _errors[index].Line;
                break;
    
            case StandardTableKeyNames.ErrorCategory:
                content = _errors[index].ErrorCategory;
                break;
    
            case StandardTableKeyNames.ErrorSeverity:
                content = _errors[index].Severity;
                break;
    
            case StandardTableKeyNames.ErrorCode:
                content = _errors[index].ErrorCode;
                break;
    
            case StandardTableKeyNames.BuildTool:
                content = "SonarLink";
                break;
    
            case StandardTableKeyNames.HelpLink:
                content = _errors[index].HelpLink;
                break;
    
            case StandardTableKeyNames.ErrorCodeToolTip:
                content = _errors[index].ErrorCodeToolTip;
                break;
    
            default:
                content = null;
                return false;
            }
    
            return true;
        }

        #endregion
    }
}
