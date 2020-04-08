// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System.ComponentModel.Composition;
using System.IO;

namespace SonarLink.TE.Utilities.Repository
{
    /// <summary>
    /// Repository for persisting SonarQube server URLs
    /// </summary>
    [Export(typeof(IRepository<UriRepositoryItem>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class UriRepository : JsonRepository<UriRepositoryItem>
    {
        /// <summary>
        /// Name of the file the SonarQube server URLs are persisted to
        /// </summary>
        private static readonly string _filename = "targetUri.json";

        /// <summary>
        /// Default constructor
        /// </summary>
        public UriRepository() : base(GetFilePath())
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">File path of the persisted SonarQube server URLs</param>
        internal UriRepository(string filepath) : base(filepath)
        {
        }

        /// <summary>
        /// Gets the file path of the persisted SonarQube server URLs
        /// </summary>
        /// <returns>The file path of the persisted SonarQube server URLs</returns>
        internal static string GetFilePath()
        {
            return Path.Combine(PathUtility.AppDataPath(), _filename);
        }
    }
}
