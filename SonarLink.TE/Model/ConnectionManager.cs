// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using SonarLink.API.Services;

namespace SonarLink.TE.Model
{
    /// <summary>
    /// Default IConnectionManager implementation
    /// </summary>
    /// <remarks>Intended to be provisioned via MEF to allow for sharing across Team Explorer components</remarks>
    [Export(typeof(IConnectionManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConnectionManager : IConnectionManager
    {
        /// <summary>
        /// Active SonarQube connections
        /// </summary>
        private ObservableCollection<ISonarQubeService> _connections = new ObservableCollection<ISonarQubeService>();
    
        #region IConnectionManager
    
        public ICollection<ISonarQubeService> Connections => _connections;
    
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                _connections.CollectionChanged += value;
            }
    
            remove
            {
                _connections.CollectionChanged -= value;
            }
        }
    
        #endregion
    }
}
