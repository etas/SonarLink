// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;

namespace SonarLink.TE.ErrorList
{
    /// <summary>
    /// SonarQube issue 'Error List' data source
    /// </summary>
    [Export(typeof(SonarLinkIssueDataSource))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SonarLinkIssueDataSource : ITableDataSource
    {
        /// <summary>
        /// Data sink subscriptions
        /// </summary>
        private readonly List<SinkManager> _managers = new List<SinkManager>();

        /// <summary>
        /// Error list snapshots
        /// </summary>
        private readonly Dictionary<string, TableEntriesSnapshot> _snapshots = new Dictionary<string, TableEntriesSnapshot>();
    
        /// <summary>
        /// 'Error list' columns/components exposed by items managed by this data source
        /// </summary>
        public static IReadOnlyCollection<string> Columns { get; } = new[]
        {
            StandardTableColumnDefinitions.DetailsExpander,
            StandardTableColumnDefinitions.ErrorCategory,
            StandardTableColumnDefinitions.ErrorSeverity,
            StandardTableColumnDefinitions.ErrorCode,
            StandardTableColumnDefinitions.ErrorSource,
            StandardTableColumnDefinitions.BuildTool,
            StandardTableColumnDefinitions.Text,
            StandardTableColumnDefinitions.DocumentName,
            StandardTableColumnDefinitions.Line,
            StandardTableColumnDefinitions.Column
        };

        #region ITableDataSource members

        public string SourceTypeIdentifier => StandardTableDataSources.ErrorTableDataSource;
    
        public string Identifier => "SonarLink";

        public string DisplayName => "SonarLink";

        public IDisposable Subscribe(ITableDataSink sink)
        {
            var manager = new SinkManager(sink, RemoveSinkManager);

            AddSinkManager(manager);

            return manager;
        }

        #endregion

        /// <summary>
        /// Registers a sink subscription
        /// </summary>
        /// <param name="manager">Subscription to register</param>
        private void AddSinkManager(SinkManager manager)
        {
            lock (_managers)
            {
                _managers.Add(manager);
            }
        }

        /// <summary>
        /// Unregisters a previously registered sink subscription
        /// </summary>
        /// <param name="manager">Subscription to unregister</param>
        private void RemoveSinkManager(SinkManager manager)
        {
            lock (_managers)
            {
                _managers.Remove(manager);
            }
        }
    
        /// <summary>
        /// Notifies all subscribers of an update in error (listings)
        /// </summary>
        private void UpdateAllSinks()
        {
            lock (_managers)
            {
                foreach (var manager in _managers)
                {
                    manager.UpdateSink(_snapshots.Values);
                }
            }
        }
    
        /// <summary>
        /// Identifies a new set of errors
        /// </summary>
        /// <remarks>All subscribed sinks are notified of the new error set</remarks>
        /// <param name="projectName">SonarQube project name</param>
        /// <param name="errors">SonarQube issues/errors for the associated project</param>
        public void AddErrors(string projectName, IEnumerable<ErrorListItem> errors)
        {
            if (errors == null || !errors.Any())
            {
                return;
            }
    
            var cleanErrors = errors.Where(e => (e != null) && !string.IsNullOrEmpty(e.FileName));
    
            lock (_snapshots)
            {
                foreach (var error in cleanErrors.GroupBy(e => e.FileName))
                {
                    _snapshots[error.Key] = new TableEntriesSnapshot(projectName, error.Key, error);
                }
            }
    
            UpdateAllSinks();
        }
    
        /// <summary>
        /// Clears all previously registered issues/errors
        /// </summary>
        public void CleanAllErrors()
        {
            lock (_snapshots)
            {
                lock (_managers)
                {
                    foreach (var manager in _managers)
                    {
                        manager.Clear();
                    }
                }

                foreach (var snapshot in _snapshots.Values)
                {
                    snapshot.Dispose();
                }

                _snapshots.Clear();
            }
        }
    }
}
