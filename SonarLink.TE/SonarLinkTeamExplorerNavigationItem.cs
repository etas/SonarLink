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
using SonarLink.TE.Utilities.CredentialsManager;

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
        /// Visual Studio Team Explorer service.
        /// </summary>
        readonly ITeamExplorer _teamExplorer;

        /// <summary>
        /// Manages the configured <see cref="ISonarQubeClient"/> instances.
        /// </summary>
        readonly IClientManager _clientManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceProvider">Visual Studio service provider.</param>
        /// <param name="clientManager">Manages the configured <see cref="ISonarQubeClient"/> instances.</param>
        [ImportingConstructor]
        public SonarLinkTeamExplorerNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider, IClientManager clientManager)
        {
            var service = serviceProvider.GetService(typeof(ITeamExplorer));
            if (service != null)
            {
                _teamExplorer = service as ITeamExplorer;
            }

            _clientManager = clientManager;
        }
    
        #region ITeamExplorerNavigationItem2
    
        public bool IsVisible => (_clientManager.Clients.Count > 0);

        public int ArgbColor => 0;

        public object Icon => null;

        public string Text => "SonarLink";

        public Image Image => null;

        public bool IsEnabled => true;

        public void Execute()
        {
            _teamExplorer.NavigateToPage(new Guid(SonarLinkProjectPage.PageId), new SonarLinkProjectPage.PageContext()
            {
                Client = _clientManager.Clients.LastOrDefault(),
                Filter = string.Empty
            });
        }
        
        public void Invalidate()
        {
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}