using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NotesApplication.Models;
using NotesApplication.Models.ViewModels;

namespace NotesApplication.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                Response.StatusCode = statusCode.Value;
            }

            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}