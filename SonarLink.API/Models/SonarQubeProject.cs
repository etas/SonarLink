// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.API.Models
{

    /// <summary>
    /// Describes a SonarQube project
    /// </summary>
    public class SonarQubeProject
    {
        /// <summary>
        /// Unique key associated with the project.
        /// </summary>
        public string Key { get; set; }
    
        /// <summary>
        /// Name of the project.
        /// </summary>
        public string Name { get; set; }
    }
}