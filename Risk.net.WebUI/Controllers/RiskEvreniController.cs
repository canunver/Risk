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
    /// RiskIzleme iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class RiskEvreniController : GenelController
    {
        /// <summary>
        /// IRiskEvreniService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniService _service;
        /// <summary>
        /// IKonfigurasyonService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IKonfigurasyonService _serviceKonfigurasyon;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RiskEvreniController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskEvreniController(IRiskEvreniService service,
                                        IKonfigurasyonService serviceKonfigurasyon,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceKonfigurasyon = serviceKonfigurasyon;
        }

        /// <summary>
        /// RiskÝzleme View sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.DurumListesi = Ortak.DurumListesiVer(_sharedResource, EnumTarihceIslemTur.RiskEvreni, _kullanan);
            ViewBag.KoordinatorlukKodu = _kullanan.KoordinatorlukKod;
            ViewBag.BirimKodu = _kullanan.BirimKod;
            ViewBag.RiskTuruListesi = Ortak.RiskTuruListesiVer(_sharedResource);
            ViewBag.EtkiOlasilikMatrisi = Newtonsoft.Json.JsonConvert.SerializeObject(_serviceKonfigurasyon.EtkiOlasilikMatrisiVer(_kullanan).Result);
            ViewBag.RiskeVerilecekCevapListesi = Newtonsoft.Json.JsonConvert.SerializeObject(Ortak.RiskeVerilecekCevapListesi(_sharedResource));
            ViewBag.RiskAzaltmaPlaniMevcutDurumListesi = Newtonsoft.Json.JsonConvert.SerializeObject(Ortak.RiskAzaltmaPlaniMevcutDurumListesi(_sharedResource));
            ViewBag.ArgPeriyotListesi = Ortak.AnahtarRiskGostergesiDonemPeriyotListesiVer(_sharedResource);

            ViewBag.RiskSahibiAra = _kullanan.AktifRolKod == "RISKSEKRETARYASI";

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

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo, false, true);

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
            Sonuc sonuc = await _service.KayitGetirHepsiAsync(_kullanan, kod);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerRiskTuru()
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            donenDeger.Add(new SelectListesi { id = (int)EnumRiskEvreniRiskTuru.Tehdit + "", text = _sharedResource["RiskEvreni.RiskTuru.Tehdit"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumRiskEvreniRiskTuru.Firsat + "", text = _sharedResource["RiskEvreni.RiskTuru.Firsat"] });

            return Ok(donenDeger);
        }

        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerRiskTanimi(RiskEvreni form)
        {
            form.Durum = (int)ENUMDurum.Onayli;

            var sonuc = await _service.ListeleAsync(_kullanan, form);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (RiskEvreni item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.RiskTanimi });
            }

            return Ok(donenDeger);
        }

        /// <summary>
        /// Ekranlardaki seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> BaskasiAdinaRiskSelectListesiVer()
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>
            {
                new SelectListesi { id = 0 + "", text = "Tümü" },
                new SelectListesi { id = 1 + "", text = "Baþkasý adýna kaydettiklerim" },
                new SelectListesi { id = 2 + "", text = "Risk sahibi olduklarým" },
                new SelectListesi { id = 3 + "", text = "Kaydettigim bütün riskler" },
            };

            return Ok(donenDeger);
        }


        /// <summary>
        /// Ekranlardaki seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> KontrolEdildiSelectListesiVer()
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>
            {
                new SelectListesi { id = 0 + "", text = "Tümü" },
                new SelectListesi { id = 1 + "", text = "Evet" },
                new SelectListesi { id = 2 + "", text = "Hayýr" },
            };

            return Ok(donenDeger);
        }

    }
}
