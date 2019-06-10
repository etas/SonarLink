// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using Moq;
using NUnit.Framework;
using SonarLink.API.Services;
using SonarLink.TE.Model;
using System;
using System.Collections.Generic;
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

            Connections = new ConnectionManager();
            NavigationItem = new SonarLinkTeamExplorerNavigationItem(services.Object, Connections);

            // Bootstrap the instance similar to what Visual Studio does
            NavigationItem.Invalidate();

            Events = new List<PropertyChangedEventArgs>();

            NavigationItem.PropertyChanged += (sender, e) => Events.Add(e);
        }

        [TearDown]
        public void TestTearDown()
        {
            NavigationItem.Dispose();
        }

        public SonarLinkTeamExplorerNavigationItem NavigationItem { get; private set; }

        public IConnectionManager Connections { get; private set; }

        public IList<PropertyChangedEventArgs> Events { get; private set; }

        /// <summary>
        /// Assert that: When no connections are available, the navigation item is not visible
        /// </summary>
        [Test]
        public void NotVisibleWhenDisconnected()
        {
            Assert.That(Connections.Connections.Count, Is.EqualTo(0));
            Assert.That(NavigationItem.IsVisible, Is.False);
        }

        /// <summary>
        /// Assert that: When connections are available, the navigation item is visible
        /// </summary>
        [Test]
        public void VisibleWhenConnected()
        {
            var service = new Mock<ISonarQubeService>();
            Connections.Connections.Add(service.Object);

            Assert.That(Events.Count, Is.GreaterThan(0));
            Assert.That(NavigationItem.IsVisible, Is.True);
        }

        /// <summary>
        /// Assert that: When connections are disconnected, the navigation item is not visible
        /// </summary>
        [Test]
        public void NotVisibleAfterDisconnectedConnection()
        {
            var service = new Mock<ISonarQubeService>();
            Connections.Connections.Add(service.Object);

            Assert.That(Events.Count, Is.GreaterThan(0));
            Assert.That(NavigationItem.IsVisible, Is.True);

            Connections.Connections.Remove(service.Object);

            Assert.That(Events.Count, Is.GreaterThan(1));
            Assert.That(NavigationItem.IsVisible, Is.False);
        }
    }
}
