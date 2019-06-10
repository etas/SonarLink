// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;

namespace SonarLink.API.Models
{

    /// <summary>
    /// Describes the information needed to connect to a SonarQube server.
    /// </summary>
    public class ConnectionInformation
    {
        /// <summary>
        /// The base address for the SonarQube API.
        /// </summary>
        public string ServerUrl { get; }
    
        /// <summary>
        /// The login of the user.
        /// </summary>
        public string Login { get; }
    
        /// <summary>
        /// The password of the user.
        /// </summary>
        public string Password { get; }
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverUrl">Base address for the SonarQube API </param>
        /// <param name="login">User login credentials (or API token)</param>
        /// <param name="password">Login password</param>
        public ConnectionInformation(string serverUrl, string login, string password)
        {
            if (string.IsNullOrEmpty(serverUrl))
            {
                throw new ArgumentException(nameof(serverUrl) + " cannot be null or empty");
            }
    
            if (string.IsNullOrEmpty(login))
            {
                throw new ArgumentException(nameof(login) + " cannot be null or empty");
            }
    
            ServerUrl = serverUrl;
            Login = login;
            Password = password;
        }
    }
}