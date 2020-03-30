// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Moq;
using NUnit.Framework;
using RestSharp;
using SonarLink.API.Clients;
using System.Threading.Tasks;

namespace SonarLink.API.UnitTests.Tests
{
    public class AuthenticationClientUnitTests
    {
        /// <summary>
        /// Mock Rest client
        /// </summary>
        private Mock<IRestClient> MockClient = null;

        void SetupResponse(string resource, IRestResponse response)
        {
            MockClient
            .Setup(x => x.ExecuteTaskAsync(It.Is<IRestRequest>(m => m.Resource == resource)))
            .Returns(Task.FromResult(response));
        }

        [SetUp]
        public void SetUp()
        {
            MockClient = new Mock<IRestClient>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        /// <summary>
        /// Assert that: true is returned if authentication is successful.
        /// </summary>
        [Test]
        public void CheckCredentials_Success()
        {
            SetupResponse("api/authentication/validate", RestResponseFactory.Create("{\"valid\": true}"));
            var client = new AuthenticationClient(MockClient.Object);
            Assert.That(client.CheckCredentials("login", "password").Result, Is.True);
        }

        /// <summary>
        /// Assert that: false is returned if authentication fails.
        /// </summary>
        [Test]
        public void CheckCredentials_Failure()
        {
            SetupResponse("api/authentication/validate", RestResponseFactory.Create("{\"valid\": false}"));
            var service = new AuthenticationClient(MockClient.Object);
            Assert.That(service.CheckCredentials("login", "password").Result, Is.False);
        }

    }
}
