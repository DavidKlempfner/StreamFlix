using StreamFlix.Models.Shelves;

namespace StreamFlix.Retrievers
{
    public interface IDataSourceRetriever
    {
        bool IsPersonalised { get; }
        DataSourceType SupportedType { get; }
        Task<IList<string>> RetrieveShowIdsAsync();
    }
}