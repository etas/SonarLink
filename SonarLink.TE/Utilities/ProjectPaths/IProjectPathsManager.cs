// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Manages access to and persistence of the Sonar projects and local path associations
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
        /// <param name="path">Local hint path associated with the specified key, if the latter is found</param>
        /// <returns>Retures true if key exists, false otherwise.</returns>
        bool TryGetValue(string project, out string path);
    }
}
