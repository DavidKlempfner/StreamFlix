using StreamFlix.Models.Shelves;

namespace StreamFlix.Retrievers
{
    public class ContinuePlayingRetriever : IDataSourceRetriever
    {
        public bool IsPersonalised => true;
        public DataSourceType SupportedType => DataSourceType.ContinuePlaying;

        public async Task<IList<string>> RetrieveShowIdsAsync()
        {
            // TODO: implement this
            return await Task.FromResult(new List<string>());
        }
    }
}