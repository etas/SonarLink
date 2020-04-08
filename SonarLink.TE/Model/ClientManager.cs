// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.Alm.Authentication;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using SonarLink.API.Clients;
using SonarLink.TE.Utilities;
using SonarLink.TE.Utilities.CredentialsManager;
using SonarLink.TE.Utilities.LoginManager;
using SonarLink.TE.Utilities.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace SonarLink.TE.Model
{
    /// <summary>
    /// Manages the configured <see cref="ISonarQubeClient"/> instances.
    /// </summary>
    [Export(typeof(IClientManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ClientManager : IClientManager
    {
        /// <summary>
        /// Repository to save and load server URLs.
        /// </summary>
        readonly IRepository<UriRepositoryItem> _repository;

        /// <summary>
        /// Credentials manager to save and load client credentials.
        /// </summary>
        readonly ICredentialsManager _credentialsManager;

        /// <summary>
        /// Provides services for logging into a SonarQube server.
        /// </summary>
        readonly ILoginManager _loginManager;

        /// <summary>
        /// Collection of lazily initialized authenticated client instances.
        /// </summary>
        readonly AsyncLazy<ICollection<ISonarQubeClient>> _clients;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repository">Repository to save and load server URLs.</param>
        /// <param name="credentialsManager">Credentials manager to save and load client credentials.</param>
        [ImportingConstructor]
        public ClientManager(IRepository<UriRepositoryItem> repository, ICredentialsManager credentialsManger, ILoginManager loginManager)
        {
            _repository = repository;
            _credentialsManager = credentialsManger;
            _loginManager = loginManager;

            JoinableTaskContext joinableTaskContext = null;

            if(ThreadHelper.CheckAccess())
            {
                joinableTaskContext = ThreadHelper.JoinableTaskContext;
            }

            _clients = new AsyncLazy<ICollection<ISonarQubeClient>>(LoadClientsAsync, joinableTaskContext?.Factory);
        }

        /// <inheritdoc/>
        public ICollection<ISonarQubeClient> Clients => _clients.GetValue();

        /// <inheritdoc/>
        public async Task<ISonarQubeClient> LogInAsync(Uri serverUrl, string username, string password)
        {
            var client = await _loginManager.LogInAsync(serverUrl, username, password);

            if(null != client)
            {
                _credentialsManager.Save(serverUrl, new Credential(username, password));
                _repository.Data.TargetUri = serverUrl;
                _repository.Save();
                Clients.Add(client);
            }

            return client;
        }

        /// <inheritdoc/>
        public async Task<ICollection<ISonarQubeClient>> GetLoadedClientsAsync()
        {
            return await _clients.GetValueAsync();
        }

        /// <inheritdoc/>
        public void LogOut()
        {
            Clients.Clear();
            _credentialsManager.Delete(_repository.Data.TargetUri);
            _repository.Data.TargetUri = null;
            _repository.Save();
        }

        /// <summary>
        /// Load client credentials from repository and use them to
        /// log into a SonarQube server.
        /// </summary>
        /// <returns>A task returning a collection of authenticated clients.</returns>
        async Task<ICollection<ISonarQubeClient>> LoadClientsAsync()
        {
            var result = new Collection<ISonarQubeClient>();
            var client = await _loginManager.LogInFromRepositoryAsync(_repository, _credentialsManager);

            if(null != client)
            {
                result.Add(client);
            }

            return result;
        }
    }
}
