using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Risk.net.Data.Entities;
using Risk.net.Services;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using Risk.net.WebUI.Classes;
using Risk.net.WebUI.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Bildirim Sistemi işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class TestController : GenelController
    {
        /// <summary>
        /// IBildirimSistemiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBildirimSistemiService _service;
        /// <summary>
        /// IViewBildirimSistemiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBildirimSistemiService _serviceView;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.BildirimSistemiController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceView"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TestController(IBildirimSistemiService service,
                                IViewBildirimSistemiService serviceView,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceView = serviceView;
        }

        /// <summary>
        /// Test View sayfası açıldığında çalışan metod
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Kullanıcı ekranından aldığı sayfa no, kayıt sayısı, sıralama alanı ve arama kriter 
        /// bilgileriyle ilgili servisten DataTables kontrolüne yüklemek üzere liste olarak getiren metod 
        /// </summary>
        /// <returns>
        /// Ok(JSON tipinde sunucudan gelen bilgi)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> TabloDoldur()
        {
            DataTablesParam dataTableInfo = new DataTablesParam(Request);

            var jsonData = await _serviceView.TabloDoldurAsync(_kullanan, dataTableInfo);

            return Ok(jsonData);
        }

        /// <summary>
        /// Kullanıcıya ait bildirim sayısını ekranın üst kısmında göstermek için bildirim sayısını servisten alan metodtur
        /// </summary>
        /// <returns>
        /// Ok(JSON tipinde sunucudan gelen bilgi)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> BildirimSayisi()
        {
            int adet = await _serviceView.BildirimSayisiAsync(_kullanan);

            return Ok(adet);
        }

        /// <summary>
        /// Belge tiplerini listeleyen metod
        /// </summary>
        /// <param name="_sharedResource"></param>
        /// <returns>
        /// JSON tipinde bilgi döndürür
        /// </returns>
        public static string BelgeTipiListesiVer(IStringLocalizer<CustomResource> _sharedResource)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            donenDeger.Add(new SelectListesi { id = (int)EnumTarihceIslemTur.RiskEvreni + "", text = _sharedResource["EnumTarihceIslemTur.RiskEvreni"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumTarihceIslemTur.RiskYonetimi + "", text = _sharedResource["EnumTarihceIslemTur.RiskYonetimi"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumTarihceIslemTur.RiskAzaltmaPlani + "", text = _sharedResource["EnumTarihceIslemTur.RiskAzaltmaPlani"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumTarihceIslemTur.OlayRaporlama + "", text = _sharedResource["EnumTarihceIslemTur.OlayRaporlama"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumTarihceIslemTur.BulguYonetimiCevap + "", text = _sharedResource["EnumTarihceIslemTur.BulguYonetimiCevap"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumTarihceIslemTur.AnahtarRiskGostergesi + "", text = _sharedResource["EnumTarihceIslemTur.AnahtarRiskGostergesi"] });


            return Newtonsoft.Json.JsonConvert.SerializeObject(donenDeger);
        }

        /// <summary>
        /// Listeden seçilen kaydın mail gönderme işlemini sağlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> MailGonder(BildirimSistemi form)
        {
            Sonuc sonuc = await _service.MailGonderAsync(_kullanan, form);

            return Ok(sonuc);
        }
    }
}
