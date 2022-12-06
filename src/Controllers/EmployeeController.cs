using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OpenSearch.Client;
using OpenSearchDemo.Entities;
using OpenSearchDemo.Models;
using OpenSearchDemo.Services.EmployeeFts;

namespace OpenSearchDemo.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IEmployeeFtsService _employeeOpenSearchFtsService;

        public EmployeeController(
            AppDbContext dbContext,
            IEmployeeFtsService employeeOpenSearchFtsService)
        {
            _dbContext = dbContext;
            _employeeOpenSearchFtsService = employeeOpenSearchFtsService;
        }

        [HttpGet("v1")]
        public async Task<List<Employee>> GetListEmployee(
          [FromQuery] string keyword,
          [FromQuery] int? pageSize,
          [FromQuery] int? pageIndex)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            var offset = (int)((pageIndex - 1) * pageSize);
            return await _dbContext.Employees
                .Where(x => x.FullName.Contains(keyword))
                .Skip(offset)
                .Take(pageSize.Value)
                .ToListAsync();
        }

        [HttpGet("v2")]
        public async Task<List<EmployeeFts>> GetListEmployeeFts(
            [FromQuery] string keyword,
            [FromQuery] int? pageSize,
            [FromQuery] int? pageIndex)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            var offset = (int)((pageIndex - 1) * pageSize);
            return await _dbContext.EmployeesFts
                .Where(x => EF.Functions.Match(x.FullName, keyword, MySqlMatchSearchMode.Boolean))
                .Skip(offset)
                .Take(pageSize.Value)
                .ToListAsync();
        }

        [HttpGet("v3")]
        public async Task<List<EmployeeFts>> GetListEmployeeOpenSearchFts(
            [FromQuery] string keyword,
            [FromQuery] int? pageSize,
            [FromQuery] int? pageIndex)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            var offset = (int)((pageIndex - 1) * pageSize);

            var listOpenSearchResult = new List<EmployeeFtsModel>();
            if (!string.IsNullOrEmpty(keyword))
            {
                listOpenSearchResult = await _employeeOpenSearchFtsService.SearchDocumentAsync(keyword, pageSize.Value, offset);
            }

            if (!listOpenSearchResult.Any()) return new List<EmployeeFts>();
            var employeeIds = listOpenSearchResult.Select(x => x.Id).ToList();

            return await _dbContext.EmployeesFts
                .Where(x => employeeIds.Contains(x.Id))
                .Skip(offset)
                .Take(pageSize.Value)
                .ToListAsync();
        }

        [HttpGet("v4")]
        public async Task<List<EmployeeFtsModel>> GetListEmployeeOpenSearchFtsv4(
            [FromQuery] string keyword,
            [FromQuery] int? pageSize,
            [FromQuery] int? pageIndex)
        {
            pageIndex = pageIndex ?? 1;
            pageSize = pageSize ?? 10;
            var offset = (int)((pageIndex - 1) * pageSize);
            return await _employeeOpenSearchFtsService.SearchDocumentAsync(keyword, pageSize.Value, offset);
        }

        [HttpPost("sync-data")]
        public async Task SyncDataToOpenSearch()
        {
            var pageSize = 1000;
            var pageIndex = 1;
            var totalCount = await _dbContext.Employees.CountAsync();
            var totalPage = (int)totalCount / pageSize;
            for (int page = 0; page < totalPage; page++)
            {
                Debug.WriteLine("Processing page:", page);
                var offset = (int)(page * pageSize);
                var listDocuments = await _dbContext.Employees.Skip(offset).Take(pageSize).Select(x =>
                    new EmployeeFtsModel
                    {
                        Id = x.Id,
                        BirthDay = x.BirthDay,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        FullName = x.FullName,
                    }).ToListAsync();
                await _employeeOpenSearchFtsService.IndexDocumentAsync(listDocuments);
                Debug.WriteLine("Finished page:", page);
            }
        }

    }
}
