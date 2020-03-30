// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using NUnit.Framework;
using SonarLink.TE.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class ConnectionInfoDialogViewModelTests
    {
        /// <summary>
        /// Test setup
        /// </summary>
        [SetUp]
        public void TestSetup()
        {
            Model = new ConnectionInfoDialogViewModel();
            Events = new List<PropertyChangedEventArgs>();

            Model.PropertyChanged += (sender, e) => Events.Add(e);
        }

        /// <summary>
        /// ConnectionInfoDialogViewModel instance under test
        /// </summary>
        private ConnectionInfoDialogViewModel Model { get; set; }

        /// <summary>
        /// Accumulated PropertyChangedEventArgs for Model
        /// </summary>
        private IList<PropertyChangedEventArgs> Events { get; set; }

        /// <summary>
        /// Extracts the property names from the PropertyChangedEventArgs values
        /// </summary>
        /// <param name="value">Enumerable of PropertyChangedEventArgs</param>
        /// <returns>A collection which identifies solely the property names</returns>
        private IEnumerable<string> PropertyNames(IEnumerable<PropertyChangedEventArgs> value)
        {
            return value.Select(e => e.PropertyName);
        }

        /// <summary>
        /// Assert that: Ill-formed URLs are not accepted and users are notified of such scenarios
        /// </summary>
        /// <param name="value">URL</param>
        [TestCase("    ")]
        [TestCase("NotAURI")]
        [TestCase("ssh://pi@localhost:/tmp")]
        public void InvalidServerURL(string value)
        {
            Model.ServerUrl = value;

            Assert.That(PropertyNames(Events), Is.EquivalentTo(new[] { "ServerUrl", "IsServerUrlValid", "ShowWarning" }));

            Assert.That(Model.IsServerUrlValid, Is.False);
            Assert.That(Model.ShowWarning, Is.True);
        }

        /// <summary>
        /// Assert that: Well-formed URLs are accepted
        /// </summary>
        /// <param name="value">URL</param>
        [TestCase("http://sonar.mydomain.com:9000")]
        [TestCase("http://sonar.mydomain.com")]
        [TestCase("https://sonar.mydomain.com")]
        public void ValidServerURL(string value)
        {
            Model.ServerUrl = value;

            Assert.That(PropertyNames(Events), Is.EquivalentTo(new[] { "ServerUrl", "IsServerUrlValid", "ShowWarning" }));

            Assert.That(Model.IsServerUrlValid, Is.True);
            Assert.That(Model.ShowWarning, Is.False);
        }

        /// <summary>
        /// Assert that: A password hint is shown when the dialog is launched
        /// </summary>
        public void ShowPasswordHintInitially()
        {
            Assert.That(Model.ShowPasswordHintText, Is.True);
        }

        /// <summary>
        /// Assert that: A password hint is shown in case of an empty/null password field
        /// </summary>
        /// <param name="value">password</param>
        /// <returns>true if the password hint should be shown, false otherwise</returns>
        [TestCase("password", ExpectedResult = false)]
        [TestCase(null, ExpectedResult = true)]
        public bool ShowPasswordHint(string value)
        {
            Model.Password = value;

            Assert.That(PropertyNames(Events), Is.EquivalentTo(new[] { "Password", "ShowPasswordHintText" }));

            return Model.ShowPasswordHintText;
        }
    }
}
