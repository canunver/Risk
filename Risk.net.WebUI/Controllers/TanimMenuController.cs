using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Yazılımın sol kısmında gösterilen menü
    /// </summary>
    public class TanimMenuController : Controller
    {
        /// <summary>
        /// TanimMenu (Sol Menu) View sayfası açıldığında çalışan metod
        /// </summary>
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
