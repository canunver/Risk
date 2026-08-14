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
    /// Risk Yonetimi Onay işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class RisklerinDegerlendirilmesiOnayController : GenelController
    {
        /// <summary>
        /// IRiskYonetimiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskYonetimiService _service;
        /// <summary>
        /// IRiskYonetimiKontrolService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskYonetimiKontrolService _serviceKontrol;
        /// <summary>
        /// IKonfigurasyonService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IKonfigurasyonService _serviceKonfigurasyon;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RisklerinDegerlendirilmesiOnayController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceKontrol"></param>
        /// <param name="serviceKonfigurasyon"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RisklerinDegerlendirilmesiOnayController(IRiskYonetimiService service,
                                    IRiskYonetimiKontrolService serviceKontrol,
                                    IKonfigurasyonService serviceKonfigurasyon,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceKontrol = serviceKontrol;
            _serviceKonfigurasyon = serviceKonfigurasyon;
        }

        /// <summary>
        /// RiskYonetimiOnay View sayfası açıldığında çalışan metod
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.DurumListesi = Ortak.DurumListesiVer(_sharedResource, EnumTarihceIslemTur.RiskYonetimi, _kullanan);
            ViewBag.RiskeVerilecekCevapListesi = Newtonsoft.Json.JsonConvert.SerializeObject(RiskeVerilecekCevapListesi());
            ViewBag.EtkiOlasilikMatrisi = Newtonsoft.Json.JsonConvert.SerializeObject(_serviceKonfigurasyon.EtkiOlasilikMatrisiVer(_kullanan).Result);

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

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo, true);

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
        /// Kullanıcı tarafından girilen bilgileri ilgili servise kayıt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Kaydet(RiskYonetimi form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

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
        public async Task<IActionResult> GeriGonder(RiskYonetimi form)
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
        public async Task<IActionResult> Reddet(RiskYonetimi form)
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
        public async Task<IActionResult> Onayla(RiskYonetimi form)
        {
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }


        /// <summary>
        /// Listeden seçilen kaydın tüm bilgisini ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RiskYonetimiKontrolKayitGetir(string kod)
        {
            Sonuc sonuc = await _serviceKontrol.KayitGetirAsync(_kullanan, kod);

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
        public async Task<ActionResult> RiskYonetimiKontrolKaydet(RiskYonetimiKontrol form)
        {
            form.Durum = (int)ENUMDurum.Onayli;
            Sonuc sonuc = await _serviceKontrol.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın silme işlemini sağlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> RiskYonetimiKontrolSil(string kod)
        {
            RiskYonetimiKontrol form = new RiskYonetimiKontrol();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _serviceKontrol.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerKontrol(string riskYonetimiKod)
        {
            var sonuc = await _serviceKontrol.ListeleAsync(_kullanan, riskYonetimiKod, (int)ENUMDurum.Aktif);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (RiskYonetimiKontrol item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Tanim });
            }

            return Ok(donenDeger);
        }

        /// <summary>
        /// Sayfada gösterilecek olan Risklere Verilecek Cevap listesinin doldurulması için
        /// </summary>
        /// <returns>
        /// List<SelectListesi>
        /// </returns>
        /// <remarks></remarks>
        public List<SelectListesi> RiskeVerilecekCevapListesi()
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>
            {
                new SelectListesi { id = (int)EnumRiskYonetimiRiskeVerilecekCevap.TransferEt + "", text = _sharedResource["RiskYonetimi.RiskeVerilecekCevap.TransferEt"] },
                new SelectListesi { id = (int)EnumRiskYonetimiRiskeVerilecekCevap.KabulEt + "", text = _sharedResource["RiskYonetimi.RiskeVerilecekCevap.KabulEt"] },
                new SelectListesi { id = (int)EnumRiskYonetimiRiskeVerilecekCevap.Reddet + "", text = _sharedResource["RiskYonetimi.RiskeVerilecekCevap.Reddet"] },
                new SelectListesi { id = (int)EnumRiskYonetimiRiskeVerilecekCevap.Azalt + "", text = _sharedResource["RiskYonetimi.RiskeVerilecekCevap.Azalt"] }
            };

            return donenDeger;
        }

        /// <summary>
        /// Onaylama yapacak kişinin form üzerinde yaptığı değişiklikleri bulmak için çağrılan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> DegisenAlanlariGetir(RiskYonetimi form)
        {
            var sonuc = await _service.DegisenAlanlariGetirAsync(_kullanan, form);

            return Ok(sonuc);
        }

    }
}
