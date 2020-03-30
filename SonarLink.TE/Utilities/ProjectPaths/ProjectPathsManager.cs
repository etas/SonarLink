// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using SonarLink.TE.Utilities.Repository;
using System.ComponentModel.Composition;

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Implementation of IProjectPathsManager
    /// </summary>
    [Export(typeof(IProjectPathsManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class ProjectPathsManager : IProjectPathsManager
    {
        /// <summary>
        /// Repository for persisting the project paths data
        /// </summary>
        private readonly IRepository<ProjectPathsRepositoryItem> _repository;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectPathsManager() :
            this(new ProjectPathsRepository())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository">Repository for persisting the project paths data</param>
        internal ProjectPathsManager(IRepository<ProjectPathsRepositoryItem> repository)
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

            _repository.Data.ProjectToPathMap[project] = path;
            _repository.Save();
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="project">Unique key associated with the Sonar project</param>
        /// <param name="path">Local hint path associated with the specified key, if the latter is found</param>
        /// <returns>Retures true if key exists, false otherwise.</returns>
        public bool TryGetValue(string project, out string path)
        {
            path = null;

            if (string.IsNullOrEmpty(project) || _repository.Data.ProjectToPathMap is null)
            {
                return false;
            }

            return _repository.Data.ProjectToPathMap.TryGetValue(project, out path);
        }
    }
}
