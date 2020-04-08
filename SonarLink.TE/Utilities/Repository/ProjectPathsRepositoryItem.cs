// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using System.Collections.Generic;

namespace SonarLink.TE.Utilities.Repository
{
    /// <summary>
    /// POCO model for serializing and persisting Sonar projects and local path associations
    /// </summary>
    public sealed class ProjectPathsRepositoryItem
    {
        /// <summary>
        /// Cache of Sonar projects and local path associations
        /// </summary>
        public Dictionary<string, string> ProjectToPathMap { get; set; }
    }
}
