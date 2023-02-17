using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using MagicVilla_VillaApi.Models;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("api/v{version:apiVersion}/Error")]
    [ApiController]
    [ApiVersionNeutral]
    public class ErrorController : ControllerBase
    {
        [HttpGet("/error")]
        public ActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = context.Error.Message;
            var stackTrace = context.Error.StackTrace;
            return Problem();
        }
    }
}
