// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using SonarLink.API.Models;
using SonarLink.TE.ErrorList;
using SonarLink.TE.MVVM;
using SonarLink.TE.Utilities;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Threading.Tasks;
using SonarLink.API.Clients;

namespace SonarLink.TE.ViewModel
{

    /// <summary>
    /// ViewModel for SonarQube project listing view
    /// </summary>
    public class ProjectViewModel : NotifyPropertyChangeSource
    {
        /// <summary>
        /// Project name filter string
        /// </summary>
        private string _filter;
    
        /// <summary>
        /// Last selected Sonar project
        /// </summary>
        private string _projectIssuesInView;
    
        /// <summary>
        /// SonarQube service/connection
        /// </summary>
        private ISonarQubeClient _client;
    
        /// <summary>
        /// ErrorList window provider
        /// </summary>
        private ErrorListProvider _errorListProvider;

        /// <summary>
        /// Manages access to and persistence of the Sonar projects and local path associations
        /// </summary>
        private readonly IProjectPathsManager _projectPathsManager;
    
        /// <summary>
        /// Command for associating Sonar project source code to a local path
        /// </summary>
        public ICommand FolderSelectCommand { get; }
    
        /// <summary>
        /// Command for enumerating Sonar project issues
        /// </summary>
        public ICommand ItemSelectCommand { get; }
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="teamExplorer">TeamExplorer VS service</param>
        /// <param name="issuesDataSource">SonarQube issues data source</param>
        /// <param name="projectPathsManager">Project paths manager</param>
        public ProjectViewModel(ITeamExplorer teamExplorer, 
                                SonarLinkIssueDataSource issuesDataSource, 
                                IProjectPathsManager projectPathsManager)
        {
            TeamExplorer = teamExplorer;
            ErrorTable = issuesDataSource;
            _projectPathsManager = projectPathsManager;

            // Bootstrap the projects view with an empty collection
            SonarProjects = Enumerable.Empty<SonarQubeProject>();

            FolderSelectCommand = new AsyncCommand(param => OnFolderSelectAsync((SonarQubeProject) param));
            ItemSelectCommand = new AsyncCommand(param => OnItemSelectAsync((SonarQubeProject) param));
        }
    
        /// <summary>
        /// File path resolution notification ID
        /// </summary>
        private static readonly Guid FilePathResolutionNotificationId = new Guid();
    
        /// <summary>
        /// Sonar errors notification ID
        /// </summary>
        private static readonly Guid SonarErrorsNotificationId = new Guid();
    
        /// <summary>
        /// Team Explorer Handler
        /// </summary>
        public ITeamExplorer TeamExplorer { get; private set; }
    
        /// <summary>
        /// (Visual Studio) Error Table
        /// </summary>
        public SonarLinkIssueDataSource ErrorTable { get; private set; }
    
        /// <summary>
        /// SonarQube service/connection
        /// </summary>
        public ISonarQubeClient Client
        {
            get
            {
                return _client;
            }
    
            set
            {
                _client = value;
                _errorListProvider = new ErrorListProvider(_client);
            }
        }
    
        /// <summary>
        /// Filter-able SonarQube projects listing
        /// </summary>
        private CollectionViewSource _projects = new CollectionViewSource();
    
        /// <summary>
        /// (Cached) Listing of SonarQube projects
        /// </summary>
        private IEnumerable<SonarQubeProject> SonarProjects
        {  
            set
            {
                var view = new CollectionViewSource
                {
                    Source = value ?? Enumerable.Empty<SonarQubeProject>()
                };
                view.Filter += ProjectsFilter;
    
                _projects = view;
    
                RaisePropertyChanged(nameof(Projects));
            }
        }
    
        /// <summary>
        /// View filter for listing SonarQube projects
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">Event arguments</param>
        private void ProjectsFilter(object sender, FilterEventArgs e)
        {
            var empty = string.IsNullOrEmpty(Filter);
            e.Accepted = empty;
    
            if (!empty)
            {
                e.Accepted = (e.Item is SonarQubeProject item) && (item.Name.IndexOf(Filter, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }
    
        /// <summary>
        /// Sonar (filtered) project listing
        /// </summary>
        public ICollectionView Projects => _projects.View;
    
        /// <summary>
        /// Project name filter string
        /// </summary>
        public string Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    Projects.Refresh();
    
                    RaisePropertyChanged(nameof(Projects));
                }
            }
        }
    
        /// <summary>
        /// Acquires the projects from SonarQube 
        /// </summary>
        /// <returns>Awaitable task to identify when all projects are loaded</returns>
        public async Task RefreshAsync()
        {
            SonarProjects = await _client?.Components.GetAllProjects();
        }
    
        /// <summary>
        /// Handler which associates source code of Sonar project to a local directory
        /// </summary>
        /// <param name="project">SonarQube project to associate</param>
        /// <returns>Awaitable task which associates a local path and enumerates associated errors</returns>
        private Task OnFolderSelectAsync(SonarQubeProject project)
        {
            string path = null;

            using (var folderSelectDialog = new FolderSelectDialog())
            {
                if (folderSelectDialog.ShowDialog())
                {
                    path = folderSelectDialog.SelectedPath;
                }
            }

            if (!string.IsNullOrEmpty(path))
            {
                _projectPathsManager.Add(project.Key, path);

                // 'Set Local Path' implies 'View Issues' if the
                // project has already had its issues loaded
                if (project.Key == _projectIssuesInView)
                {
                    return OnItemSelectAsync(project);
                }
            }

            return Task.CompletedTask;
        }
    
        /// <summary>
        /// Handler which enumerates issues for a Sonar project
        /// </summary>
        /// <param name="project">SonarQube project to query</param>
        /// <returns>Awaitable task which enumerates associated errors</returns>
        private async Task OnItemSelectAsync(SonarQubeProject project)
        {
            _projectPathsManager.TryGetValue(project.Key, out string projectLocalPath);

            IEnumerable<ErrorListItem> errors;

            try
            {
                errors = await _errorListProvider.GetErrorsAsync(project.Key, projectLocalPath);
            }
            catch
            {
                TeamExplorer.ShowNotification($"Failed to download issues for \"{project.Name}\".",
                        NotificationType.Error, NotificationFlags.None, null, SonarErrorsNotificationId);

                return;
            }

            TeamExplorer.HideNotification(SonarErrorsNotificationId);
    
            if (errors.Any())
            {
                if (!Path.IsPathRooted(errors.First().FileName))
                {
                    TeamExplorer.ShowNotification("Unable to resolve local file path for issues. Make sure project local path is correctly configured.",
                            NotificationType.Warning, NotificationFlags.None, null, FilePathResolutionNotificationId);
                }
                else
                {
                    TeamExplorer.HideNotification(FilePathResolutionNotificationId);
                }
            }
    
            ErrorTable.CleanAllErrors();
            ErrorTable.AddErrors(project.Name, errors);
    
            _projectIssuesInView = project.Key;
        }
    }
}