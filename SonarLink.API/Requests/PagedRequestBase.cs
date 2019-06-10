// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using SonarLink.API.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonarLink.API.Requests
{

    /// <summary>
    /// Specialized extension of RequestBase<TResponseItem> that automatically downloads all pages
    /// from the server. Paginated request classes are expected to inherit from this class.
    /// </summary>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public abstract class PagedRequestBase<TResponse> : RequestBase<List<TResponse>>
    {
        /// <summary>
        /// Page query string parameter identifier.
        /// </summary>
        private static string PageParameter => "p";
    
        /// <summary>
        /// Page size query string parameter identifier.
        /// </summary>
        private static string PageSizeParameter => "ps";
    
        /// <summary>
        /// The first page number.
        /// </summary>
        private const int FirstPage = 1;
    
        /// <summary>
        /// The maximum page size that may be requested.
        /// </summary>
        protected const int MaximumPageSize = 500;
    
        /// <summary>
        /// The request page number.
        /// </summary>
        protected int Page { get; set; } = FirstPage;
    
        /// <inheritdoc />
        public override async Task<List<TResponse>> InvokeAsync(IRestClient client)
        {
            int page = Page;
            bool lastPage = false;
            var result = new List<TResponse>();
    
            // Retain any accumulated issues until either
            // all pages are consumed or an exception occurs
            while (!lastPage)
            {
                var response = await InvokePageRequestAsync(client, page, MaximumPageSize).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
    
                var parsedResponse = ParseResponse(response);
                result.AddRange(parsedResponse);
    
                // Stop sending further requests if all the elements have been parsed
                // or the SonarQube /ElasticSearch 10K limit is reached
                lastPage = (parsedResponse.Count < MaximumPageSize) || (result.Count >= 10000);
                ++page;
            }
    
            return result;
        }
    
        /// <summary>
        /// Invokes a page request for the given page of the specified size using the provided client.
        /// </summary>
        /// <param name="client">Client responsible for making http requests and processing the response.</typeparam>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="maxPageSize">Maximum page size.</param>
        /// <returns>The processed response.</returns>
        protected Task<IRestResponse> InvokePageRequestAsync(IRestClient client, int pageNumber, int maxPageSize = MaximumPageSize)
        {
            var request = GenerateRequest().
                            AddOrUpdateParameter(PageParameter, pageNumber).
                            AddOrUpdateParameter(PageSizeParameter, maxPageSize);
    
            return client.ExecuteTaskAsync(request);
        }
    }

}