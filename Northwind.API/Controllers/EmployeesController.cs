using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Employees;
using Northwind.Application.Employees.Commands.CreateEmployee;
using Northwind.Application.Employees.Queries;

namespace Northwind.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "NhanVien", AuthenticationSchemes=JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize(Roles ="Admin")]
    //[Authorize]
    public class EmployeesController : BaseController
    {


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<EmployeesListViewModel>> GetAll()
        {
           
            return Ok(await Mediator.Send(new GetEmployeesListQuery()));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromBody]CreateEmployeeCommand command)
        {
           var r= await Mediator.Send(command);
            Debug.WriteLine($"XEM:{r}");

            return NoContent();
        }

        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload()
        //{
        //    var file = Request.Form.Files[0];
        //    var request = new UploadPhotoCommand(file.FileName, file.OpenReadStream());
        //    var url = await Mediator.Send(request);
        //    return Ok(new { url });
        //}
    }
}