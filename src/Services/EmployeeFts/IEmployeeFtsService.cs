using OpenSearchDemo.Models;

namespace OpenSearchDemo.Services.EmployeeFts
{
    public interface IEmployeeFtsService
    {
        Task IndexDocumentAsync(List<EmployeeFtsModel> input);
        Task<List<EmployeeFtsModel>> SearchDocumentAsync(string keyword, int pageSize, int offset);
    }
}
