using OpenSearch.Client;
using OpenSearchDemo.Models;

namespace OpenSearchDemo.Services.EmployeeFts
{
    public static class Mapping
    {
        public static CreateIndexDescriptor EmployeeFtsModelMapping(this CreateIndexDescriptor descriptor)
        {
            return descriptor
                .Map<EmployeeFtsModel>(m => m
                    // Flatten object mapping
                    .Properties(p => p
                        .Text(t => t.Name(n => n.Id))
                        .Date(t => t.Name(n => n.BirthDay))
                        .Text(t => t.Name(n => n.FullName))
                        .Text(t => t.Name(n => n.FirstName))
                        .Text(t => t.Name(n => n.LastName))
                    )
                );
        }
    }
}
