using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Services;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using Risk.net.WebUI.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Anahtar Risk Göstergesi iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]//Yetki Tamam
    public class AnahtarRiskGostergesiController : GenelController
    {
        /// <summary>
        /// IAnahtarRiskGostergesiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IAnahtarRiskGostergesiService _service;
        /// <summary>
        /// IAnahtarRiskGostergesiDonemService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IAnahtarRiskGostergesiDonemService _serviceDonem;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.AnahtarRiskGostergesiController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceDonem"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public AnahtarRiskGostergesiController(IAnahtarRiskGostergesiService service,
                                    IAnahtarRiskGostergesiDonemService serviceDonem,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceDonem = serviceDonem;
        }

        /// <summary>
        /// AnahtarRiskGostergesi View sayfasý açýldýðýnda çalýþan metod.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            ViewBag.PeriyotListesi = Ortak.AnahtarRiskGostergesiDonemPeriyotListesiVer(_sharedResource);
            ViewBag.RiskKod = HttpContext.Request.Query["r"];

            string argKod = HttpContext.Request.Query["a"];

            if (!string.IsNullOrWhiteSpace(argKod))
            {
                var sonuc = await _service.RiskNoGetirAsync(_kullanan, argKod);
                if (sonuc.IslemSonuc)
                    ViewBag.RiskKod = sonuc.AnahtarAlan;
            }

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

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo);

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
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerOneri(RiskEvreni form)
        {
            Sonuc sonuc = await _service.ListeleOneriAsync(_kullanan, form);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (RiskEvreni item in sonuc.Liste)
            {
                if (item.AnahtarRiskGostergesi != null)
                    donenDeger.Add(new SelectListesi { id = item.AnahtarRiskGostergesi.Kod, text = item.AnahtarRiskGostergesi.Adi });
                else
                    donenDeger.Add(new SelectListesi { id = "", text = item.AnahtarRiskGostergesiAdi });

            }

            return Ok(donenDeger);
        }

        /// <summary>
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Kaydet(AnahtarRiskGostergesi form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn silme iþlemini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Sil(string kod)
        {
            var form = new AnahtarRiskGostergesi();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn silme iþlemini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> DonemSil(string kod)
        {
            AnahtarRiskGostergesiDonem kriter = new AnahtarRiskGostergesiDonem();
            kriter.Kod = kod;
            Sonuc sonuc = await _serviceDonem.SilAsync(_kullanan, kriter);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> BosDoluSelectListesiVer()
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>
            {
                new SelectListesi { id = 0 + "", text = "Tümü" },
                new SelectListesi { id = 1 + "", text = "Boþ Kayýtlar" },
                new SelectListesi { id = 2 + "", text = "Dolu Kayýtlar" },
            };

            return Ok(donenDeger);
        }
    }
}
