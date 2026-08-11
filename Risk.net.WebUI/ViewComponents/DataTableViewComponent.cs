using Microsoft.AspNetCore.Mvc;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// DataTable iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public class DataTableViewComponent : ViewComponent
    {
        /// <summary>
        /// DataTable ViewComponent sayfasý açýldýðýnda çalýþan metod.
        /// </summary>
        /// <param name="tabloAyar"></param>
        public IViewComponentResult Invoke(string tabloAyar)
        {
            DataTablesYapi tablo = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTablesYapi>(tabloAyar);

            return View(tablo);
        }
    }
}
