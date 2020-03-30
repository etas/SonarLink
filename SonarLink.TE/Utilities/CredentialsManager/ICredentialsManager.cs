// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using System;
using Microsoft.Alm.Authentication;

namespace SonarLink.TE.Utilities.CredentialsManager
{
    /// <summary>
    /// Manages the storage and retrieval of user credentials. 
    /// </summary>
    public interface ICredentialsManager
    {
        /// <summary>
        /// Loads the store credential.
        /// </summary>
        /// <param name="target">Name of the application/URL the credential is used for.</param>
        /// <returns>Credential details if successful, empty credential details otherwise.</returns>
        Credential Load(Uri target);

        /// <summary>
        /// Saves the given credential.
        /// </summary>
        /// <param name="target">Name of the application/URL the credential is used for.</param>
        /// <param name="credential">Credential to store.</param>
        /// <returns>True if successfully stored, false otherwise.</returns>
        bool Save(Uri target, Credential credential);

        /// <summary>
        /// Delete stored credential.
        /// </summary>
        /// <param name="target">Name of the application/URL the credential is used for.</param>
        /// <returns>True if successfully deleted, false otherwise.</returns>
        bool Delete(Uri target);
    }
}
