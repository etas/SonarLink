// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using System.Threading.Tasks;

namespace SonarLink.API.Requests
{

    /// <summary>
    /// Base request interface, not to be directly implemented.
    /// </summary>
    public interface IRequest
    {
    }
    
    /// <summary>
    /// Request interface to be implemented on request classes.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public interface IRequest<TResponse> : IRequest
    {
        /// <summary>
        /// Invokes the request using the provided client.
        /// </summary>
        /// <typeparam name="client">Client responsible for making http requests and processing the response.</typeparam>
        /// <returns>The processed response.</returns>
        Task<TResponse> InvokeAsync(IRestClient client);
    }

}