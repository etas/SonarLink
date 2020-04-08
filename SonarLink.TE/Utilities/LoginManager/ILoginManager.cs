// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Clients;
using SonarLink.TE.Utilities.CredentialsManager;
using SonarLink.TE.Utilities.Repository;
using System;
using System.Threading.Tasks;

namespace SonarLink.TE.Utilities.LoginManager
{
    /// <summary>
    /// Provides services for logging into a SonarQube server.
    /// </summary>
    public interface ILoginManager
    {
        /// <summary>
        /// Attempts to log into a SonarQube server.
        /// </summary>
        /// <param name="baseUrl">The URL of the server.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="ISonarQubeClient"/> if login is successful, null otherwise.</returns>
        Task<ISonarQubeClient> LogInAsync(Uri baseUrl, string userName, string password);

        /// <summary>
        /// Attempts to log into a SonarQube server using saved credentials.
        /// </summary>
        /// <param name="repository">Repository to retrieve server URL.</param>
        /// <param name="credentialsManager">Credentials manager to load credentials.</param>
        /// <returns>A <see cref="ISonarQubeClient"/> if login is successful, null otherwise.</returns>
        Task<ISonarQubeClient> LogInFromRepositoryAsync(IRepository<UriRepositoryItem> repository, ICredentialsManager credentialsManager);
    }
}
