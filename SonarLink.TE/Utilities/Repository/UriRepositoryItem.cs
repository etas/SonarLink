// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;

namespace SonarLink.TE.Utilities.Repository
{
    /// <summary>
    /// POCO model for serializing and persisting SonarQube server URLs
    /// </summary>
    public sealed class UriRepositoryItem
    {
        /// <summary>
        /// SonarQube server URL
        /// </summary>
        public Uri TargetUri { get; set; }
    }
}
