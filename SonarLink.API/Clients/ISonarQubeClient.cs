// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.API.Clients
{
    /// <summary>
    /// A client for the SonarQube API.
    /// </summary>
    public interface ISonarQubeClient
    {
        /// <summary>
        /// Access SonarQube's Authentication API.
        /// </summary>
        IAuthenticationClient Authentication { get; }

        /// <summary>
        /// Access SonarQube's Components API.
        /// </summary>
        IComponentsClient Components { get; }

        /// <summary>
        /// Access SonarQube's Issues API.
        /// </summary>
        IIssuesClient Issues { get; }
    }
}
