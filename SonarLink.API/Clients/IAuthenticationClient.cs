// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System.Threading.Tasks;

namespace SonarLink.API.Clients
{
    /// <summary>
    /// A client for SonarQube authorization API.
    /// </summary>
    public interface IAuthenticationClient
    {
        /// <summary>
        /// Check user credentials.
        /// </summary>
        /// <param name="username">Client's user name.</param>
        /// <param name="password">Client's password.</param>
        /// <returns>True if user credentials are valid, false otherwise.</returns>
        Task<bool> CheckCredentials(string username, string password);
    }
}
