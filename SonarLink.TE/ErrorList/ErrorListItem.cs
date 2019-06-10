// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.VisualStudio.Shell.Interop;
using SonarLink.API.Models;
using SonarLink.API.Services;
using System;
using System.Globalization;
using System.Linq;

namespace SonarLink.TE.ErrorList
{

    /// <summary>
    /// Item in 'Error List' window for a SonarQube issue
    /// </summary>
    public class ErrorListItem
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ErrorListItem()
        {
        }
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">SonarQube source from which this issue originates</param>
        /// <param name="issue">SonarQube issue</param>
        public ErrorListItem(ISonarQubeService service, SonarQubeIssue issue)
            : this()
        {
            var errorCode = GetErrorCode(issue.Rule);
    
            ProjectName = string.Empty;
            FileName = GetFileName(issue.Component);
    
            // Visual Studio expects line to be 0-based rather than 1-based like SonarQube
            // Reference: https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.tablemanager.standardtablekeynames.line?view=visualstudiosdk-2017
            Line = issue.Line - 1;
    
            Message = GetErrorMessage(issue.Message);
            ErrorCode = errorCode;
            ErrorCodeToolTip = GetErrorCodeToolTip(errorCode);
            ErrorCategory = GetErrorCategory(issue.Severity, issue.Type);
            Severity = __VSERRORCATEGORY.EC_MESSAGE;
            HelpLink = GetHelpLink(service.BaseUrl, issue.Rule);
        }
    
        /// <summary>
        /// Project name of the error item
        /// </summary>
        public string ProjectName { get; set; }
    
        /// <summary>
        /// File name of the error item
        /// </summary>
        public string FileName { get; set; }
    
        /// <summary>
        /// 0-based line of code on the error item
        /// </summary>
        public int Line { get; set; }
    
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }
    
        /// <summary>
        /// Error code for the error item
        /// </summary>
        public string ErrorCode { get; set; }
    
        /// <summary>
        /// Error code tool tip
        /// </summary>
        public string ErrorCodeToolTip { get; set; }
    
        /// <summary>
        /// Error category
        /// </summary>
        public string ErrorCategory { get; set; }
    
        /// <summary>
        /// Severity of the error item
        /// </summary>
        public __VSERRORCATEGORY Severity { get; set; }
    
        /// <summary>
        /// Error help link
        /// </summary>
        public string HelpLink { get; set; }
    
        #region Static helpers
    
        /// <summary>
        /// Identifies the filename portion from the respective SonarQube component field
        /// </summary>
        /// <param name="fileName">SonarQube component field from an issue response</param>
        /// <returns>File name to which the SonarQube issue is attributed to</returns>
        private static string GetFileName(string fileName)
        {
            var cleanFileName = fileName.Split(':').LastOrDefault();
            return cleanFileName.Replace("/", "\\");
        }

        /// <summary>
        /// Identifies the error message component from a SonarQube error message serialization
        /// </summary>
        /// <param name="message">SonarQube error message</param>
        /// <returns>Error message suitable for listing in 'Error List' window</returns>
        private static string GetErrorMessage(string message)
        {
            if (message.Contains("QACPP"))
            {
                var colonIndex = message.IndexOf(']');
                return message.Substring(colonIndex + 3);
            }
    
            if (message.Contains("warning"))
            {
                var colonIndex = message.IndexOf(':');
                return message.Substring(colonIndex + 3);
            }
    
            return message;
        }

        /// <summary>
        /// Identifies the respective Visual Studio error category for the SonarQube issue
        /// </summary>
        /// <param name="severity">SonarQube issue severty</param>
        /// <param name="type">SonarQube issue type</param>
        /// <returns></returns>
        private static string GetErrorCategory(string severity, string type)
        {
            var errorCategory = string.Empty;
            var textInfo = CultureInfo.CurrentCulture.TextInfo;
    
            if (!severity.Contains("INFO"))
            {
                errorCategory += textInfo.ToTitleCase(severity.ToLower()) + ' ';
            }
    
            errorCategory += textInfo.ToTitleCase(type.ToLower()).Replace('_', ' ');
    
            return errorCategory;
        }

        /// <summary>
        /// (Attempts to) Map an error code for the provided rule
        /// </summary>
        /// <param name="rule">SonarQube (violated) rule</param>
        /// <returns>Error code for the provided rule</returns>
        private static string GetErrorCode(string rule)
        {
            if (rule.Any(char.IsDigit))
            {
                var colonIndex = rule.IndexOf(':');
                var ruleId = rule.Substring(colonIndex + 1);
                return ruleId.Replace(".", "");
            }
    
            return string.Empty;
        }

        /// <summary>
        /// Identifies the URL for for the SonarQube rule relative to the origin SonarQube server
        /// </summary>
        /// <param name="baseUrl">Base SonarQube server URL</param>
        /// <param name="rule">SonarQube (error) rule</param>
        /// <returns>URL for for the SonarQube rule</returns>
        private static string GetHelpLink(Uri baseUrl, string rule)
        {
            var builder = new UriBuilder(baseUrl);
    
            builder.Path = "coding_rules";
            builder.Fragment = $"rule_key={Uri.EscapeDataString(rule)}";
    
            return builder.ToString();
        }

        /// <summary>
        /// Default 'Error List' item tooltip text for error hyperlink
        /// </summary>
        /// <param name="errorCode">Associated error code to SonarQube error</param>
        /// <returns>tooltip text for error hyperlink</returns>
        private static string GetErrorCodeToolTip(string errorCode)
        {
            return $"Get help for '{errorCode}'";
        }
        #endregion
    }
}
