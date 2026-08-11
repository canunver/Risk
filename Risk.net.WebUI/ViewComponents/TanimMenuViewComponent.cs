using Microsoft.AspNetCore.Mvc;
using Risk.net.WebUI.Models;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// Tanýmlama Menüsü iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public class TanimMenuViewComponent : ViewComponent
    {
        /// <summary>
        /// TanimMenu ViewComponent sayfasý açýldýðýnda çalýþan metod.
        /// </summary>
        public IViewComponentResult Invoke()
        {
            string navFile = "navTanim.json";

            var items = NavigationModel.BuildNavigation(navFile, User);

            return View(items);
        }
    }
}
