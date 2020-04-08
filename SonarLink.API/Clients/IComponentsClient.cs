// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonarLink.API.Clients
{
    /// <summary>
    /// A client for SonarQube components API.
    /// </summary>
    public interface IComponentsClient
    {
        /// <summary>
        /// Gets a list of all projects.
        /// </summary>
        /// <returns>List of projects currently on the SonarQube server.</returns>
        Task<List<SonarQubeProject>> GetAllProjects();
    }
}
