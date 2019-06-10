// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using NUnit.Framework;
using SonarLink.API.Models;
using System;

namespace SonarLink.API.UnitTests
{
    public class ConnectionInformationUnitTests
    {
    
        /// <summary>
        /// Assert that an exception is thrown if 'serverUrl' parameter is null or empty.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        public void Empty_Server_Url(string serverUrl)
        {
            var exception = Assert.Throws<ArgumentException>(() => new ConnectionInformation(serverUrl, "login", "password"));
            Assert.That(exception.Message, Is.EqualTo("serverUrl cannot be null or empty"));
    
        }
    
        /// <summary>
        /// Assert that an exception is thrown if 'login' parameter  is null or empty.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        public void Empty_Login(string login)
        {
            var exception = Assert.Throws<ArgumentException>(() => new ConnectionInformation("server_url", login, "password"));
            Assert.That(exception.Message, Is.EqualTo("login cannot be null or empty"));
        }
    
        /// <summary>
        /// Assert that no exception is thrown if 'password' parameter  is null or empty. A token may be used
        /// for authentication without any password.
        /// </summary>
        [TestCase(null)]
        [TestCase("")]
        public void Empty_Password(string password)
        {
            Assert.That(() => new ConnectionInformation("server_url", "login", password), Throws.Nothing);
        }
    }
}
