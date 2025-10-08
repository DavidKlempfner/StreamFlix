using StreamFlix.Models.Shelves;

namespace StreamFlix.Retrievers
{
    public interface IDataSourceRetriever
    {
        DataSourceType SupportedType { get; }
        Task<IList<string>> RetrieveShowIdsAsync();
    }
}