using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Risk.net.Services;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Functions;
using Risk.net.WebUI.Classes;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Risk Sahibini Değiştir işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class RiskSahibiDegistirController : GenelController
    {
        /// <summary>
        /// IRiskSahibiDegistirService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskSahibiDegistirService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RiskSahibiDegistirController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceKontrol"></param>
        /// <param name="serviceKonfigurasyon"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskSahibiDegistirController(IRiskSahibiDegistirService service,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// RiskSahibiDegistir View sayfası açıldığında çalışan metod
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.RiskSahibiYetki = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI", _kullanan);

            return View();
        }

        /// <summary>
        /// Kullanıcı tarafından girilen bilgileri ilgili servise kayıt edilmesi için gönderen metod
        /// </summary>
        /// <param name="eskiKod"></param>
        /// <param name="yeniKod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Kaydet(string eskiKod, string yenikod)
        {
            if (!Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI", _kullanan))
                eskiKod = _kullanan.PersonelKod;

            Sonuc sonuc = await _service.KaydetAsync(_kullanan, eskiKod, yenikod);

            return Ok(sonuc);
        }

    }
}
