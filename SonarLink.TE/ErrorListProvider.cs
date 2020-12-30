// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Clients;
using SonarLink.TE.ErrorList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SonarLink.TE
{
    /// <summary>
    /// SonarQube issue provider
    /// </summary>
    public class ErrorListProvider
    {
        /// <summary>
        /// SonarQube server instance
        /// </summary>
        private readonly ISonarQubeClient _client;

        /// <summary>
        /// Cache of previously download errors
        /// </summary>
        private readonly Dictionary<string, IEnumerable<ErrorListItem>> _cachedErrors = new Dictionary<string, IEnumerable<ErrorListItem>>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">SonarQube server</param>
        public ErrorListProvider(ISonarQubeClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Downloads SonarQube issues associated to the requested project. Issues are
        /// translated to 'Visual Studio' error items and get their source code file
        /// paths relative to the provided project path.
        /// </summary>
        /// <param name="projectKey">SonarQube project key (not name)</param>
        /// <param name="projectLocalPath">Local directory path which hosts the project source files</param>
        /// <returns></returns>
        public async Task<IEnumerable<ErrorListItem>> GetErrorsAsync(string projectKey, string projectLocalPath)
        {
            IEnumerable<ErrorListItem> errors = Enumerable.Empty<ErrorListItem>();

            if (!_cachedErrors.TryGetValue(projectKey, out errors))
            {
                var issues = await _client.Issues.GetProjectIssues(projectKey);
                errors = issues.Select(issue => new ErrorListItem(_client.SonarQubeApiUrl, issue)).ToList();
                _cachedErrors[projectKey] = errors;
            }

            if (!string.IsNullOrEmpty(projectLocalPath) && (errors.Any() && !Path.IsPathRooted(errors.First().FileName)))
            {
                var pathRewrite = errors.ToList();

                pathRewrite.ForEach(x =>
                {
                    x.FileName = Path.Combine(projectLocalPath, x.FileName);
                });

                errors = pathRewrite;
            }

            return errors;
        }
    }

}
