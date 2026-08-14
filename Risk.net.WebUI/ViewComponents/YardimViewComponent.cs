using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Risk.net.Data.Entities;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// Yardim işlemlerinin yapıldığı sayfa
    /// </summary>
    public class YardimViewComponent : ViewComponent
    {
        /// <summary>
        /// IYardimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IYardimService _service;
        /// <summary>
        /// Kullanıcı bilgisini taşıyan değişken
        /// </summary>
        /// <remarks></remarks>
        public readonly KullaniciDto _kullanan;

        /// <summary>
        /// <see cref="Risk.net.WebUI.ViewComponents.YardimViewComponent" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <remarks></remarks>
        public YardimViewComponent(IYardimService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _kullanan = Arac.KullaniciNesnesiOlustur(httpContextAccessor.HttpContext.User);
        }

        /// <summary>
        /// Yardim ViewComponent sayfası açıldığında çalışan metod.
        /// </summary>
        /// <param name="sayfaAdi"></param>
        public async Task<IViewComponentResult> InvokeAsync(string sayfaAdi)
        {
            bool kayitYetkisi = false;

            if (Arac.YetkisiVarmi("SISTEMYONETICISI", _kullanan))
                kayitYetkisi = true;

            ViewBag.KayitYetki = kayitYetkisi;

            Sonuc sonuc = await _service.KayitGetirAsync(null, sayfaAdi);
            sonuc.AnahtarAlan = sayfaAdi;
            return View(sonuc);
        }

    }
}
