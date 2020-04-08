// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using SonarLink.API.Models;
using SonarLink.API.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonarLink.API.Clients
{
    /// <summary>
    /// A client for SonarQube issues API.
    /// </summary>
    public class IssuesClient : ApiClient, IIssuesClient
    {
        /// <summary>
        /// Initializes a new SonarQube components API client.
        /// </summary>
        /// <param name="restClient">Underlying HTTP rest client</param>
        public IssuesClient(IRestClient restClient) : base(restClient)
        {
        }

        /// <inheritdoc />
        public Task<List<SonarQubeIssue>> GetProjectIssues(string projectKey)
        {
            // Split the request per issue type to (attempt to) overcome the
            // 10k listing limitation imposed by ElasticSearch/SonarQube.

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

            return requests.ContinueWith((Task<List<SonarQubeIssue>[]> issues) =>
            {
                // Flatten each issue type category group into one list.
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
            return request.InvokeAsync(RestClient);
        }
    }
}
