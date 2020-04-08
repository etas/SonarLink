// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Implementation of IProjectPathsManager
    /// </summary>
    public sealed class ProjectPathsManager : IProjectPathsManager
    {
        /// <summary>
        /// Repository for persisting the project paths data
        /// </summary>
        private readonly IProjectPathsDataRepository _repository;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectPathsManager() :
            this(new ProjectPathsDataRepository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository">Repository for persisting the project paths data</param>
        internal ProjectPathsManager(IProjectPathsDataRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Add Sonar project and local path
        /// </summary>
        /// <param name="project">Unique key associated with the Sonar project</param>
        /// <param name="path">Local path associated with the Sonar project</param>
        public void Add(string project, string path)
        {
            if(string.IsNullOrEmpty(project) || string.IsNullOrEmpty(path))
            {
                return;
            }

            _repository.Data.ProjectPaths[project] = path;
            _repository.Save();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="project">Unique key associated with the Sonar project</param>
        /// <param name="path">Local hint path associated with the spcified key, if the latter is found</param>
        public void TryGetvalue(string project, out string path)
        {
            path = null;

            if (string.IsNullOrEmpty(project))
            {
                return;
            }

            _repository.Data.ProjectPaths?.TryGetValue(project, out path);
        }
    }
}
