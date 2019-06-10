// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using SonarLink.TE.MVVM;
using SonarLink.TE.Model;
using SonarLink.TE.View;
using SonarLink.TE.ViewModel;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace SonarLink.TE
{
    /// <summary>
    /// A Team Explorer section, part of the system-default 'Connect'
    /// page which allows users to connect to SonarQube instances
    /// </summary>
    [TeamExplorerSection("6588C6F3-D20C-48FE-89A7-00460152CEF0", TeamExplorerPageIds.Connect, 20)]
    public class SonarLinkConnectionSection : NotifyPropertyChangeSource, ITeamExplorerSection
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// This constructor is resolved through the MEF framework via the catalog and
        /// composition system instances which are part of the Visual Studio instance
        /// </remarks>
        /// <param name="serviceProvider">Visual Studio service provider</param>
        /// <param name="connections">SonarQube connections</param>
        [ImportingConstructor]
        public SonarLinkConnectionSection([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider, IConnectionManager connections)
        {
            var teamExplorer = serviceProvider.GetService(typeof(ITeamExplorer)) as ITeamExplorer;

            SectionContent = new ConnectPageView()
            {
                DataContext = new ConnectPageViewModel(teamExplorer, connections)
            };

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        /// <summary>
        /// Property change handler primarily responsible for changing the busy state of the Team Explorer
        /// page in case an attempt to establish a connection is ongoing.
        /// </summary>
        /// <param name="sender">Source command which triggered the event</param>
        /// <param name="e">Property changed event arguments</param>
        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsAttemptingToConnect))
            {
                IsBusy = ViewModel.IsAttemptingToConnect;
            }
        }

        /// <summary>
        /// Section UI View
        /// </summary>
        private ConnectPageView View => (ConnectPageView)SectionContent;

        /// <summary>
        /// Section UI ViewModel
        /// </summary>
        private ConnectPageViewModel ViewModel => (ConnectPageViewModel)View.DataContext;

        #region ITeamExplorerSection

        public string Title => "SonarLink";

        public object SectionContent { get; }

        /// <summary>
        /// Visibility status
        /// </summary>
        private bool visible = true;
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

        /// <summary>
        /// Expanded (section) status
        /// </summary>
        private bool expanded = true;
        public bool IsExpanded
        {
            get
            {
                return expanded;
            }

            set
            {
                if (expanded != value)
                {
                    expanded = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Busy status
        /// </summary>
        private bool busy = false;
        public bool IsBusy
        {
            get
            {
                return busy;
            }

            set
            {
                if (busy != value)
                {
                    busy = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void Cancel()
        {
        }

        public object GetExtensibilityService(Type serviceType)
        {
            return null;
        }

        public void Initialize(object sender, SectionInitializeEventArgs e)
        {
        }

        public void Loaded(object sender, SectionLoadedEventArgs e)
        {
        }

        public void Refresh()
        {
        }

        public void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #endregion
    }
}
