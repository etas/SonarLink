// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;
using RestSharp;
using RestSharp.Authenticators;
using SonarLink.API.Models;
using SonarLink.API.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonarLink.API.Services
{

    /// <summary>
    /// Service capable of consuming SonarQube WebAPIs.
    /// </summary>
    public class SonarQubeService : ISonarQubeService
    {
        /// <summary>
        /// Client responsible for making HTTP requests and processing the response.
        /// </summary>
        private IRestClient Client;
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">Underlying HTTP REST client</param>
        public SonarQubeService(IRestClient client = null)
        {
            Client = client ?? new RestClient();
        }
    
        /// <inheritdoc />
        public bool IsConnected { get; private set; } = false;
    
        /// <inheritdoc />
        public Uri BaseUrl => Client.BaseUrl;
    
        /// <inheritdoc />
        public async Task<bool> ConnectAsync(ConnectionInformation connection)
        {
            Client.BaseUrl = new Uri(connection.ServerUrl);
            Client.Authenticator = new HttpBasicAuthenticator(connection.Login, connection.Password);
            Client.UserAgent = "SonarLink";
    
            IsConnected = await InvokeRequestAsync(new ValidateCredentialsRequest()).ConfigureAwait(false);
    
            return IsConnected;
        }
    
        /// <inheritdoc />
        public void Disconnect()
        {
            IsConnected = false;
        }
    
        /// <inheritdoc />
        public Task<List<SonarQubeProject> > GetAllProjectsAsync()
        {
            EnsureIsConnected();
            return InvokeRequestAsync(new GetProjectsRequest());
        }
    
        /// <inheritdoc />
        public Task<List<SonarQubeIssue> > GetProjectIssuesAsync(string projectKey)
        {
            EnsureIsConnected();
    
            // Split the request per issue type to (attempt to) overcome
            // the 10k listing limitation imposed by ElasticSearch/SonarQube
    
            var smells = InvokeRequestAsync(new GetIssuesRequest()
            {
                ProjectKey = projectKey,
                Type = IssueType.CodeSmell
            });
    
            var vulnerabilities = InvokeRequestAsync(new GetIssuesRequest()
            {
                ProjectKey = projectKey,
                Type = IssueType.Vulnerability
            });
        
            var bugs = InvokeRequestAsync(new GetIssuesRequest()
            {
                ProjectKey = projectKey,
                Type = IssueType.Bug
            });
    
            var requests = Task.WhenAll(smells, vulnerabilities, bugs);
                
            return requests.ContinueWith((Task<List<SonarQubeIssue>[] > issues) =>
            {
                // Flatten each issue type category group into 1 list
                return issues.Result.SelectMany(group => group).ToList();
            });
        }
    
        /// <summary>
        /// Invokes the specified (WEB) request and returns its response.
        /// </summary>
        /// <typeparam name="TResponse">The response type.</typeparam>
        /// <returns>Returns the processed response of the request invocation.</returns>
        private Task<TResponse> InvokeRequestAsync<TResponse>(IRequest<TResponse> request)
        {
            return request.InvokeAsync(Client);
        }
    
        /// <summary>
        /// Check whether client authentication was successful. This method is expected to be called prior to any requests
        /// that require authentication.
        /// </summary>
        private void EnsureIsConnected()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("This operation expects the service to be connected.");
            }
        }
    }
}
