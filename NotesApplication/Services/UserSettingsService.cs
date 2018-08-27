using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace NotesApplication.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserSettingsService(IHttpContextAccessor httpContextAccessor)
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
        
        public IDictionary<string, string> GetAvailableSortOrderLabelByKeys()
        {
            return new Dictionary<string, string>
            {
                {"finishedAt", "Finished date"},
                {"createdAt", "Creation date"},
                {"importance", "Importance"}
            };
        }

        public bool ChangeSortOrder(string sortOrderKey)
        {
            if (GetAvailableSortOrderLabelByKeys().ContainsKey(sortOrderKey))
            {
                var session = httpContextAccessor.HttpContext.Session;
                session.SetString("orderBy", sortOrderKey);

                return true;
            }

            return false;
        }

        public string GetCurrentSortOrderKey()
        {
            var sortOrders = GetAvailableSortOrderLabelByKeys();
            var session = httpContextAccessor.HttpContext.Session;
            var sortOrder = session.GetString("orderBy") ?? string.Empty;

            return sortOrders.ContainsKey(sortOrder) ? sortOrder : sortOrders.First().Key;
        }

        public void ChangeHideFinished(bool value)
        {
            var session = httpContextAccessor.HttpContext.Session;
            
            session.SetString("hideFinished", value.ToString());
        }

        public bool GetShouldHideFinished()
        {
            var session = httpContextAccessor.HttpContext.Session;

            return bool.TryParse(session.GetString("hideFinished"), out var hideFinished) && hideFinished;
        }
    }
}