// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Models;
using SonarLink.TE.View;
using System.Windows;

namespace SonarLink.TE.ViewModel.Connection
{
    /// <summary>
    /// SonarQube Connection UI
    /// </summary>
    public static class ConnectionInformationDialog
    {
        /// <summary>
        /// Shows a (modal) dialog which requests connection details from the user
        /// </summary>
        /// <param name="currentConnection">Connection information used to pre-populate fields</param>
        /// <returns>Connection details inputted by user or null if the dialog is dismissed</returns>
        public static ConnectionInformation ShowDialog(ConnectionInformation currentConnection = null)
        {
            var viewModel = new ConnectionInfoDialogViewModel();
            var dialog = new ConnectionInfoDialogView
            {
                DataContext = viewModel,
                Owner = Application.Current.MainWindow
            };
    
            if (null != currentConnection)
            {
                viewModel.ServerUrl = currentConnection.ServerUrl;
                viewModel.Login = currentConnection.Login;
            }
            
            bool? result = dialog.ShowDialog();
    
            if (result.GetValueOrDefault())
            {
                return new ConnectionInformation(viewModel.ServerUrl, viewModel.Login, viewModel.Password);
            }
    
            return null;
        }
    }
}
