using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// AktifRolSec iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public class AktifRolSecViewComponent : ViewComponent
    {
        /// <summary>
        /// IViewYetkiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewYetkiService _service;
        /// <summary>
        /// Kullanýcý bilgisini taþýyan deðiþken
        /// </summary>
        /// <remarks></remarks>
        public readonly KullaniciDto _kullanan;

        /// <summary>
        /// <see cref="Risk.net.WebUI.ViewComponents.AktifRolSecViewComponent" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <remarks></remarks>
        public AktifRolSecViewComponent(IViewYetkiService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _kullanan = Arac.KullaniciNesnesiOlustur(httpContextAccessor.HttpContext.User);
        }

        /// <summary>
        /// AktifRolSec ViewComponent sayfasý açýldýðýnda çalýþan metod.
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Risk.net.Data.Entities.ViewYetki kriter = new Risk.net.Data.Entities.ViewYetki();
            kriter.PersonelKod = _kullanan.PersonelKod;
            Sonuc sonuc = await _service.ListeleAsync(new Utilities.Objects.KullaniciDto(), kriter);

            return View(sonuc);
        }

    }
}
