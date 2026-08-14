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
    /// Risk Evreni Onay işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class RiskKaydiOnayController : GenelController
    {
        /// <summary>
        /// IRiskEvreniService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniService _service;
        /// <summary>
        /// IKonfigurasyonService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IKonfigurasyonService _serviceKonfigurasyon;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RiskKaydiOnayController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceKonfigurasyon"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskKaydiOnayController(IRiskEvreniService service,
                                        IKonfigurasyonService serviceKonfigurasyon,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _serviceKonfigurasyon = serviceKonfigurasyon;
            _service = service;
        }

        /// <summary>
        /// RiskEvreniOnay View sayfası açıldığında çalışan metod
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.DurumListesi = Ortak.DurumListesiVer(_sharedResource, EnumTarihceIslemTur.RiskEvreni, _kullanan);
            ViewBag.AktifRol = _kullanan.AktifRolKod;

            var sonuc = _serviceKonfigurasyon.KayitGetirAsync(_kullanan, (int)ENUMDurum.Onayli);
            ViewBag.Konfigurasyon = sonuc.Result.IslemSonuc ? Newtonsoft.Json.JsonConvert.SerializeObject(sonuc.Result.Nesne) : "";

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

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo, true, false);

            return Ok(jsonData);
        }

        /// <summary>
        /// Listeden seçilen kaydın tüm bilgisini ilgili servis aracılığıyla getiren metod
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
        /// Onaylama yapacak kişinin form üzerinde yaptığı değişiklikleri bulmak için çağrılan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> DegisenAlanlariGetir(RiskEvreni form)
        {
            var sonuc = await _service.DegisenAlanlariGetirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın geri gonderilmesi işlemini sağlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> GeriGonder(RiskEvreni form)
        {
            form.Durum = (int)ENUMDurum.GeriGonderildi;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın reddedilme işlemini sağlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Reddet(RiskEvreni form)
        {
            form.Durum = (int)ENUMDurum.Reddedildi;

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
        public async Task<IActionResult> Onayla(RiskEvreni form)
        {
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kayıtların hepsini onaylama işlemini sağlayan metod
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
