using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Customers.Queries.GetCustomersList;
using Northwind.Application.Employees.Queries;
using Northwind.Application.Interfaces;

namespace Northwind.Application.Employees
{
    public class GetEmployeesListQueryHandler : IRequestHandler<GetEmployeesListQuery, EmployeesListViewModel>
    {
        private readonly INorthwindDbContext _context;
        private readonly IMapper _mapper;

        public GetEmployeesListQueryHandler(INorthwindDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EmployeesListViewModel> Handle(GetEmployeesListQuery request, CancellationToken cancellationToken)
        {
            return new EmployeesListViewModel
            {
                Employees = await _context.Employees.ProjectTo<EmployeeLookupModel>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken)
            };
        }
    }
}
