using Microsoft.AspNetCore.Mvc;
using Risk.net.WebUI.Models;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// Menü iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public class NavigationViewComponent : ViewComponent
    {
        /// <summary>
        /// Menu ViewComponent sayfasý açýldýðýnda çalýþan metod.
        /// </summary>
        public IViewComponentResult Invoke()
        {
            string navFile = "nav.json";

            var items = NavigationModel.BuildNavigation(navFile, User);

            return View(items);
        }
    }
}
