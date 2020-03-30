// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// Interface of a repository for loading and saving data
    /// </summary>
    public interface IRepository<T>
    {
        /// <summary>
        /// POCO data model 
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Persist data
        /// </summary>
        void Save();
    }
}
