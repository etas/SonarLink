// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.TeamFoundation.Controls;
using SonarLink.API.Models;
using SonarLink.API.Services;
using SonarLink.TE.MVVM;
using SonarLink.TE.Model;
using SonarLink.TE.ViewModel.Connection;
using System;
using System.Windows.Input;
using System.Threading.Tasks;

namespace SonarLink.TE.ViewModel
{
    /// <summary>
    /// ViewModel for SonarQube connection page
    /// </summary>
    public class ConnectPageViewModel : NotifyPropertyChangeSource
    {
        /// <summary>
        /// Connection notification ID
        /// </summary>
        private static readonly Guid ConnectionNotificationId = new Guid();

        /// <summary>
        /// Indicates whether an attempt to establish a connection is ongoing.
        /// </summary>
        private bool _isAttemptingToConnect = false;

        public ConnectPageViewModel(ITeamExplorer explorer, IConnectionManager manager)
        {
            TeamExplorer = explorer;
            ConnectionManager = manager;

            ConnectCommand = new AsyncCommand(OnConnectAsync);
        }

        private ConnectionInformation LastAttemptedConnection { get; set; }

        public IConnectionManager ConnectionManager { get; private set; }

        public ICommand ConnectCommand { get; }

        public ITeamExplorer TeamExplorer { get; private set; }

        /// <summary>
        /// Indicates whether an attempt to establish a connection is ongoing.
        /// </summary>
        public bool IsAttemptingToConnect
        {
            get
            {
                return _isAttemptingToConnect;
            }

            set
            {
                if (_isAttemptingToConnect != value)
                {
                    _isAttemptingToConnect = value;
                    RaisePropertyChanged();
                }
            }
        }

        private async Task OnConnectAsync(object parameter)
        {
            var connectionInfo = ConnectionInformationDialog.ShowDialog(LastAttemptedConnection);
            if (connectionInfo != null)
            {
                LastAttemptedConnection = connectionInfo;

                SonarQubeService service = null;
                bool connected = false;

                IsAttemptingToConnect = true;

                try
                {
                    service = new SonarQubeService();
                    connected = await service.ConnectAsync(LastAttemptedConnection);
                }
                catch
                {
                    connected = false;
                }

                IsAttemptingToConnect = false;

                if (connected)
                {
                    // This ViewModel/Controller only supports 1 connection
                    ConnectionManager.Connections.Clear();

                    ConnectionManager.Connections.Add(service);

                    TeamExplorer.HideNotification(ConnectionNotificationId);

                    TeamExplorer.NavigateToPage(new Guid(SonarLinkProjectPage.PageId), new SonarLinkProjectPage.PageContext()
                    {
                        Service = service,
                        Filter = string.Empty
                    });
                }
                else
                {
                    TeamExplorer.ShowNotification($"Could not connect to \"{connectionInfo.ServerUrl}\".",
                                                  NotificationType.Error, NotificationFlags.None, null, ConnectionNotificationId);
                }
            }
        }
    }
}
