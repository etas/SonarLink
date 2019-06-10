// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Shell.TableManager;

namespace SonarLink.TE.UnitTests.Tests
{
    /// <summary>
    /// Stub implementation of ITableDataSink
    /// </summary>
    public class StubTableDataSink : ITableDataSink
    {
        /// <summary>
        /// Table entries
        /// </summary>
        public List<ITableEntry> Entries { get; private set; } = new List<ITableEntry>();

        /// <summary>
        /// Snapshot factories
        /// </summary>
        public List<ITableEntriesSnapshotFactory> SnapshotFactories { get; private set; } = new List<ITableEntriesSnapshotFactory>();

        /// <summary>
        /// Table entry snapshots
        /// </summary>
        public List<ITableEntriesSnapshot> Snapshots { get; private set; } = new List<ITableEntriesSnapshot>();

        #region ITableDataSink

        /// <inheritdoc />
        public bool IsStable { get; set; } = true;

        /// <inheritdoc />
        public void AddEntries(IReadOnlyList<ITableEntry> newEntries, bool removeAllEntries = false)
        {
            if (removeAllEntries)
            {
                RemoveAllEntries();
            }

            Entries.AddRange(newEntries);
        }

        /// <inheritdoc />
        public void AddFactory(ITableEntriesSnapshotFactory newFactory, bool removeAllFactories = false)
        {
            if (removeAllFactories)
            {
                RemoveAllFactories();
            }

            SnapshotFactories.Add(newFactory);
        }

        /// <inheritdoc />
        public void AddSnapshot(ITableEntriesSnapshot newSnapshot, bool removeAllSnapshots = false)
        {
            if (removeAllSnapshots)
            {
                RemoveAllSnapshots();
            }

            Snapshots.Add(newSnapshot);
        }

        /// <inheritdoc />
        public void FactorySnapshotChanged(ITableEntriesSnapshotFactory factory)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void RemoveAllEntries()
        {
            Entries.Clear();
        }

        /// <inheritdoc />
        public void RemoveAllFactories()
        {
            SnapshotFactories.Clear();
        }

        /// <inheritdoc />
        public void RemoveAllSnapshots()
        {
            Snapshots.Clear();
        }

        /// <inheritdoc />
        public void RemoveEntries(IReadOnlyList<ITableEntry> oldEntries)
        {
            foreach (var entry in oldEntries)
            {
                Entries.Remove(entry);
            }
        }

        /// <inheritdoc />
        public void RemoveFactory(ITableEntriesSnapshotFactory oldFactory)
        {
            SnapshotFactories.Remove(oldFactory);
        }

        /// <inheritdoc />
        public void RemoveSnapshot(ITableEntriesSnapshot oldSnapshot)
        {
            Snapshots.Remove(oldSnapshot);
        }

        /// <inheritdoc />
        public void ReplaceEntries(IReadOnlyList<ITableEntry> oldEntries, IReadOnlyList<ITableEntry> newEntries)
        {
            var entries = oldEntries.Zip(newEntries, (oldEntry, newEntry) => new { Old = oldEntry, New = newEntry });

            foreach (var entry in entries)
            {
                var index = Entries.IndexOf(entry.Old);
                if (index != -1)
                {
                    Entries[index] = entry.New;
                }
            }
        }

        /// <inheritdoc />
        public void ReplaceFactory(ITableEntriesSnapshotFactory oldFactory, ITableEntriesSnapshotFactory newFactory)
        {
            var index = SnapshotFactories.IndexOf(oldFactory);
            if (index != -1)
            {
                SnapshotFactories[index] = newFactory;
            }
        }

        /// <inheritdoc />
        public void ReplaceSnapshot(ITableEntriesSnapshot oldSnapshot, ITableEntriesSnapshot newSnapshot)
        {
            var index = Snapshots.IndexOf(oldSnapshot);
            if (index != -1)
            {
                Snapshots[index] = newSnapshot;
            }
        }

        #endregion
    }
}
