using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotesApplication.Models.ViewModels;

namespace NotesApplication.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(int? statusCode = null, string message = null)
        {
            if (statusCode.HasValue)
            {
                Response.StatusCode = statusCode.Value;
            }

            return View(GetErrorViewModel(HttpContext, message));
        }

        public static ErrorViewModel GetErrorViewModel(HttpContext httpContext, string message)
        {
            return new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? httpContext.TraceIdentifier,
                StatusCode = httpContext.Response.StatusCode,
                Message = string.IsNullOrEmpty(message) ? "An error occurred while processing your request!" : message
            };
        }
    }
}