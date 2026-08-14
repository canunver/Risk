using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Risk.net.Data.Entities;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using Risk.net.WebUI.Classes;
using System.Threading.Tasks;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// DenetimGorevlendirme işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI")]
    public class DenetimGorevlendirmeViewComponent : ViewComponent
    {
        /// <summary>
        /// IDenetimGorevlendirmeService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimGorevlendirmeService _service;
        /// <summary>
        /// Kullanıcı bilgisini taşıyan değişken
        /// </summary>
        /// <remarks></remarks>
        public readonly KullaniciDto _kullanan;

        /// <summary>
        /// <see cref="Risk.net.WebUI.ViewComponents.DenetimGorevlendirmeViewComponent" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <remarks></remarks>
        public DenetimGorevlendirmeViewComponent(IDenetimGorevlendirmeService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _kullanan = Arac.KullaniciNesnesiOlustur(httpContextAccessor.HttpContext.User);
        }

        /// <summary>
        /// DenetimGorevlendirme ViewComponent sayfası açıldığında çalışan metod.
        /// </summary>
        /// <param name="sayfaAdi"></param>
        public async Task<IViewComponentResult> InvokeAsync(string denetimKodu, int tip)
        {
            DenetimGorevlendirme nesne = new DenetimGorevlendirme();
            nesne.DenetimKod = denetimKodu;
            nesne.Tip = tip;

            return View(nesne);
        }

    }
}
