using Microsoft.AspNetCore.Mvc;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// Tarihçe işlemlerinin yapıldığı sayfa
    /// </summary>
    public class TarihceViewComponent : ViewComponent
    {
        /// <summary>
        /// ITarihceService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITarihceService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.ViewComponents.TarihceViewComponent" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <remarks></remarks>
        public TarihceViewComponent(ITarihceService service)
        {
            _service = service;
        }

        /// <summary>
        /// Tarihce ViewComponent sayfası açıldığında çalışan metod.
        /// </summary>
        /// <param name="ilgiKod"></param>
        public async Task<IViewComponentResult> InvokeAsync(string ilgiKod)
        {
            Sonuc sonuc = await _service.ListeleAsync(ilgiKod);

            return View(sonuc);
        }

    }
}
