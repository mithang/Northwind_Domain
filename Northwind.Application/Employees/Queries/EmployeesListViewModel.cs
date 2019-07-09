using System;
using System.Collections.Generic;
using System.Text;
using Northwind.Application.Customers.Queries.GetCustomersList;
using Northwind.Domain.Entities;

namespace Northwind.Application.Employees
{
    public class EmployeesListViewModel
    {
        public IList<EmployeeLookupModel> Employees { get; set; }
    }
}
