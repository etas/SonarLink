// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using SonarLink.API.Models;
using SonarLink.API.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonarLink.API.Clients
{
    /// <summary>
    /// A client for SonarQube components API.
    /// </summary>
    public class ComponentsClient : ApiClient, IComponentsClient
    {
        /// <summary>
        /// Initializes a new SonarQube components API client.
        /// </summary>
        /// <param name="restClient">Underlying HTTP rest client</param>
        public ComponentsClient(IRestClient restClient) : base(restClient)
        {
        }

        /// <summary>
        /// Gets a list of all projects.
        /// </summary>
        /// <returns>List of projects currently on the SonarQube server.</returns>
        public Task<List<SonarQubeProject>> GetAllProjects()
        {
            return new GetProjectsRequest().InvokeAsync(RestClient);
        }
    }
}
