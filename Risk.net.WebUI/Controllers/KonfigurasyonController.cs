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
    /// Konfigurasyon iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    public class KonfigurasyonController : GenelController
    {
        /// <summary>
        /// IKonfigurasyonService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IKonfigurasyonService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.KonfigurasyonController" /> 'ýn yeni bir örneðini baþlatan sýnýf
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
        /// Konfigurasyon View sayfasý açýldýðýnda çalýþan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayý görme yetkisi olan kullanýcýlar belirlenir
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
        /// Listeden seçilen kaydýn tüm bilgisini ilgili servis aracýlýðýyla getiren metod
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
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servise kayýt edilmesi için gönderen metod
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
        /// Listeden seçilen kaydýn onaya gönderilme iþlemini saðlayan metod
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
        /// Listeden seçilen kaydýn onaylama iþlemini saðlayan metod
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
        /// Listeden seçilen kaydýn yapýsal risk seviyesini getiren metod
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
        /// Listeden seçilen kaydýn etki kriter adýný getiren metod
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
        /// Listeden seçilen kaydýn artýk risk seviyesini getiren metod
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
