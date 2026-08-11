using Microsoft.AspNetCore.Html;
using Risk.net.WebUI.Models;

namespace Risk.net.WebUI.Classes
{
    public static class Navigation
    {
        public static HtmlString AsRaw(this string value) => new HtmlString(value);

        public static string ToPage(this string href) => System.IO.Path.GetFileNameWithoutExtension(href)?.ToLower();

        public static bool IsVoid(this string href) => href?.ToLower() == NavigationModel.Void;

        public static bool IsRelatedTo(this ListItem item, string pageName) => item?.Type == ItemType.Parent && item?.Href?.ToPage() == pageName?.ToLower();

    }
}
