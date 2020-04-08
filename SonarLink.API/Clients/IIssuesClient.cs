// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonarLink.API.Clients
{
    /// <summary>
    /// A client for SonarQube issues API.
    /// </summary>
    public interface IIssuesClient
    {
        /// <summary>
        /// Gets a list of all open issues pertaining to a given project.
        /// </summary>
        /// <param name="projectKey">Unique key associated with the project.</param>
        /// <returns>List of open issues pertaining to a given project.</returns>
        Task<List<SonarQubeIssue>> GetProjectIssues(string projectKey);
    }
}
