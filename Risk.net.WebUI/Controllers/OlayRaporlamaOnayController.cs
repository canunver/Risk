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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Olay Raporlama Onay iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class OlayRaporlamaOnayController : GenelController
    {
        /// <summary>
        /// IOlayRaporlamaService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IOlayRaporlamaService _service;
        /// <summary>
        /// IRiskEvreniService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniService _serviceRiskEvreni;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.OlayRaporlamaOnayController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceRiskEvreni"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public OlayRaporlamaOnayController(IOlayRaporlamaService service,
                                        IRiskEvreniService serviceRiskEvreni,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceRiskEvreni = serviceRiskEvreni;
        }

        /// <summary>
        /// OlayRaporlamaOnay View sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.DurumListesi = Ortak.DurumListesiVer(_sharedResource, EnumTarihceIslemTur.OlayRaporlama, _kullanan);
            ViewBag.KoordinatorlukKodu = _kullanan.KoordinatorlukKod;
            ViewBag.BirimKodu = _kullanan.BirimKod;

            return View();
        }

        /// <summary>
        /// Kullanýcý ekranýndan aldýðý sayfa no, kayýt sayýsý, sýralama alaný ve arama kriter 
        /// bilgileriyle ilgili servisten DataTables kontrolüne yüklemek üzere liste olarak getiren metod 
        /// </summary>
        /// <returns>
        /// Ok(JSON tipinde sunucudan gelen bilgi)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> TabloDoldur()
        {
            DataTablesParam dataTableInfo = new DataTablesParam(Request);

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo, true);

            return Ok(jsonData);
        }

        /// <summary>
        /// Listeden seçilen kaydýn tüm bilgisini ilgili servis aracýlýðýyla getiren metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> KayitGetir(string kod)
        {
            Sonuc sonuc = await _service.KayitGetirAsync(_kullanan, kod);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn geri gonderilmesi iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> GeriGonder(OlayRaporlama form)
        {
            form.Durum = (int)ENUMDurum.GeriGonderildi;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn reddedilme iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Reddet(OlayRaporlama form)
        {
            form.Durum = (int)ENUMDurum.Reddedildi;

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
        public async Task<IActionResult> Onayla(OlayRaporlama form)
        {
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kayýtlarýn hepsini onaylama iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> OnaylaToplu(string[] form)
        {
            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form, (int)ENUMDurum.Onayli);

            return Ok(sonuc);
        }
    }
}
