// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Services;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SonarLink.TE.Model
{
    /// <summary>
    /// Manages a collection of SonarQube connections
    /// </summary>
    public interface IConnectionManager : INotifyCollectionChanged
    {
        /// <summary>
        /// Active SonarQube connections
        /// </summary>
        ICollection<ISonarQubeService> Connections { get; }
    }
}
