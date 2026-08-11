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
    /// Risk Yonetimi iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class RisklerinDegerlendirilmesiController : GenelController
    {
        /// <summary>
        /// IRiskYonetimiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskYonetimiService _service;
        /// <summary>
        /// IRiskYonetimiKontrolService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskYonetimiKontrolService _serviceKontrol;
        /// <summary>
        /// IKonfigurasyonService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IKonfigurasyonService _serviceKonfigurasyon;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RisklerinDegerlendirilmesiController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceKontrol"></param>
        /// <param name="serviceKonfigurasyon"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RisklerinDegerlendirilmesiController(IRiskYonetimiService service,
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
        /// RiskYonetimi View sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        public async Task<IActionResult> Index()
        {
            ViewBag.DurumListesi = Ortak.DurumListesiVer(_sharedResource, EnumTarihceIslemTur.RiskYonetimi, _kullanan);
            ViewBag.RiskeVerilecekCevapListesi = Newtonsoft.Json.JsonConvert.SerializeObject(Ortak.RiskeVerilecekCevapListesi(_sharedResource));
            ViewBag.ArtikRiskSeviyesiListesi = Ortak.ArtikRiskSeviyesiListesiVer(_sharedResource);
            ViewBag.EtkiOlasilikMatrisi = Newtonsoft.Json.JsonConvert.SerializeObject(_serviceKonfigurasyon.EtkiOlasilikMatrisiVer(_kullanan).Result);
            ViewBag.PeriyotListesiArg = Ortak.AnahtarRiskGostergesiDonemPeriyotListesiVer(_sharedResource);
            ViewBag.RiskSahibiDegistir = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI,BIRIMAMIRI", _kullanan) ? "true" : "false";
            ViewBag.RiskSekretaryasi = _kullanan.AktifRolKod == "RISKSEKRETARYASI" ? "true" : "false";

            ViewBag.RiskKod = HttpContext.Request.Query["r"];

            string riskYonetimiKod = HttpContext.Request.Query["ry"];

            if (!string.IsNullOrWhiteSpace(riskYonetimiKod))
            {
                var sonuc = await _service.RiskNoGetirAsync(_kullanan, riskYonetimiKod);
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

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo, false);

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
        public async Task<ActionResult> Kaydet(RiskYonetimi form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn silme iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Sil(RiskYonetimi form)
        {
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn onaya gönderme iþlemini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> OnayaGonder(string kod)
        {
            RiskYonetimi form = new RiskYonetimi
            {
                Kod = kod,
                Durum = (int)ENUMDurum.OnayaGonderdi
            };

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn tüm bilgisini ilgili servis aracýlýðýyla getiren metod
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
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servise kayýt edilmesi için gönderen metod
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
        /// Listeden seçilen kaydýn silme iþlemini saðlayan metod
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
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
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
        /// Onaylý risk deðerlendirilmesi kaydýnda deðiþiklik yapýlmak istenildiðinde uyarý bilgisini çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> DegisiklikUyari(RiskEvreni form)
        {
            if (!string.IsNullOrWhiteSpace(form.Kod))
            {
                var kayit = await _service.KayitGetirAsync(_kullanan, form.Kod);
                var eskiKayit = (RiskEvreni)kayit.Nesne;

                if (eskiKayit.RiskYonetimi != null)
                {
                    //Riske verilecek cevap azlaltma planý oluþtursa ve burada deðiþiklik yapýlýrsa Onaylý bir kayýtta deðiþtirirse süreç en baþtan baþlayacak ve diðer ekranlar da sýfýrlanacak.
                    //(Uyarý da versin emin misiniz diye diðer ekranlardaki bilgileri sýfýrlayacaðýna dair) Ama kalan alanlarda deðiþiklik yapýlýrsa diðer ekranlar sýfýrlanmayacak.
                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli && eskiKayit.RiskYonetimi.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.Azalt && eskiKayit.RiskYonetimi.RiskeVerilecekCevap != form.RiskYonetimi.RiskeVerilecekCevap)
                    {
                        return Ok(new Sonuc(ENUMIslemDurum.Uyari, "", ""));
                    }
                }
            }
            return Ok(new Sonuc(ENUMIslemDurum.Basarili, "", ""));
        }

    }
}
