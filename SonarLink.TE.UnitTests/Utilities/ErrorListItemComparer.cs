using SonarLink.TE.ErrorList;
using System.Collections.Generic;

namespace SonarLink.TE.UnitTests.Utilities
{
    /// <summary>
    /// Comparer for ErrorListItem instances
    /// </summary>
    public class ErrorListItemComparer : IComparer<ErrorListItem>
    {
        #region IComparer<ErrorListItem>

        /// <inheritdoc />
        public int Compare(ErrorListItem x, ErrorListItem y)
        {
            if (Equals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }

            int compare = x.ProjectName.CompareTo(y.ProjectName);
            if (compare != 0)
            {
                return compare;
            }

            compare = x.FileName.CompareTo(y.FileName);
            if (compare != 0)
            {
                return compare;
            }

            compare = x.Line.CompareTo(y.Line);
            if (compare != 0)
            {
                return compare;
            }

            compare = x.Message.CompareTo(y.Message);
            if (compare != 0)
            {
                return compare;
            }

            compare = x.ErrorCode.CompareTo(y.ErrorCode);
            if (compare != 0)
            {
                return compare;
            }

            compare = x.ErrorCodeToolTip.CompareTo(y.ErrorCodeToolTip);
            if (compare != 0)
            {
                return compare;
            }

            compare = x.ErrorCategory.CompareTo(y.ErrorCategory);
            if (compare != 0)
            {
                return compare;
            }

            compare = x.Severity.CompareTo(y.Severity);
            if (compare != 0)
            {
                return compare;
            }

            return compare;
        }

        #endregion
    }
}
