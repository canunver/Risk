using Risk.net.Utilities.Functions;
using Risk.net.WebUI.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Risk.net.WebUI.Models
{
    public static class NavigationModel
    {
        private const string Underscore = "_";
        private const string Dash = "-";
        private const string Space = " ";
        private static readonly string Empty = string.Empty;
        public static readonly string Void = "javascript:void(0);";

        public static SmartNavigation BuildNavigation(string navFile, System.Security.Principal.IPrincipal User)
        {
            List<string> yetkiler = Arac.KullaniciYetkiGetir(User);

            var jsonText = File.ReadAllText(navFile);
            var navigation = NavigationBuilder.FromJson(jsonText);
            var menu = FillProperties(navigation.Lists, yetkiler);

            return new SmartNavigation(menu);
        }

        private static List<ListItem> FillProperties(IEnumerable<ListItem> items, List<string> yetkiler, ListItem parent = null)
        {
            var result = new List<ListItem>();

            foreach (var item in items)
            {
                item.Text ??= item.Title;
                item.Tags = string.Concat(parent?.Tags, Space, item.Title.ToLower()).Trim();

                var parentRoute = (Path.GetFileNameWithoutExtension(parent?.Text ?? Empty)?.Replace(Space, Underscore) ?? Empty).ToLower();
                var sanitizedHref = parent == null ? item.Href?.Replace(Dash, Empty) : item.Href?.Replace(parentRoute, parentRoute.Replace(Underscore, Empty)).Replace(Dash, Empty);
                var route = Path.GetFileNameWithoutExtension(sanitizedHref ?? Empty)?.Split(Underscore) ?? Array.Empty<string>();

                item.Route = route.Length > 1 ? $"/{route.First()}/{string.Join(Empty, route.Skip(1))}" : item.Href;

                item.I18n = parent == null
                    ? $"nav.{item.Title.ToLower().Replace(Space, Underscore)}"
                    : $"{parent.I18n}_{item.Title.ToLower().Replace(Space, Underscore)}";
                item.Type = parent == null ? item.Href == null ? ItemType.Category : ItemType.Single : item.Items.Any() ? ItemType.Parent : ItemType.Child;
                item.Items = FillProperties(item.Items, yetkiler, item);

                if (item.Href.IsVoid() && item.Items.Any())
                    item.Type = ItemType.Sibling;

                ///Yetki kontrolü
                bool hasAccess = false;
                if (item.Type != ItemType.Category)
                {
                    hasAccess = Arac.YetkiKontrolu(yetkiler, item.Roles);
                }
                else
                {
                    if (item.Items.Count > 0)
                        hasAccess = true;
                }

                if (hasAccess)
                    result.Add(item);
            }

            return result;
        }
    }
}
