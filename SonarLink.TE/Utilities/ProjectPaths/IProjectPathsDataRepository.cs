// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Interface of a repository for persisting the project paths data
    /// </summary>
    public interface IProjectPathsDataRepository
    {
        /// <summary>
        /// Sonar projects and local path associations
        /// </summary>
        ProjectPathData Data { get; }

        /// <summary>
        /// Persist project paths data
        /// </summary>
        void Save();
    }
}
