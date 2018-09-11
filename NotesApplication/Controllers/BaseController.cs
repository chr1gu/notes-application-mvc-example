using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace NotesApplication.Controllers
{
    public class BaseController : Controller
    {
        protected IActionResult UserFriendlyBadRequestError(string message)
        {
            return UserFriendlyError(message, HttpStatusCode.BadRequest);
        }
        
        protected IActionResult UserFriendlyNotFoundError(string message)
        {
            return UserFriendlyError(message, HttpStatusCode.NotFound);
        }

        protected IActionResult UserFriendlyError(string message, HttpStatusCode statusCode)
        {
            Response.StatusCode = (int) statusCode;
            
            var view = View("~/Views/Error/Index.cshtml", ErrorController.GetErrorViewModel(HttpContext, message));
            view.StatusCode = Response.StatusCode;

            return view;
        }
    }
}