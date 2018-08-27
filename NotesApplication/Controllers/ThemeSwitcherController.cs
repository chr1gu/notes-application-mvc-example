using Microsoft.AspNetCore.Mvc;
using NotesApplication.Services;

namespace NotesApplication.Controllers
{
    public class ThemeSwitcherController : Controller
    {
        private readonly IUserSettingsService _userSettingsService;

        public ThemeSwitcherController(IUserSettingsService userSettingsService)
        {
            _userSettingsService = userSettingsService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Update(string id)
        {
            _userSettingsService.ChangeTheme(id);
            
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                referer = "/";
            }

            return Redirect(referer);
        }
    }
}