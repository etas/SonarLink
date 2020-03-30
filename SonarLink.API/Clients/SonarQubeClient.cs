// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using System;

namespace SonarLink.API.Clients
{
    /// <summary>
    /// A client for the SonarQube API.
    /// </summary>
    public class SonarQubeClient : ISonarQubeClient
    {
        /// <summary>
        /// Rest client to make rest requests to HTTP endpoints.
        /// </summary>
        private readonly IRestClient _restClient;

        /// <summary>
        /// Initializes a new instance of the SonarQube API client.
        /// </summary>
        /// <param name="baseUrl">The base address for the SonarQube API</param>
        public SonarQubeClient(Uri baseUrl) :
            this(new RestClient())
        {
            _restClient.BaseUrl = baseUrl;
        }

        /// <summary>
        /// Initializes a new instance of the SonarQube API client.
        /// </summary>
        /// <param name="restClient">Underlying HTTP rest client.</param>
        internal SonarQubeClient(IRestClient restClient)
        {
            _restClient = restClient;

            Authentication = new AuthenticationClient(_restClient);
            Components = new ComponentsClient(_restClient);
            Issues = new IssuesClient(_restClient);
        }

        /// <summary>
        /// Access SonarQube's Authentication API.
        /// </summary>
        public IAuthenticationClient Authentication { get; private set; }

        /// <summary>
        /// Access SonarQube's Components API.
        /// </summary>
        public IComponentsClient Components { get; private set; }

        /// <summary>
        /// Access SonarQube's Issues API.
        /// </summary>
        public IIssuesClient Issues { get; private set; }
    }
}
