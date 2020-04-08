// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using System.Collections.Generic;

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// POCO model for serializing and presisting Sonar projects and local path associations
    /// </summary>
    public sealed class ProjectPathData
    {
        /// <summary>
        /// Cache of Sonar projects and local path associations
        /// </summary>
        public Dictionary<string, string> ProjectPaths { get; set; }
    }
}
