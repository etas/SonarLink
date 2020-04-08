// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;

namespace SonarLink.API.Clients
{
    /// <summary>
    /// Base class for an API client.
    /// </summary>
    public abstract class ApiClient
    {
        /// <summary>
        /// Initializes a new API client.
        /// </summary>
        /// <param name="restClient">Underlying HTTP rest client</param>
        protected ApiClient(IRestClient restClient)
        {
            RestClient = restClient;
        }

        /// <summary>
        /// Rest client to make rest requests to HTTP endpoints.
        /// </summary>
        protected IRestClient RestClient { get; private set; }
    }
}
