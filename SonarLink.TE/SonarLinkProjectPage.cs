// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableManager;
using SonarLink.API.Services;
using SonarLink.TE.MVVM;
using SonarLink.TE.Model;
using SonarLink.TE.View;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using System.Windows.Input;
using SonarLink.TE.ViewModel;
using SonarLink.TE.ErrorList;

namespace SonarLink.TE
{
    /// <summary>
    /// Team Explorer page listing SonarQube projects of a specific SonarQube server instance
    /// </summary>
    [TeamExplorerPage(SonarLinkProjectPage.PageId)]
    class SonarLinkProjectPage : NotifyPropertyChangeSource, ITeamExplorerPage
    {
        /// <summary>
        /// Team Explorer page GUID
        /// </summary>
        public const string PageId = "1788bec6-2d48-4b79-bb9c-3688302d23da";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider">Visual Studio service provider</param>
        /// <param name="connections">(Shared) SonarQube connection cache/manager</param>
        /// <param name="issues">(Shared) SonarQube error list cache</param>
        [ImportingConstructor]
        public SonarLinkProjectPage([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider, IConnectionManager connections, SonarLinkIssueDataSource issues)
        {
            ConnectionManager = connections;
    
            var service = serviceProvider.GetService(typeof(ITeamExplorer));
            if (service != null)
            {
                TeamExplorer = service as ITeamExplorer;
            }

            var table = GetErrorsTable(serviceProvider);
            table.AddSource(issues, SonarLinkIssueDataSource.Columns);

            PageContent = new ProjectView()
            {
                DataContext = new ProjectViewModel(TeamExplorer, issues)
            };
    
            ViewModel.ItemSelectCommand.CanExecuteChanged += ItemSelectCommand_CanExecuteChanged;
        }
    
        /// <summary>
        /// Command handler which displays the TeamExplorer loading bar based on connection status
        /// </summary>
        /// <param name="sender">Source command which triggered the event</param>
        /// <param name="e">Event arguments</param>
        private void ItemSelectCommand_CanExecuteChanged(object sender, EventArgs e)
        {
            IsBusy = !((ICommand)sender).CanExecute(null);
        }
    
        /// <summary>
        /// Project loading notifications ID
        /// </summary>
        private static readonly Guid ProjectLoadNotificationId = new Guid();
    
        /// <summary>
        /// Team explorer services
        /// </summary>
        private ITeamExplorer TeamExplorer { get; set; }
    
        /// <summary>
        /// ConnectionManager (shared) instance
        /// </summary>
        private IConnectionManager ConnectionManager { get; set; }
    
        /// <summary>
        /// Utility accessor to the underlying View
        /// </summary>
        internal ProjectView View => (ProjectView)PageContent;
    
        /// <summary>
        /// Utility accessor to the View's underlying ViewModel
        /// </summary>
        internal ProjectViewModel ViewModel => (ProjectViewModel)View.DataContext;
    
        #region ITeamExplorerPage
    
        public string Title => "SonarLink";
        public object PageContent { get; private set; }
    
        /// <summary>
        /// Busy status
        /// </summary>
        private bool _isBusy = false;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
    
            private set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    RaisePropertyChanged();
                }
            }
        }
    
        public void Cancel()
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
                    ViewModel.ItemSelectCommand.CanExecuteChanged -= ItemSelectCommand_CanExecuteChanged;
                }
    
                disposedValue = true;
            }
        }
    
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    
        public object GetExtensibilityService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    
        public void Initialize(object sender, PageInitializeEventArgs e)
        {
            var context = e.Context as PageContext;
    
            if (context == null)
            {
                // Bootstrap the ViewModel with a 'default' context
                context = new PageContext()
                {
                    Service = ConnectionManager.Connections.LastOrDefault(),
                    Filter = string.Empty
                };
            }
    
            ViewModel.Service = context.Service;
            ViewModel.Filter = context.Filter;
    
            // Trigger a refresh to force the service to enumerate projects
            // based on the specified filter and connection instances
            Refresh();
        }
    
        public void Loaded(object sender, PageLoadedEventArgs e)
        {
        }
    
        public void Refresh()
        {
            RefreshAsync().Forget();
        }

        /// <summary>
        /// Saves the state/context of the current page
        /// </summary>
        /// <remarks>
        /// Team Explorer triggers this handler when any of the controls/actions are triggered (e.g. home button).
        /// This is used to allow client code to save any necessary context so that navigation through the 'back'
        /// and 'forward' button/controls can resume from said context/state.
        /// </remarks>
        /// <param name="sender">Event source</param>
        /// <param name="e">Event arguments</param>
        public void SaveContext(object sender, PageSaveContextEventArgs e)
        {
            e.Context = new PageContext()
            {
                Service = ViewModel.Service,
                Filter = ViewModel.Filter ?? string.Empty
            };
        }
    
        #endregion
    
        /// <summary>
        /// Refreshes the Sonar projects
        /// </summary>
        /// <returns>Awaitable task which completes when all projects are loaded</returns>
        public async System.Threading.Tasks.Task RefreshAsync()
        {
            try
            {
                IsBusy = true;

                await ViewModel.RefreshAsync();

                TeamExplorer.HideNotification(ProjectLoadNotificationId);
            }
            catch
            {
                // Await until we are in the main thread context to be able
                // to access the View's ViewModel without issues
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                TeamExplorer.ShowNotification($"Could not load projects from \"{ViewModel.Service.BaseUrl}\".",
                            NotificationType.Error, NotificationFlags.None, null, ProjectLoadNotificationId);
            }
            finally
            {
                IsBusy = false;
            }
        }
    
        /// <summary>
        /// Acquires the ITableManagerProvider for the 'Errors' table
        /// </summary>
        /// <param name="serviceProvider">(Visual Studio) Service provider</param>
        /// <returns>An ITableManager instance associated to the 'Errors' table (if available)</returns>
        private static ITableManager GetErrorsTable(IServiceProvider serviceProvider)
        {
            var componentModel = serviceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            var tableManagerProvider = componentModel?.GetService<ITableManagerProvider>();
    
            if (null != tableManagerProvider)
            {
                return tableManagerProvider.GetTableManager(StandardTables.ErrorsTable);
            }
    
            return null;
        }
    
        /// <summary>
        /// Team Explorer Page Context
        /// </summary>
        internal class PageContext
        {
            /// <summary>
            /// Associated SonarQube service/connection
            /// </summary>
            public ISonarQubeService Service { get; set; }
    
            /// <summary>
            /// Project filter
            /// </summary>
            public string Filter { get; set; }
        }
    }
}
