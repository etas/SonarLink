// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Manages access to and persistence of the Sonar projects and local path associatons
    /// </summary>
    public interface IProjectPathsManager
    {
        /// <summary>
        /// Add Sonar project and local path
        /// </summary>
        /// <param name="project">Unique key associated with the Sonar project</param>
        /// <param name="path">Local path associated with the Sonar project</param>
        void Add(string project, string path);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="project">Unique key associated with the Sonar project</param>
        /// <param name="path">Local hint path associated with the spcified key, if the latter is found</param>
        void TryGetvalue(string project, out string path);
    }
}
