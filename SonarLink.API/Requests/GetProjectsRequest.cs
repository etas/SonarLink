// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using RestSharp.Deserializers;
using System.Collections.Generic;
using SonarLink.API.Models;

namespace SonarLink.API.Requests
{
    /// <summary>
    /// Request to get a list all projects on SonarQube.
    /// </summary>
    public class GetProjectsRequest : PagedRequestBase<SonarQubeProject>
    {
    
        /// <inheritdoc />
        protected override string Path => "api/components/search";
    
        /// <inheritdoc />
        protected override IRestRequest GenerateRequest()
        {
            return base.GenerateRequest()
                   .AddParameter("qualifiers", "TRK"); // TRK - Projects qualifier
        }
    
        /// <inheritdoc />
        protected override List<SonarQubeProject> ParseResponse(IRestResponse response)
        {
            return new JsonDeserializer().Deserialize<ParsedResponse>(response).Components;
        }
    
        /// <summary>
        /// Describes the parsed response with properties matching the deserialized JSON string.
        /// </summary>
        private class ParsedResponse
        {
            /// <summary>
            /// List of SonarQube projects.
            /// </summary>
            public List<SonarQubeProject> Components { get; set; }
        }
    }
}