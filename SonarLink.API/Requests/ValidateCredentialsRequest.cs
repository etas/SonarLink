// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using RestSharp.Deserializers;

namespace SonarLink.API.Requests
{
    /// <summary>
    /// Request to validate user credentials.
    /// </summary>
    public class ValidateCredentialsRequest : RequestBase<bool>
    {
        /// <inheritdoc />
        protected override string Path => "api/authentication/validate";
    
        /// <inheritdoc />
        protected override bool ParseResponse(IRestResponse response)
        {
            return new JsonDeserializer().Deserialize<ValidateCredentialsResponse>(response).Valid;
        }
    
        /// <summary>
        /// Describes the parsed response with properties matching the deserialized JSON string.
        /// </summary>
        private class ValidateCredentialsResponse
        {
            /// <summary>
            /// Indicates whether credentials are valid or not.
            /// </summary>
            public bool Valid { get; set; }
        }
    }
}