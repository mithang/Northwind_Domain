using MediatR;

namespace Northwind.Application.Employees.Queries
{
    public class GetEmployeesListQuery : IRequest<EmployeesListViewModel>
    {
    }
}
