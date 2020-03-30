using Moq;
using NUnit.Framework;
using SonarLink.API.Clients;
using SonarLink.TE.Model;
using SonarLink.TE.UnitTests.Utilities;
using SonarLink.TE.Utilities;
using SonarLink.TE.Utilities.CredentialsManager;
using SonarLink.TE.Utilities.LoginManager;
using SonarLink.TE.Utilities.Repository;
using System;
using System.Threading.Tasks;

namespace SonarLink.TE.UnitTests.Tests
{
    class ClientManagerTests
    {
        /// <summary>
        /// Test setup
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            UrlRepository = new InMemoryUriRepository();
            CredentialsManager = new InMemoryCredentialsManager();
            LoginManager = new Mock<ILoginManager>();
        }

        /// <summary>
        /// Test tear-down
        /// </summary>
        [TearDown]
        public void TestTearDown()
        {
        }

        /// <summary>
        /// In memory server URL repository.
        /// </summary>
        private InMemoryUriRepository UrlRepository { get; set; }

        /// <summary>
        /// In memory client credentials manager.
        /// </summary>
        private InMemoryCredentialsManager CredentialsManager { get; set; }

        /// <summary>
        /// Mock login manager
        /// </summary>
        private Mock<ILoginManager> LoginManager { get; set; }

        /// <summary>
        /// Assert that: client instances loaded from repository are added
        /// to the collection of authenticated clients.
        /// </summary>
        [Test]
        public async Task GetLoadedClientsFromRepository()
        {
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(new SonarQubeClient(null) as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);

            Assert.That(clientManager.Clients.Count, Is.EqualTo(1));

            // Assert that the same collection is returned 
            var clients = await clientManager.GetLoadedClientsAsync();
            Assert.That(clients.Count, Is.EqualTo(1));

            // Assert that the client instances are loaded from repository only once.
            LoginManager.Verify(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>()), Times.Once());
        }

        /// <summary>
        /// Assert that: if no client instances are loaded from repository,
        /// the collection of authenticated clients is empty.
        /// </summary>
        [Test]
        public async Task NoClientsLoadedFromRepository()
        {
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);

            Assert.That(clientManager.Clients, Is.Empty);

            // Assert that the same collection is returned 
            var clients = await clientManager.GetLoadedClientsAsync();
            Assert.That(clients, Is.Empty);

            // Assert that the client instances are loaded from repository only once.
            LoginManager.Verify(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>()), Times.Once());
        }

        /// <summary>
        /// Assert that: a successfully authenticated client is returned if the login attempt
        /// is successful.
        /// </summary>
        [Test]
        public async Task ReturnAuthenticatedClient()
        {
            LoginManager.
                Setup(i => i.LogInAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns(Task.FromResult(new SonarQubeClient(null) as ISonarQubeClient));
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);
            var client = await clientManager.LogInAsync(new Uri("https://server.com/"), "username", "password");

            Assert.That(client, Is.Not.Null);
        }

        /// <summary>
        /// Assert that: a null client is returned when a login attempt fails.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task ReturnNullWhenLoginFails()
        {
            LoginManager.
                Setup(i => i.LogInAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns(Task.FromResult(null as ISonarQubeClient));
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);
            var client = await clientManager.LogInAsync(new Uri("https://server.com/"), "username", "password");

            Assert.That(client, Is.Null);
        }

        /// <summary>
        /// Assert that: a successfully authenticated client is added to the collection.
        /// </summary>
        [Test]
        public async Task AddAuthenticatedClientToCollection()
        {
            LoginManager.
                Setup(i => i.LogInAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns(Task.FromResult(new SonarQubeClient(null) as ISonarQubeClient));
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);

            Assert.That(clientManager.Clients, Is.Empty);

            _ = await clientManager.LogInAsync(new Uri("https://server.com/"), "username", "password");

            Assert.That(clientManager.Clients.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Assert that: no client is added to the collection when a login attempt fails.
        /// </summary>
        [Test]
        public async Task NoClientIsAddedWhenLoginFails()
        {
            LoginManager.
                Setup(i => i.LogInAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns(Task.FromResult(null as ISonarQubeClient));
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);
            _ = await clientManager.LogInAsync(new Uri("https://server.com/"), "username", "password");

            Assert.That(clientManager.Clients, Is.Empty);
        }

        /// <summary>
        /// Assert that: a successfully authenticated client is saved to repository.
        /// </summary>
        [Test]
        public async Task SaveAuthenticatedClientToRepository()
        {
            LoginManager.
                Setup(i => i.LogInAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns(Task.FromResult(new SonarQubeClient(null) as ISonarQubeClient));
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);

            var serverUrl = new Uri("https://server.com/");
            _ = await clientManager.LogInAsync(serverUrl, "username", "password");

            Assert.That(UrlRepository.Data.TargetUri, Is.EqualTo(serverUrl));
            Assert.That(CredentialsManager._credentials.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Assert that: no client is saved to repository when a login attempt fails.
        /// </summary>
        [Test]
        public async Task NoClientIsSavedWhenLoginFails()
        {
            LoginManager.
                Setup(i => i.LogInAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns(Task.FromResult(null as ISonarQubeClient));
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);
            _ = await clientManager.LogInAsync(new Uri("https://server.com/"), "username", "password");

            Assert.That(UrlRepository.Data.TargetUri, Is.Null);
            Assert.That(CredentialsManager._credentials, Is.Empty);
        }

        /// <summary>
        /// Assert that: on log out, the authenticated client is removed from collection.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task LogOutRemovesClientFromCollection()
        {
            LoginManager.
                Setup(i => i.LogInAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns(Task.FromResult(new SonarQubeClient(null) as ISonarQubeClient));
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);
            _ = await clientManager.LogInAsync(new Uri("https://server.com/"), "username", "password");

            Assert.That(clientManager.Clients.Count, Is.EqualTo(1));

            clientManager.LogOut();

            Assert.That(clientManager.Clients, Is.Empty);
        }

        /// <summary>
        /// Assert that: on log out, client credentials are removed form repository.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task LogOutRemovesClientFromRepository()
        {
            LoginManager.
                Setup(i => i.LogInAsync(It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<string>())).
                Returns(Task.FromResult(new SonarQubeClient(null) as ISonarQubeClient));
            LoginManager.
                Setup(i => i.LogInFromRepositoryAsync(It.IsAny<IRepository<UriRepositoryItem>>(), It.IsAny<ICredentialsManager>())).
                Returns(Task.FromResult(null as ISonarQubeClient));

            var clientManager = new ClientManager(UrlRepository, CredentialsManager, LoginManager.Object);

            var serverUrl = new Uri("https://server.com/");
            _ = await clientManager.LogInAsync(serverUrl, "username", "password");

            Assert.That(UrlRepository.Data.TargetUri, Is.EqualTo(serverUrl));
            Assert.That(CredentialsManager._credentials.Count, Is.EqualTo(1));

            clientManager.LogOut();

            Assert.That(UrlRepository.Data.TargetUri, Is.Null);
            Assert.That(CredentialsManager._credentials, Is.Empty);
        }
    }
}
