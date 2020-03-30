// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using SonarLink.API.Models;
using SonarLink.TE.MVVM;
using SonarLink.TE.Model;
using SonarLink.TE.ViewModel.Connection;
using System;
using System.Windows.Input;
using System.Threading.Tasks;
using SonarLink.API.Clients;
using System.Linq;

namespace SonarLink.TE.ViewModel
{
    /// <summary>
    /// ViewModel for SonarQube connection page.
    /// </summary>
    public class ConnectPageViewModel : NotifyPropertyChangeSource
    {
        /// <summary>
        /// Connection notification ID.
        /// </summary>
        static readonly Guid ConnectionNotificationId = new Guid();

        /// <summary>
        /// Visual Studio Team Explorer service.
        /// </summary>
        readonly ITeamExplorer _teamExplorer;

        /// <summary>
        /// Manages the configured <see cref="ISonarQubeClient"/> instances.
        /// </summary>
        readonly IClientManager _clientManager;

        /// <summary>
        /// Indicates whether an attempt to sign in is ongoing.
        /// </summary>
        bool _isSigningIn = false;

        /// <summary>
        /// Indicates whether a client is signed in.
        /// </summary>
        bool _isSignedIn = false;

        /// <summary>
        /// Server URL and user credentials used for the last failed sign in attempt.
        /// </summary>
        ConnectionInformation _lastAttemptedSignIn;

        /// <summary>,
        /// Constructor.
        /// </summary>
        /// <param name="teamExplorer">TeamExplorer VS service.</param>
        /// <param name="clientManager">Manages the configured <see cref="ISonarQubeClient"/> instances.</param>
        public ConnectPageViewModel(ITeamExplorer explorer, IClientManager clientManager)
        {
            _teamExplorer = explorer;
            _clientManager = clientManager;

            IsSignedIn = (clientManager.Clients.Count > 0);

            SignInCommand = new AsyncCommand(OnSignInAsync);
            ViewProjectsCommand = new Command(OnViewProjects);
            SignOutCommand = new Command(OnSignOut);
        }

        /// <summary>
        /// Command for signing in to a SonarQube server.
        /// </summary>
        public ICommand SignInCommand { get; }

        /// <summary>
        /// Command to switch to projects view.
        /// </summary>
        public ICommand ViewProjectsCommand { get; }

        /// <summary>
        /// Command to sign out of a SonarQube server.
        /// </summary>
        public ICommand SignOutCommand { get; }

        /// <summary>
        /// Indicates whether an attempt to sign in is ongoing.
        /// </summary>
        public bool IsSigningIn
        {
            get
            {
                return _isSigningIn;
            }

            set
            {
                if (_isSigningIn != value)
                {
                    _isSigningIn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicates whether a client is signed in.
        /// </summary>
        public bool IsSignedIn
        {
            get
            {
                return _isSignedIn;
            }

            set
            {
                if (_isSignedIn != value)
                {
                    _isSignedIn = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Handler which attempts to sign in to a SonarQube server.
        /// </summary>
        private async Task OnSignInAsync(object parameter)
        {
            var connectionInfo = ConnectionInformationDialog.ShowDialog(_lastAttemptedSignIn);
            if (connectionInfo is null)
            {
                return;
            }

            _lastAttemptedSignIn = connectionInfo;
            ISonarQubeClient client = null;

            IsSignedIn = false;
            IsSigningIn = true;

            try
            {
                client = await _clientManager.LogInAsync(new Uri(connectionInfo.ServerUrl), connectionInfo.Login, connectionInfo.Password);
                IsSignedIn = (null != client);
            }
            catch
            {
                IsSignedIn = false;
            }

            IsSigningIn = false;

            if (IsSignedIn)
            {
                _teamExplorer.HideNotification(ConnectionNotificationId);
                NavigateToProjectPage(client);
            }
            else
            {
                _teamExplorer.ShowNotification($"Could not connect to \"{connectionInfo.ServerUrl}\".",
                    NotificationType.Error, NotificationFlags.None, null, ConnectionNotificationId);
            }
        }

        /// <summary>
        /// Handler which navigates to the project view page.
        /// </summary>
        private void OnViewProjects(object parameter)
        {
            NavigateToProjectPage(_clientManager.Clients.LastOrDefault());
        }

        /// <summary>
        /// Handler which signs out of a SonarQube server.
        /// </summary>
        private void OnSignOut(object parameter)
        {
            _clientManager.LogOut();
            IsSignedIn = false;
        }

        /// <summary>
        /// Navigates to the project view page.
        /// </summary>
        /// <param name="client">Client for the SonarQube API.</param>
        private void NavigateToProjectPage(ISonarQubeClient client)
        {
            _teamExplorer.NavigateToPage(new Guid(SonarLinkProjectPage.PageId), new SonarLinkProjectPage.PageContext()
            {
                Client = client,
                Filter = string.Empty
            });
        }
    }
}
