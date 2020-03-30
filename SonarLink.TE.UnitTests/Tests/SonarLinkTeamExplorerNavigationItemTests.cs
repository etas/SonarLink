// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using Moq;
using NUnit.Framework;
using SonarLink.API.Clients;
using SonarLink.TE.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class SonarLinkTeamExplorerNavigationItemTests
    {
        [SetUp]
        public void TestSetup()
        {
            var explorer = new Mock<ITeamExplorer>();
            var services = new Mock<IServiceProvider>();

            services.
                Setup(i => i.GetService(It.Is<Type>(type => type == typeof(ITeamExplorer)))).
                Returns(explorer.Object);

            ClientManager = new Mock<IClientManager>();
            ClientManager.SetupGet(i => i.Clients).Returns(new Collection<ISonarQubeClient>());

            NavigationItem = new SonarLinkTeamExplorerNavigationItem(services.Object, ClientManager.Object);

            // Bootstrap the instance similar to what Visual Studio does
            NavigationItem.Invalidate();
        }

        [TearDown]
        public void TestTearDown()
        {
            NavigationItem.Dispose();
        }

        public SonarLinkTeamExplorerNavigationItem NavigationItem { get; private set; }

        public Mock<IClientManager> ClientManager { get; private set; }

        /// <summary>
        /// Assert that: When client is not signed in, the navigation item is not visible
        /// </summary>
        [Test]
        public void NotVisibleNoClientsAreSignedIn()
        {
            Assert.That(ClientManager.Object.Clients.Count, Is.EqualTo(0));
            Assert.That(NavigationItem.IsVisible, Is.False);
        }

        /// <summary>
        /// Assert that: When client is signed in, the navigation item is visible
        /// </summary>
        [Test]
        public void VisibleClientIsSignedIn()
        {
            var client = new Mock<ISonarQubeClient>();

            ClientManager.Object.Clients.Add(client.Object);
            Assert.That(NavigationItem.IsVisible, Is.True);
        }

        /// <summary>
        /// Assert that: When all client signs out, the navigation item is not visible
        /// </summary>
        [Test]
        public void NotVisibleAfterClientSignsOut()
        {
            var client = new Mock<ISonarQubeClient>();

            ClientManager.Object.Clients.Add(client.Object);
            Assert.That(NavigationItem.IsVisible, Is.True);

            ClientManager.Object.Clients.Remove(client.Object);
            Assert.That(NavigationItem.IsVisible, Is.False);
        }
    }
}
