using OpenSearch.Client;

namespace OpenSearchDemo.Services.OpenSearch
{
    public interface IOpenSearchService<T> where T : class
    {
        Task<List<T>> SearchDocumentAsync(Func<SearchDescriptor<T>, ISearchRequest> selector = null);
        Task<long> CountDocumentAsync(Func<CountDescriptor<T>, ICountRequest> selector = null);
        Task IndexDocumentAsync(IEnumerable<T> obj, string index);
        Task EnsureIndexCreatedAsync(string index, Func<CreateIndexDescriptor, ICreateIndexRequest> indexDescription = null);
        Task DeleteDocumentAsync(DocumentPath<T> id, Func<DeleteDescriptor<T>, IDeleteRequest> selector = null);
    }
}
