// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonarLink.API.Services
{

    /// <summary>
    /// Interface for service capable of consuming SonarQube WebAPIs.
    /// </summary>
    public interface ISonarQubeService
    {
        /// <summary>
        /// Indicates whether client authentication has been performed successfully.
        /// </summary>
        bool IsConnected { get; }
    
        /// <summary>
        /// Indicates the SonarQube server (base) URL
        /// </summary>
        Uri BaseUrl { get; }
    
        /// <summary>
        /// Attempts to authenticate the provided client credentials.
        /// </summary>
        /// <param name="connection">The information required to connect to a SonarQube server.</param>
        /// <returns>True if client authentication was successful, false otherwise.</returns>
        Task<bool> ConnectAsync(ConnectionInformation connection);
    
        /// <summary>
        /// Disconnects from SonarQube server.
        /// </summary>
        void Disconnect();
    
        /// <summary>
        /// Gets a list of all projects.
        /// </summary>
        /// <returns>List of projects currently on the SonarQube server.</returns>
        Task<List<SonarQubeProject> > GetAllProjectsAsync();
    
        /// <summary>
        /// Gets a list of all open issues pertaining to a given project.
        /// </summary>
        /// <param name="projectKey">Unique key associated with the project.</param>
        /// <returns>List of open issues pertaining to a given project.</returns>
        Task<List<SonarQubeIssue> > GetProjectIssuesAsync(string projectKey);
    }
}