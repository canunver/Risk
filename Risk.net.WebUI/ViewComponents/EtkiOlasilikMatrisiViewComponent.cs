using Microsoft.AspNetCore.Mvc;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// EtkiOlasilikMatrisi iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public class EtkiOlasilikMatrisiViewComponent : ViewComponent
    {
        /// <summary>
        /// IKonfigurasyonService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IKonfigurasyonService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.ViewComponents.EtkiOlasilikMatrisi" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <remarks></remarks>
        public EtkiOlasilikMatrisiViewComponent(IKonfigurasyonService service)
        {
            _service = service;
        }

        /// <summary>
        /// EtkiOlasilikMatrisi ViewComponent sayfasý açýldýðýnda çalýþan metod.
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Sonuc sonuc = await _service.KayitGetirAsync(null, (int)ENUMDurum.Onayli);

            return View(sonuc);
        }

    }
}
