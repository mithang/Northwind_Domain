using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Northwind.Application.Customers.Queries.GetCustomersList;
using Northwind.Application.Interfaces.Mapping;
using Northwind.Domain.Entities;

namespace Northwind.Application.Employees
{
    public class EmployeeLookupModel : IHaveCustomMapping
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }
        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<Employee, EmployeeLookupModel>()
                .ForMember(cDTO => cDTO.EmployeeId, opt => opt.MapFrom(c => c.EmployeeId))
                .ForMember(cDTO => cDTO.LastName, opt => opt.MapFrom(c => c.LastName))
                .ForMember(cDTO => cDTO.FirstName, opt => opt.MapFrom(c => c.FirstName))
                .ForMember(cDTO => cDTO.Title, opt => opt.MapFrom(c => c.Title))
                .ForMember(cDTO => cDTO.TitleOfCourtesy, opt => opt.MapFrom(c => c.TitleOfCourtesy));
        }
    }
}
