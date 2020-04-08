// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Clients;
using SonarLink.TE.Utilities.CredentialsManager;
using SonarLink.TE.Utilities.Repository;
using System;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace SonarLink.TE.Utilities.LoginManager
{
    /// <summary>
    /// Provides services for logging into a SonarQube server.
    /// </summary>
    [Export(typeof(ILoginManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LoginManager :  ILoginManager
    {
        /// <inheritdoc/>
        public async Task<ISonarQubeClient> LogInAsync(Uri baseUrl, string userName, string password)
        {
            var client = new SonarQubeClient(baseUrl);

            if (await client.Authentication.CheckCredentials(userName, password))
            {
                return client;
            }

            return null;
        }

        /// <inheritdoc/>
        public Task<ISonarQubeClient> LogInFromRepositoryAsync(IRepository<UriRepositoryItem> repository, ICredentialsManager credentialsManager)
        {
            var targetUri = repository.Data.TargetUri;

            if(targetUri is null)
            {
                return Task.FromResult<ISonarQubeClient>(null);
            }

            var credential = credentialsManager.Load(targetUri);

            if(credential is null)
            {
                return Task.FromResult<ISonarQubeClient>(null);
            }

            return LogInAsync(targetUri, credential.Username, credential.Password);
        }
    }
}
