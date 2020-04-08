// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonarLink.TE.Model
{
    /// <summary>
    /// Manages the configured <see cref="ISonarQubeClient"/> instances.
    /// </summary>
    public interface IClientManager
    {
        /// <summary>
        /// Gets a collection containing authenticated client instances.
        /// </summary>
        ICollection<ISonarQubeClient> Clients { get; }

        /// <summary>
        /// Attempts to log into a SonarQube server.
        /// </summary>
        /// <param name="baseUrl">The URL of the server.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="ISonarQubeClient"/> if login is successful, null otherwise.</returns>
        Task<ISonarQubeClient> LogInAsync(Uri serverUrl, string username, string password);

        /// <summary>
        /// Gets a collection containing authenticated client instances whose credentials
        /// were loaded from a repository.
        /// </summary>
        /// <returns>
        /// A task returning a collection of authenticated clients.
        /// </returns>
        Task<ICollection<ISonarQubeClient>> GetLoadedClientsAsync();
        
        /// <summary>
        /// Logs out all client instances.
        /// </summary>
        void LogOut();
    }
}
