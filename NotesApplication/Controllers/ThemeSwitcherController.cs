using Microsoft.AspNetCore.Mvc;
using NotesApplication.Services;

namespace NotesApplication.Controllers
{
    public class ThemeSwitcherController : Controller
    {
        private readonly IThemeService themeService;

        public ThemeSwitcherController(IThemeService themeService)
        {
            this.themeService = themeService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Update(string id)
        {
            themeService.ChangeTheme(id);
            
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                referer = "/";
            }

            return Redirect(referer);
        }
    }
}