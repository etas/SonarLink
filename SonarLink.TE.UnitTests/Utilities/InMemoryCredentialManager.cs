using Microsoft.Alm.Authentication;
using SonarLink.TE.Utilities.CredentialsManager;
using System;
using System.Collections.Generic;

namespace SonarLink.TE.UnitTests.Utilities
{
    public class InMemoryCredentialsManager : ICredentialsManager
    {
        public Dictionary<Uri, Credential> _credentials = new Dictionary<Uri, Credential>();

        public bool Delete(Uri target)
        {
            return _credentials.Remove(target);
        }

        public Credential Load(Uri target)
        {
            Credential result = _credentials.TryGetValue(target, out result) ? result : Credential.Empty;
            return result;
        }

        public bool Save(Uri target, Credential credential)
        {
            _credentials[target] = credential;
            return true;
        }
    }
}
