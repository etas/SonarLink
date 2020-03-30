// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Moq;
using NUnit.Framework;
using RestSharp;
using SonarLink.API.Clients;
using SonarLink.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonarLink.API.UnitTests.Tests
{
    public class ComponentsClientUnitTests
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
        /// Assert that: given a valid response, SonarQube project details are parsed successfully.
        /// </summary>
        [Test]
        public void GetAllProjects_Success()
        {
            SetupResponse("api/components/search", RestResponseFactory.CreateGetProjects_ValidResponse());
            var service = new ComponentsClient(MockClient.Object);

            var expected = new List<SonarQubeProject>()
            {
                new SonarQubeProject {Key="ETAS:INCA:HWA:A1b:LIN", Name="ETAS-INCA-HWA-A1b-LIN"},
                new SonarQubeProject {Key="ETAS:INCA:HWA:A1b:UDS", Name="ETAS-INCA-HWA-A1b-UDS"},
                new SonarQubeProject {Key="ETAS:INCA:HWA:A1b:MT1To2", Name="ETAS-INCA-HWA-A1b-MT1To2"}
            };

            var actual = service.GetAllProjects().Result;

            Assert.That(actual.Count, Is.EqualTo(expected.Count));

            for (var i = 0; i < actual.Count; ++i)
            {
                Assert.That(actual[i].Key, Is.EqualTo(expected[i].Key));
                Assert.That(actual[i].Name, Is.EqualTo(expected[i].Name));
            }
        }

        /// <summary>
        /// Assert that: if no projects exist, the returned list of SonarQube projects is empty.
        /// </summary>
        /// <remarks>
        /// This response is expected for the following scenarios as well:
        /// - client is not authenticated prior to getting list of projects
        /// </remarks>
        [Test]
        public void GetAllProjects_NoProjects()
        {
            SetupResponse("api/components/search", RestResponseFactory.CreateComponentsResponse_NoProjects());
            var service = new ComponentsClient(MockClient.Object);

            var projects = service.GetAllProjects().Result;
            Assert.That(projects.Count, Is.EqualTo(0));
        }
    }
}
