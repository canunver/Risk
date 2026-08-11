using Microsoft.AspNetCore.Mvc;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// KontrolKriterMatrisi iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public class KontrolKriterMatrisiViewComponent : ViewComponent
    {
        /// <summary>
        /// KontrolKriterMatrisi ViewComponent sayfasý açýldýðýnda çalýþan metod.
        /// </summary>
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
