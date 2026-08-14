using Microsoft.AspNetCore.Mvc;
using Risk.net.WebUI.Models;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// Menü işlemlerinin yapıldığı sayfa
    /// </summary>
    public class NavigationViewComponent : ViewComponent
    {
        /// <summary>
        /// Menu ViewComponent sayfası açıldığında çalışan metod.
        /// </summary>
        public IViewComponentResult Invoke()
        {
            string navFile = "nav.json";

            var items = NavigationModel.BuildNavigation(navFile, User);

            return View(items);
        }
    }
}
