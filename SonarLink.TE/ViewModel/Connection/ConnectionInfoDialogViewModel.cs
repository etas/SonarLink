// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.TE.MVVM;
using System;

namespace SonarLink.TE.ViewModel
{
    /// <summary>
    /// ViewModel for SonarQube connection details dialog
    /// </summary>
    public class ConnectionInfoDialogViewModel : NotifyPropertyChangeSource
    {
        /// <summary>
        /// SonarQube server (base) URL
        /// </summary>
        private string _serverUrl = string.Empty;

        /// <summary>
        /// SonarQube user name/API token
        /// </summary>
        private string _login = string.Empty;

        /// <summary>
        /// SonarQube user password
        /// </summary>
        private string _password = string.Empty;

        /// <summary>
        /// SonarQube server (base) URL
        /// </summary>
        public string ServerUrl
        {
            get
            {
                return _serverUrl;
            }
            set
            {
                if (_serverUrl != value)
                {
                    _serverUrl = value;
                    RaisePropertyChanged();

                    // Dependendant properties
                    RaisePropertyChanged(nameof(ShowWarning));
                    RaisePropertyChanged(nameof(IsServerUrlValid));
                }
            }
        }

        /// <summary>
        /// SonarQube user name/API token
        /// </summary>
        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                if (_login != value)
                {
                    _login = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// SonarQube user password
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
    
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(ShowPasswordHintText));
                }
            }
        }
    
        /// <summary>
        /// Determines if the 'warning' alert should be made visible
        /// </summary>
        public bool ShowWarning
        {
            get { return !string.IsNullOrEmpty(ServerUrl) && !IsServerUrlValid; }
        }

        /// <summary>
        /// Identifies whether or not the SonarQube URL is valid
        /// </summary>
        public bool IsServerUrlValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ServerUrl))
                {
                    return false;
                }
    
                if (!Uri.TryCreate(ServerUrl, UriKind.Absolute, out Uri uri))
                {
                    return false;
                }
    
                if ((uri.Scheme != Uri.UriSchemeHttp) && (uri.Scheme != Uri.UriSchemeHttps))
                {
                    return false;
                }
    
                return true;
            }
        }

        /// <summary>
        /// Determines if the password hint overlay text should be made visible
        /// </summary>
        public bool ShowPasswordHintText
        {
            get { return string.IsNullOrEmpty(Password); }
        }
    }
}