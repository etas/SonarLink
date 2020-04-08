// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using System.Collections.Generic;
using System.IO;

namespace SonarLink.TE.Utilities.Repository
{
    /// <summary>
    /// Repository for persisting project paths data
    /// </summary>
    public class ProjectPathsRepository : JsonRepository<ProjectPathsRepositoryItem>
    {
        /// <summary>
        /// Name of the file the project paths data is persisted to
        /// </summary>
        private static readonly string _filename = "projectPaths.json";

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectPathsRepository() : 
            this(GetFilePath())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">File path of the persisted project paths data</param>
        internal ProjectPathsRepository(string filepath) : base(filepath)
        {
            if (Data.ProjectToPathMap is null)
            {
                Data.ProjectToPathMap = new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Gets the file path of the persisted project paths data
        /// </summary>
        /// <returns>The file path of the persisted project paths data</returns>
        internal static string GetFilePath()
        {
            return Path.Combine(PathUtility.AppDataPath(), _filename);
        }
    }
}
