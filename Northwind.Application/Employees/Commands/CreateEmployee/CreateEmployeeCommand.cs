using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Northwind.Application.Customers.Commands.CreateCustomer;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;

namespace Northwind.Application.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommand :IRequest<int>
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }

        public class Handler : IRequestHandler<CreateEmployeeCommand, int>
        {
            private readonly INorthwindDbContext _context;
            private readonly IMediator _mediator;

            public Handler(INorthwindDbContext context, IMediator mediator)
            {
                _context = context;
                _mediator = mediator;
            }

            public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
            {
                var entity = new Employee
                {
                    EmployeeId = request.EmployeeId,
                    LastName = request.LastName,
                    FirstName = request.FirstName,
                    Title = request.Title,
                    TitleOfCourtesy = request.TitleOfCourtesy
                };

                _context.Employees.Add(entity);

                await _context.SaveChangesAsync(cancellationToken);

                //await _mediator.Publish(new CustomerCreated { CustomerId = entity.EmployeeId }, cancellationToken);

                return entity.EmployeeId;
            }
        }
    }
}
