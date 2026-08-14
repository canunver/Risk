using Microsoft.AspNetCore.Mvc;
using Risk.net.WebUI.Models;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// Tanımlama Menüsü işlemlerinin yapıldığı sayfa
    /// </summary>
    public class TanimMenuViewComponent : ViewComponent
    {
        /// <summary>
        /// TanimMenu ViewComponent sayfası açıldığında çalışan metod.
        /// </summary>
        public IViewComponentResult Invoke()
        {
            string navFile = "navTanim.json";

            var items = NavigationModel.BuildNavigation(navFile, User);

            return View(items);
        }
    }
}
