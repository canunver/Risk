using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Services;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using Risk.net.WebUI.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Konfigurasyon işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class KonfigurasyonController : GenelController
    {
        /// <summary>
        /// IKonfigurasyonService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IKonfigurasyonService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.KonfigurasyonController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public KonfigurasyonController(IKonfigurasyonService service,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// Konfigurasyon View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI,YETKILIRISKGOREVLISI")]
        public IActionResult Index()
        {
            bool kayitYetkisi = false;
            bool onayYetkisi = false;

            if (Arac.YetkisiVarmi("RISKSEKRETARYASI", _kullanan))
                kayitYetkisi = true;
            if (Arac.YetkisiVarmi("YETKILIRISKGOREVLISI", _kullanan))
                onayYetkisi = true;

            //_service.EtkiKriteriAdiVer(_kullanan, 1, "FINANSAL");
            //_service.YapisalRiskSeviyesiVer(_kullanan, 4, 4);

            ViewBag.KayitYetki = kayitYetkisi;
            ViewBag.OnayYetki = onayYetkisi;

            return View();
        }

        /// <summary>
        /// Listeden seçilen kaydın tüm bilgisini ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="durum"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> KayitGetir(int durum)
        {
            Sonuc sonuc = await _service.KayitGetirAsync(_kullanan, durum);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcı tarafından girilen bilgileri ilgili servise kayıt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Kaydet(Konfigurasyon form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın onaya gönderilme işlemini sağlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> OnayaGonder(string kod)
        {
            Konfigurasyon form = new Konfigurasyon();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.OnayaGonderdi;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın onaylama işlemini sağlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Onayla(Konfigurasyon form)
        {
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın yapısal risk seviyesini getiren metod
        /// </summary>
        /// <param name="etki"></param>
        /// <param name="olasilik"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> YapisalRiskSeviyesiVer(int etki, int olasilik)
        {
            object sonuc = await _service.YapisalRiskSeviyesiVer(_kullanan, etki, olasilik);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın etki kriter adını getiren metod
        /// </summary>
        /// <param name="seviye"></param>
        /// <param name="riskKategorisi"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> EtkiKriteriAdiVer(int seviye, string riskKategorisi)
        {
            object sonuc = await _service.EtkiKriteriAdiVer(_kullanan, seviye, riskKategorisi);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın artık risk seviyesini getiren metod
        /// </summary>
        /// <param name="artikRiskPuani"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> ArtikRiskSeviyesiVer(string artikRiskPuani)
        {
            double _artikRiskPuani = Arac.ConvertToDouble(artikRiskPuani.Replace(".", ","));

            object sonuc = await _service.ArtikRiskSeviyesiVer(_kullanan, _artikRiskPuani);

            return Ok(sonuc);
        }
    }
}
