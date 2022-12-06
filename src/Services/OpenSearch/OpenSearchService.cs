using OpenSearch.Client;
using OpenSearch.Net;
using OpenSearchDemo.Models;

namespace OpenSearchDemo.Services.OpenSearch
{
    public class OpenSearchService<T> : IOpenSearchService<T> where T : class
    {
        private readonly OpenSearchClient _client;

        public OpenSearchService(
            AwsConfig awsConfig
        )
        {
            var openSearchConfig = awsConfig.OpenSearch;
            var pool = new SingleNodeConnectionPool(
                new Uri(openSearchConfig.Url));
            var settings = new ConnectionSettings(pool)
                .BasicAuthentication(openSearchConfig.UserName, openSearchConfig.Password);
            _client = new OpenSearchClient(settings);
        }

        public async Task<List<T>> SearchDocumentAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null)
        {
            var response = await _client.SearchAsync<T>(selector);
            return response.Documents.ToList();
        }

        public async Task<long> CountDocumentAsync(Func<CountDescriptor<T>, ICountRequest> selector = null)
        {
            var response = await _client.CountAsync<T>(selector);
            return response.Count;
        }

        public async Task IndexDocumentAsync(IEnumerable<T> obj, string index)
        {
            await EnsureIndexCreatedAsync(index);
            var result = await _client.IndexManyAsync(obj, index);
            if (!result.IsValid || result.Errors) throw new Exception(result.DebugInformation);
        }

        public async Task EnsureIndexCreatedAsync(string index,
            Func<CreateIndexDescriptor, ICreateIndexRequest> indexDescription = null)
        {
            var isExisted = (await _client.Indices.ExistsAsync(index)).Exists;
            if (!isExisted)
            {
                var result = await _client.Indices.CreateAsync(index, indexDescription = null);
                if (!result.IsValid) throw new Exception(result.DebugInformation);
            }
        }

        public async Task DeleteDocumentAsync(DocumentPath<T> id,
            Func<DeleteDescriptor<T>, IDeleteRequest> selector = null)
        {
            var result = await _client.DeleteAsync<T>(id, selector);
            if (!result.IsValid) throw new Exception(result.DebugInformation);
        }
    }
}