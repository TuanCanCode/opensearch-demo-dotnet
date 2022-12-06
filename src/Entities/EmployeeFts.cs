using System.ComponentModel.DataAnnotations.Schema;

namespace OpenSearchDemo.Entities
{
    [Table("employees_fts")]
    public class EmployeeFts
    {
        public int Id { get; set; }

        public DateTime BirthDay { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }
    }
}
