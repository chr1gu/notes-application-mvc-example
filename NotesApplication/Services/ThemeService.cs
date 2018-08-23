using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NotesApplication.Services
{
    public class ThemeService : IThemeService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ThemeService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetCurrentThemeKey()
        {
            var themes = GetAvailableThemeLabelByKeys();
            var session = httpContextAccessor.HttpContext.Session;
            var theme = session.GetString("theme") ?? string.Empty;

            return themes.ContainsKey(theme) ? theme : themes.First().Key;
        }
        
        public IDictionary<string, string> GetAvailableThemeLabelByKeys()
        {
            // don't forget to add a matching bootstrap theme in the css folder
            return new Dictionary<string, string>
            {
                {"default", "Default Theme"},
                {"spacelab", "Spacelab Theme"},
                {"darkly", "Darkly Theme"}
            };
        }

        public void ChangeTheme(string themeKey)
        {
            var session = httpContextAccessor.HttpContext.Session;

            session.SetString("theme", themeKey);
        }
    }
}