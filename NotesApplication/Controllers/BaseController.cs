using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace NotesApplication.Controllers
{
    public class BaseController : Controller
    {
        protected IActionResult UserFriendlyBadRequestError(string message)
        {
            Response.StatusCode = (int) HttpStatusCode.BadRequest;
            
            return View("~/Views/Error/Index.cshtml", ErrorController.GetErrorViewModel(HttpContext, message));
        }
        
        protected IActionResult UserFriendlyNotFoundError(string message)
        {
            Response.StatusCode = (int) HttpStatusCode.NotFound;
            
            return View("~/Views/Error/Index.cshtml", ErrorController.GetErrorViewModel(HttpContext, message));
        }
    }
}