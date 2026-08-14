using Microsoft.AspNetCore.Mvc;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// KontrolKriterMatrisi işlemlerinin yapıldığı sayfa
    /// </summary>
    public class KontrolKriterMatrisiViewComponent : ViewComponent
    {
        /// <summary>
        /// KontrolKriterMatrisi ViewComponent sayfası açıldığında çalışan metod.
        /// </summary>
        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
