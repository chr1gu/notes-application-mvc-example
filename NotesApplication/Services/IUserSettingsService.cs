using System.Collections.Generic;

namespace NotesApplication.Services
{
    public interface IUserSettingsService
    {
        IDictionary<string, string> GetAvailableThemeLabelByKeys();
        void ChangeTheme(string themeKey);
        string GetCurrentThemeKey();

        IDictionary<string, string> GetAvailableSortOrderLabelByKeys();
        bool ChangeSortOrder(string sortOrderKey);
        string GetCurrentSortOrderKey();

        void ChangeHideFinished(bool value);
        bool GetShouldHideFinished();
    }
}