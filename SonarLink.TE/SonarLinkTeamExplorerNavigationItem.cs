// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Drawing;
using SonarLink.TE.Model;
using SonarLink.TE.MVVM;
using System.Linq;

namespace SonarLink.TE
{

    /// <summary>
    /// Button/Item displayed in the 'Team Explorer' home page for SonarLink.
    /// Allows user to navigate to their last connected SonarQube instance.
    /// </summary>
    [TeamExplorerNavigationItem("cc9d1e49-2175-49fd-a9c7-1fbae8dc13e9", 1000)]
    public class SonarLinkTeamExplorerNavigationItem : NotifyPropertyChangeSource, ITeamExplorerNavigationItem2
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider">Visual Studio service provider</param>
        /// <param name="manager">SonarQube connection cache/manager</param>
        [ImportingConstructor]
        public SonarLinkTeamExplorerNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider, IConnectionManager manager)
        {
            var service = serviceProvider.GetService(typeof(ITeamExplorer));
            if (service != null)
            {
                TeamExplorer = service as ITeamExplorer;
            }
            
            ConnectionManager = manager;
            IsVisible = ConnectionManager.Connections.Count > 0;
    
            ConnectionManager.CollectionChanged += ConnectionManager_CollectionChanged;
        }
    
        /// <summary>
        /// Visual Studio Team Explorer service
        /// </summary>
        public ITeamExplorer TeamExplorer { get; private set; }
    
        /// <summary>
        /// SonarQube connection cache/manager
        /// </summary>
        public IConnectionManager ConnectionManager { get; set; }
    
        #region ITeamExplorerNavigationItem2
    
        /// <summary>
        /// Visibility status
        /// </summary>
        private bool visible = false;
    
        public bool IsVisible
        {
            get
            {
                return visible;
            }
    
            set
            {
                if (visible != value)
                {
                    visible = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int ArgbColor => 0;

        public object Icon => null;

        public string Text => "SonarLink";

        public Image Image => null;

        public bool IsEnabled => true;

        public void Execute()
        {
            TeamExplorer.NavigateToPage(new Guid(SonarLinkProjectPage.PageId), new SonarLinkProjectPage.PageContext()
            {
                Service = ConnectionManager.Connections.LastOrDefault(),
                Filter = string.Empty
            });
        }
        
        public void Invalidate()
        {
        }
    
        #endregion
    
        /// <summary>
        /// Event handler to react to SonarQube connection additions/removals
        /// </summary>
        /// <param name="sender">Event source (assumed to be ConnectionManager)</param>
        /// <param name="e">Event arguments</param>
        private void ConnectionManager_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsVisible = ConnectionManager.Connections.Count > 0;
        }
    
        #region IDisposable Support
    
        private bool disposedValue = false;
    
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ConnectionManager.CollectionChanged -= ConnectionManager_CollectionChanged;
                }
    
                disposedValue = true;
            }
        }
    
        public void Dispose()
        {
        }
    
        #endregion
    }
}