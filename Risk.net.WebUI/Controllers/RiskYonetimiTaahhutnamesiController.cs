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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// RiskYonetimiBeyannamesi iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    public class RiskYonetimiTaahhutnamesiController : GenelController
    {
        /// <summary>
        /// IRiskYonetimiBeyannamesiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskYonetimiBeyannamesiService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RiskYonetimiTaahhutnamesiController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskYonetimiTaahhutnamesiController(IRiskYonetimiBeyannamesiService service,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// RiskYonetimiBeyannamesi View sayfasý açýldýðýnda çalýþan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayý görme yetkisi olan kullanýcýlar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "SISTEMYONETICISI,RISKSEKRETARYASI,YETKILIRISKGOREVLISI,MERKEZKOORDINATOR,ILKOORDINATOR")]
        public IActionResult Index()
        {
            var onayYetki = Arac.YetkisiVarmi("MERKEZKOORDINATOR,ILKOORDINATOR", _kullanan);
            var tarihceYetki = Arac.YetkisiVarmi("RISKSEKRETARYASI,YETKILIRISKGOREVLISI", _kullanan);
            var planlamaYetki = _kullanan.AktifRolKod == "RISKSEKRETARYASI";

            ViewBag.OnayYetki = onayYetki;
            ViewBag.TarihceYetki = tarihceYetki;
            ViewBag.PlanlamaYetki = planlamaYetki;
            ViewBag.KoordinatorlukKod = _kullanan.KoordinatorlukKod;

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
        /// Kullaniciya ait kaydýn tüm bilgisini ilgili servis aracýlýðýyla getiren metod
        /// </summary>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DurumGetir()
        {
            var kriter = new RiskYonetimiBeyannamesi();
            kriter.Yil = DateTime.Now.Year - 1;
            kriter.KoordinatorlukKod = _kullanan.KoordinatorlukKod;
            //kriter.IslemYapanKod = _kullanan.PersonelKod;

            Sonuc sonuc = await _service.ListeleAsync(_kullanan, kriter);

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
        public async Task<IActionResult> Onayla()
        {
            var kriter = new RiskYonetimiBeyannamesi();
            kriter.Yil = DateTime.Now.Year - 1;
            kriter.KoordinatorlukKod = _kullanan.KoordinatorlukKod;
            kriter.IslemTarihi = DateTime.Now;
            kriter.IslemYapanKod = _kullanan.PersonelKod;
            kriter.IslemYapanRol = _kullanan.AktifRolKod;
            kriter.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.KaydetAsync(_kullanan, kriter);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden kayýtlarýn onay kaldýrma iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> OnayKaldir()
        {
            Sonuc sonuc = await _service.OnayKaldirAsync(_kullanan);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanýcýdan gelen kriterlere göre risk paneli için gereken bilgiyi ilgili servis aracýlýðýyla getiren metod
        /// </summary>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> TarihceGoster()
        {
            var kriter = new RiskYonetimiBeyannamesi();
            kriter.Yil = DateTime.Now.Year;

            Sonuc sonuc = await _service.ListeleAsync(_kullanan, kriter);

            return Ok(sonuc);
        }

        /// <summary>
        /// Hatýrlatma maili gönderilmesi iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> HatirlatmaMailiGonder()
        {
            Sonuc sonuc = await _service.HatirlatmaMailiGonder(_kullanan);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerBeyannemeYil(RiskEvreni form)
        {
            var donenDeger = new List<SelectListesi>();

            for (int i = DateTime.Now.Year + 1; i >= 2023; i--)
            {
                donenDeger.Add(new SelectListesi { id = i + "", text = i.ToString() });
            }

            return Ok(donenDeger);
        }

    }
}
