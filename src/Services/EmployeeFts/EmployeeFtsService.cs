using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenSearch.Client;
using OpenSearchDemo.Models;
using OpenSearchDemo.Services.OpenSearch;

namespace OpenSearchDemo.Services.EmployeeFts
{
    public class EmployeeFtsService : IEmployeeFtsService
    {
        private readonly IOpenSearchService<EmployeeFtsModel> _openSearchService;
        private const string INDEX_NAME = "employees";
        public EmployeeFtsService(
            IOpenSearchService<EmployeeFtsModel> openSearchService
        )
        {
            _openSearchService = openSearchService;
        }

        public async Task IndexDocumentAsync(List<EmployeeFtsModel> input)
        {
            await EnsureIndexCreatedAsync();
            await _openSearchService.IndexDocumentAsync(input, INDEX_NAME);
        }

        public async Task<List<EmployeeFtsModel>> SearchDocumentAsync(string keyword, int pageSize, int offset)
        {
            var filterQuery = BuildFilterConditions(keyword, pageSize, offset);
            var totalCount = await _openSearchService.CountDocumentAsync(BuildCountQuery(filterQuery));
            if (totalCount < 1)
            {
                return new List<EmployeeFtsModel>();
            }

            var result = await _openSearchService.SearchDocumentAsync(BuildSearchQuery(filterQuery, pageSize, offset));
            return result;
        }

        public async Task DeleteDocumentAsync(Guid id)
        {
            await _openSearchService.DeleteDocumentAsync(id, x => x.Index(INDEX_NAME));
        }

        #region Private methods

        private async Task EnsureIndexCreatedAsync()
        {
            await _openSearchService.EnsureIndexCreatedAsync(
                INDEX_NAME,
            indice => indice.EmployeeFtsModelMapping());
        }

        private Func<SearchDescriptor<EmployeeFtsModel>, ISearchRequest> BuildSearchQuery
          (Func<QueryContainerDescriptor<EmployeeFtsModel>, QueryContainer> filter, int pageSize, int offset)
        {
            Func<SearchDescriptor<EmployeeFtsModel>, ISearchRequest> query = s =>
              s.Index(INDEX_NAME)
               .Query(filter)
               .Skip(offset)
               .Take(pageSize);
            return query;
        }


        private Func<CountDescriptor<EmployeeFtsModel>, ICountRequest> BuildCountQuery
          (Func<QueryContainerDescriptor<EmployeeFtsModel>, QueryContainer> filter)
        {
            Func<CountDescriptor<EmployeeFtsModel>, ICountRequest> query = s =>
              s.Index(INDEX_NAME)
                .Query(filter);
            return query;
        }

        private Func<QueryContainerDescriptor<EmployeeFtsModel>, QueryContainer> BuildFilterConditions(string keyword, int pageSize, int offset)
        {
            Func<QueryContainerDescriptor<EmployeeFtsModel>, QueryContainer> filter = default;
            if (!string.IsNullOrEmpty(keyword))
            {
                filter = s => s
                  .QueryString(m => m
                        .Fields(x => x
                          .Field(fld => fld.FullName)
                        )
                        .Query($"*{keyword}*")); // Like partial
            }

            return filter;
        }

        #endregion
    }
}
