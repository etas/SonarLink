// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using SonarLink.API.Helpers;
using System.Threading.Tasks;

namespace SonarLink.API.Requests
{

    /// <summary>
    /// Abstract implementation of IRequest<TResponse> that automatically deserializes
    /// and parses the returned string response.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public abstract class RequestBase<TResponse> : IRequest<TResponse>
    {
        /// <summary>
        /// The relative path of the request.
        /// </summary>
        protected abstract string Path { get; }
    
        /// <summary>
        /// Deserialize and parse the response.
        /// </summary>
        /// <param name="response">The response to be processed.</param>
        /// <returns>The processed response.</returns>
        protected abstract TResponse ParseResponse(IRestResponse response);
    
        /// <summary>
        /// Generates the request.
        /// </summary>
        /// <returns>Class instance that represents the request.</returns>
        protected virtual IRestRequest GenerateRequest()
        {
            return new RestRequest(Path, Method.GET);
        }
    
        /// <inheritdoc />
        public virtual async Task<TResponse> InvokeAsync(IRestClient client)
        {
            var request = GenerateRequest();
            var response = await client.ExecuteTaskAsync(request).ConfigureAwait(false);
                
            response.EnsureSuccessStatusCode();
    
            return ParseResponse(response);
        }
    }

}