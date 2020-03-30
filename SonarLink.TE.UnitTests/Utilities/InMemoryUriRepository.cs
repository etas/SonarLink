using SonarLink.TE.Utilities;
using SonarLink.TE.Utilities.Repository;

namespace SonarLink.TE.UnitTests.Utilities
{
    public class InMemoryUriRepository : IRepository<UriRepositoryItem>
    {
        public UriRepositoryItem Data { get; set; } = new UriRepositoryItem();

        public void Save()
        {
        }
    }
}
