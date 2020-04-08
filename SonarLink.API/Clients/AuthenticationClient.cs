// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using RestSharp.Authenticators;
using SonarLink.API.Requests;
using System.Threading.Tasks;

namespace SonarLink.API.Clients
{
    /// <summary>
    /// A client for SonarQube authorization API.
    /// </summary>
    public class AuthenticationClient : ApiClient, IAuthenticationClient
    {
        /// <summary>
        /// Initializes a new SonarQube authorization API client.
        /// </summary>
        /// <param name="restClient">Underlying HTTP rest client</param>
        public AuthenticationClient(IRestClient restClient) : base(restClient)
        {
        }

        /// <summary>
        /// Check user credentials.
        /// </summary>
        /// <param name="username">Client's user name.</param>
        /// <param name="password">Client's password.</param>
        /// <returns>True if user credentials are valid, false otherwise.</returns>
        public Task<bool> CheckCredentials(string username, string password)
        {
            RestClient.Authenticator = new HttpBasicAuthenticator(username, password);
            return new ValidateCredentialsRequest().InvokeAsync(RestClient);
        }
    }
}
