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
    /// Ýl Ýrtibat ofislerinin tanýmlama iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    public class TanimIlIrtibatOfisiController : GenelController
    {
        /// <summary>
        /// ITanimIlIrtibatOfisiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITanimIlIrtibatOfisiService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.TanimIlIrtibatOfisiController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TanimIlIrtibatOfisiController(ITanimIlIrtibatOfisiService service,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// TanimIlIrtibatOfisi View sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [YetkiKontrol(Yetkiler = "*")]
        public IActionResult Index()
        {
            if (Arac.YetkisiVarmi("RISKSEKRETARYASI", User))
                ViewBag.KayitYetki = true;
            else
                ViewBag.KayitYetki = false;

            if (Arac.YetkisiVarmi("YETKILIRISKGOREVLISI", User))
                ViewBag.OnayYetki = true;
            else
                ViewBag.OnayYetki = false;

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
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Kaydet([FromBody] TanimIlIrtibatOfisi form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn durumunu Pasif yaparak durumunun deðiþtirilmesini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Sil(string kod)
        {
            TanimIlIrtibatOfisi form = new TanimIlIrtibatOfisi();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn durumunu Onaylý yaparak durumunun deðiþtirilmesini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Onayla(string kod)
        {
            TanimIlIrtibatOfisi form = new TanimIlIrtibatOfisi();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Risk Kategorisi seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer()
        {
            var sonuc = await _service.ListeleAsync(_kullanan, (int)ENUMDurum.Onayli);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (TanimIlIrtibatOfisi item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Adi });
            }

            return Ok(donenDeger);
        }
    }
}
