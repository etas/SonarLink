// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.API.Models
{

    /// <summary>
    /// Describes a SonarQube issue.
    /// </summary>
    public class SonarQubeIssue
    {
        /// <summary>
        /// Key associated with rule that is violated.
        /// </summary>
        public string Rule { get; set; }
    
        /// <summary>
        /// The severity of the issue.
        /// </summary>
        public string Severity { get; set; }
    
        /// <summary>
        /// The component to which the issue pertains. A component can be a portfolio,
        /// project, module, directory or file.
        /// </summary>
        public string Component { get; set; }
    
        /// <summary>
        /// The line number at which violates a given rule.
        /// </summary>
        public int Line { get; set; }
    
        /// <summary>
        /// Description of the issue.
        /// </summary>
        public string Message { get; set; }
    
        /// <summary>
        /// The type of rule violated.
        /// </summary>
        public string Type { get; set; }
    }
}