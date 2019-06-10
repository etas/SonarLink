// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using System;
using System.Net;

namespace SonarLink.API.Helpers
{

    /// <summary>
    /// Helper utilities for RestSharp components
    /// </summary>
    public static class RestResponseHelper
    {
    
        /// <summary>
        /// Verifies that the response has a 'success' HTTP status code
        /// </summary>
        /// <param name="response">Response object to test</param>
        /// <returns>The input response</returns>
        /// <exception cref="InvalidOperationException">On non-success (200) HTTP status code</exception>
        public static IRestResponse EnsureSuccessStatusCode(this IRestResponse response)
        {
            if (null == response)
            {
                throw new ArgumentNullException(nameof(response));
            }
    
            // Timeout check
            if (response.ResponseStatus == ResponseStatus.TimedOut)
            {
                throw new InvalidOperationException("Timeout occurred: " + response.ErrorMessage);
            }
            else if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new InvalidOperationException("Transmission error occurred: " + response.ErrorMessage);
            }
    
            if (HttpStatusCode.OK != response.StatusCode)
            {
                string info = "Request to " + response.ResponseUri.Authority + response.ResponseUri.AbsolutePath + " failed with status code " + response.StatusCode + ", parameters: "
                              + response.ResponseUri.Query + ", and content: " + response.Content;
    
                throw new InvalidOperationException(info);
            }
    
            return response;
        }
    
    }

}
