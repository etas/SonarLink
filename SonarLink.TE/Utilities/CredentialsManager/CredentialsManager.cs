// (C) Copyright 2020 ETAS GmbH (http://www.etas.com/)

using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Microsoft.Alm.Authentication;

namespace SonarLink.TE.Utilities.CredentialsManager
{
    [Export(typeof(ICredentialsManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class CredentialsManager : ICredentialsManager
    {
        /// <summary>
        /// Credential storage system backed by the operating system's keychain / secrets vault.
        /// </summary>
        public readonly SecretStore store = new SecretStore(Assembly.GetExecutingAssembly().GetName().Name);

        /// <summary>
        /// Provides secure credential storage. 
        /// </summary>
        public Credential Load(Uri target)
        {
            return store.ReadCredentials(target);
        }

        /// <summary>
        /// Saves the given credential.
        /// </summary>
        /// <param name="target">Name of the application/URL the credential is used for.</param>
        /// <param name="credential">Credential to store.</param>
        /// <returns>True if successfully stored, false otherwise.</returns>
        public bool Save(Uri target, Credential credential)
        {
            return store.WriteCredentials(target, credential);
        }

        /// <summary>
        /// Delete stored credential.
        /// </summary>
        /// <param name="target">Name of the application/URL the credential is used for.</param>
        /// <returns>True if successfully deleted, false otherwise.</returns>
        public bool Delete(Uri target)
        {
            return store.DeleteCredentials(target);
        }
    }
}
