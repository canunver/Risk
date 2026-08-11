using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Yazýlýmýn sol kýsmýnda gösterilen menü
    /// </summary>
    public class TanimMenuController : Controller
    {
        /// <summary>
        /// TanimMenu (Sol Menu) View sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
