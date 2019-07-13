using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.API.Controllers
{
    public class ReponseResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public dynamic Data { get; set; }

        public ReponseResult ReponseValid(bool IsSuccess=false, string Message="",int TotalItems=0, int PageSize=1,dynamic Data=null)
        {
            return new ReponseResult
            {
                IsSuccess = IsSuccess,
                Message = Message,
                TotalItems = TotalItems,
                PageSize = PageSize,
                Data = Data
            };
        }
    }

    [ApiController]
    [Route("api/[controller]/[action]")]
    public abstract class BaseController : Controller
    {
        private IMediator _mediator;
        private ReponseResult _reponseResult;
        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());

        public ReponseResult ReponseResult {
            get
            {
                return new ReponseResult();
            }
            set
            {
                _reponseResult = value;
            }
        }
    }
}
