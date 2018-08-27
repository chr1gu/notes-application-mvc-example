using System;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NotesApplication.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString GetFiveStarRatingFromImportance(this IHtmlHelper htmlHelper, int importance)
        {
            var stars = Enumerable.Range(0, importance)
                .Select(i => "<span class=\"glyphicon glyphicon-star\" aria-hidden=\"true\"></span>");

            return new HtmlString(String.Concat(stars));
        }
    }
}