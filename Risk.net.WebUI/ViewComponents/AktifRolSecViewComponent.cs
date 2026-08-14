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
    /// AktifRolSec işlemlerinin yapıldığı sayfa
    /// </summary>
    public class AktifRolSecViewComponent : ViewComponent
    {
        /// <summary>
        /// IViewYetkiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewYetkiService _service;
        /// <summary>
        /// Kullanıcı bilgisini taşıyan değişken
        /// </summary>
        /// <remarks></remarks>
        public readonly KullaniciDto _kullanan;

        /// <summary>
        /// <see cref="Risk.net.WebUI.ViewComponents.AktifRolSecViewComponent" /> 'ın yeni bir örneğini başlatan sınıf
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
        /// AktifRolSec ViewComponent sayfası açıldığında çalışan metod.
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
