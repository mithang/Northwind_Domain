using System.Collections.Generic;

namespace Northwind.Application.Employees.Queries
{
    public class EmployeesListViewModel
    {
        public IList<EmployeeLookupModel> Employees { get; set; }
    }
}
