using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Northwind.Application.Customers.Commands.CreateCustomer;

namespace Northwind.Application.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeCommandValidator: AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeCommandValidator()
        {
            RuleFor(x => x.LastName).Length(5).NotEmpty();
            RuleFor(x => x.FirstName).MaximumLength(60);
        }
    }
}
