// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using SonarLink.API.Services;
using SonarLink.TE.ErrorList;
using SonarLink.TE.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly ISonarQubeService _service;
    
        /// <summary>
        /// Cache of previously download errors
        /// </summary>
        private Dictionary<string, IEnumerable<ErrorListItem>> _cachedErrors = new Dictionary<string, IEnumerable<ErrorListItem>>();
    
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service">SonarQube server</param>
        public ErrorListProvider(ISonarQubeService service)
        {
            _service = service;
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
                var issues = await _service.GetProjectIssuesAsync(projectKey);
                errors = issues.Select(issue => new ErrorListItem(_service, issue)).ToList();
                _cachedErrors[projectKey] = errors;
            }
    
            if (!errors.Any())
            {
                return errors;
            }
    
            if (!Path.IsPathRooted(errors.First().FileName) && !string.IsNullOrEmpty(projectLocalPath))
            {
                var directories = await DirectorySearcher.GetDirectoriesFastAsync(projectLocalPath);
                var projectRootPath = ResolveProjectRootPath(errors.First().FileName, directories);
                if (!string.IsNullOrEmpty(projectRootPath))
                {
                    errors.ToList().ForEach(x =>
                    {
                        string cleanFilePath = x.FileName.Split(':').Last();
                        x.FileName = projectRootPath + cleanFilePath;
                    });
                }
            }
    
            return errors;
        }
    
        /// <summary>
        /// Attempts to find the best match directory which hosts the project content
        /// </summary>
        /// <param name="projectPath">Project directory path (as stated by SonarQube)</param>
        /// <param name="directories">Set of directories which should be tested for validity</param>
        /// <returns>A resolved project path or null if it cannot be resolved</returns>
        private string ResolveProjectRootPath(string projectPath, IEnumerable<DirectoryInfo> directories)
        {
            string projectDirectoryName = Path.GetDirectoryName(projectPath);
    
            var regexString = projectDirectoryName.Replace("\\", "\\\\");
            var regex = new Regex(regexString.ToString());

            var directory = directories.FirstOrDefault(dir => regex.IsMatch(dir.FullName));

            if (directory != null)
            {
                var index = directory.FullName.IndexOf(projectDirectoryName);
                return directory.FullName.Substring(0, index);
            }

            return null;
        }
    }

}
