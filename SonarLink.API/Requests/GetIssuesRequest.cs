// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;
using System.Collections.Generic;
using SonarLink.API.Models;
using RestSharp;
using RestSharp.Deserializers;

namespace SonarLink.API.Requests
{

    /// <summary>
    /// SonarQube issue categories
    /// </summary>
    /// <remarks>Reference: http://{sonarqube}/web_api/api/issues/search</remarks>
    public enum IssueType
    {
        Bug,
        Vulnerability,
        CodeSmell
    }
    
    /// <summary>
    /// Request to get a list of all the issues pertaining to a given project.
    /// </summary>
    public class GetIssuesRequest : PagedRequestBase<SonarQubeIssue>
    {
        /// <summary>
        /// Unique key associated with the project.
        /// </summary>
        public string ProjectKey { get; set; }
    
        /// <summary>
        /// Unique key associated with the project.
        /// </summary>
        public IssueType? Type { get; set; } = null;
    
        /// <inheritdoc />
        protected override string Path => "api/issues/search";
    
        /// <inheritdoc />
        protected override IRestRequest GenerateRequest()
        {
            if (string.IsNullOrEmpty(ProjectKey))
            {
                throw new ArgumentNullException(nameof(ProjectKey));
            }
    
            var request = base.GenerateRequest()
                              .AddParameter("statuses", "OPEN")
                              .AddParameter("componentKeys", ProjectKey);
            
            if (Type != null)
            {
                request = request.AddParameter("types", IssueTypeAsQueryParameter(Type.Value));
            }
    
            return request;
        }
    
        /// <inheritdoc />
        protected override List<SonarQubeIssue> ParseResponse(IRestResponse response)
        {
            return new JsonDeserializer().Deserialize<ParsedResponse>(response).Issues;
        }
    
        /// <summary>
        /// Transforms the issue type into a valid query string value.
        /// </summary>
        /// <param name="value">Issue type.</param>
        /// <returns>Query string value representation of the provided issue type.</returns>
        private static string IssueTypeAsQueryParameter(IssueType value)
        {
            switch (value)
            {
                case IssueType.CodeSmell:
                    return "CODE_SMELL";
    
                default:
                    return value.ToString().ToUpperInvariant();
            }
        }
    
        /// <summary>
        /// Describes the parsed response with properties matching the deserialized JSON string.
        /// </summary>
        private class ParsedResponse
        {
            /// <summary>
            /// List of SonarQube issues.
            /// </summary>
            public List<SonarQubeIssue> Issues { get; set; }
        }
    }
}