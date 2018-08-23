using System.Collections.Generic;

namespace NotesApplication.Services
{
    public interface IThemeService
    {
        IDictionary<string, string> GetAvailableThemeLabelByKeys();
        
        void ChangeTheme(string themeKey);
        
        string GetCurrentThemeKey();
    }
}