// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using NUnit.Framework;
using SonarLink.TE.Utilities;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SonarLink.TE.UnitTests.Tests
{
    [TestFixture]
    class DirectorySearcherTests
    {
        /// <summary>
        /// Assert that: It is possible to enumerate the bottom-most (leaf) directories
        ///              from within a directory hierarchy
        /// </summary>
        [Test]
        public void RecursiveDirectoryEnumeration()
        {
            using (var a = TemporaryFile.CreateDirectory(Path.Combine(Path.GetTempPath(), "a")))
            using (var aa = TemporaryFile.CreateDirectory(Path.Combine(a.Path, "aa")))
            using (var ab = TemporaryFile.CreateDirectory(Path.Combine(a.Path, "ab")))
            using (var aaa = TemporaryFile.CreateDirectory(Path.Combine(aa.Path, "aaa")))
            {
                var directories = DirectorySearcher.GetDirectories(a.Path);
                Assert.That(directories.Select(dir => dir.FullName), Is.EquivalentTo(new[] { ab.Path, aaa.Path }));
            }
        }

        /// <summary>
        /// Assert that: It is possible to enumerate the bottom-most (leaf) directories
        ///              from within a directory hierarchy
        /// </summary>
        [Test]
        public void FastRecursiveDirectoryListing()
        {
            using (var a = TemporaryFile.CreateDirectory(Path.Combine(Path.GetTempPath(), "a")))
            using (var aa = TemporaryFile.CreateDirectory(Path.Combine(a.Path, "aa")))
            using (var ab = TemporaryFile.CreateDirectory(Path.Combine(a.Path, "ab")))
            using (var aaa = TemporaryFile.CreateDirectory(Path.Combine(aa.Path, "aaa")))
            {
                var directories = DirectorySearcher.GetDirectoriesFast(a.Path);
                Assert.That(directories.Select(dir => dir.FullName), Is.EquivalentTo(new[] { ab.Path, aaa.Path }));
            }
        }

        /// <summary>
        /// Assert that: It is possible to enumerate the bottom-most (leaf) directories
        ///              from within a directory hierarchy
        /// </summary>
        [Test]
        public async Task AsyncRecursiveDirectoryListing()
        {
            using (var a = TemporaryFile.CreateDirectory(Path.Combine(Path.GetTempPath(), "a")))
            using (var aa = TemporaryFile.CreateDirectory(Path.Combine(a.Path, "aa")))
            using (var ab = TemporaryFile.CreateDirectory(Path.Combine(a.Path, "ab")))
            using (var aaa = TemporaryFile.CreateDirectory(Path.Combine(aa.Path, "aaa")))
            {
                var directories = await DirectorySearcher.GetDirectoriesFastAsync(a.Path);
                Assert.That(directories.Select(dir => dir.FullName), Is.EquivalentTo(new[] { ab.Path, aaa.Path }));
            }
        }
    }
}
