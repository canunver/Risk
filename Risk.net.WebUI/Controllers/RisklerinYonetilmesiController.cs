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
    /// Risk Azaltma Planý iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]//Yetki Tamam
    public class RisklerinYonetilmesiController : GenelController
    {
        /// <summary>
        /// IRiskAzaltmaPlaniService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniService _service;
        /// <summary>
        /// IRiskAzaltmaPlaniRiskService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniRiskService _serviceRisk;
        /// <summary>
        /// IRiskAzaltmaPlaniNotService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniNotService _serviceNot;
        /// <summary>
        /// IRiskAzaltmaPlaniIliskiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniIliskiService _serviceIliski;
        /// <summary>
        /// IRiskAzaltmaPlaniIsbirligiBirimService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniIsbirligiBirimService _serviceIsbirligiBirim;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RisklerinYonetilmesiController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceRisk"></param>
        /// <param name="serviceNot"></param>
        /// <param name="serviceIliski"></param>
        /// <param name="serviceIsbirligiBirim"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RisklerinYonetilmesiController(IRiskAzaltmaPlaniService service,
                                    IRiskAzaltmaPlaniRiskService serviceRisk,
                                    IRiskAzaltmaPlaniNotService serviceNot,
                                    IRiskAzaltmaPlaniIliskiService serviceIliski,
                                    IRiskAzaltmaPlaniIsbirligiBirimService serviceIsbirligiBirim,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceRisk = serviceRisk;
            _serviceNot = serviceNot;
            _serviceIliski = serviceIliski;
            _serviceIsbirligiBirim = serviceIsbirligiBirim;
        }

        /// <summary>
        /// RiskAzaltmaPlani View sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.DurumListesi = Ortak.DurumListesiVer(_sharedResource, EnumTarihceIslemTur.RiskAzaltmaPlani, _kullanan);
            ViewBag.ArtikRiskSeviyesiListesi = Ortak.ArtikRiskSeviyesiListesiVer(_sharedResource);
            ViewBag.MevcutDurumListesi = Newtonsoft.Json.JsonConvert.SerializeObject(Ortak.RiskAzaltmaPlaniMevcutDurumListesi(_sharedResource));
            ViewBag.AzaltmaPlaniSorumlusuDegistir = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI,BIRIMAMIRI", _kullanan) ? "true" : "false";
            ViewBag.RiskSekretaryasi = _kullanan.AktifRolKod == "RISKSEKRETARYASI" ? "true" : "false";

            ViewBag.RiskSahibiAra = _kullanan.AktifRolKod == "RISKSEKRETARYASI";

            ViewBag.RiskKod = HttpContext.Request.Query["r"];

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
        /// <param name="riskAzaltmaPlaniKod"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> KayitGetir(string riskAzaltmaPlaniKod, string kod)
        {
            Sonuc sonuc = await _service.KayitGetirAsync(_kullanan, riskAzaltmaPlaniKod, kod);

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
        public async Task<ActionResult> Kaydet(RiskAzaltmaPlani form)
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
            RiskAzaltmaPlani form = new RiskAzaltmaPlani();
            form.Kod = kod;
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
            RiskAzaltmaPlani form = new RiskAzaltmaPlani
            {
                Kod = kod,
                Durum = (int)ENUMDurum.OnayaGonderdi
            };

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public IActionResult SelectListesiVerMevcutDurum()
        {
            return Ok(Ortak.RiskAzaltmaPlaniMevcutDurumListesi(_sharedResource));
        }

        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerAzaltmaPlani(string kod)
        {
            var sonuc = await _service.ListeleAsync(_kullanan);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (RiskEvreni item in sonuc.Liste)
            {
                if (item.Kod == kod)
                    continue;
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani, ekBilgi = Newtonsoft.Json.JsonConvert.SerializeObject(item.RiskYonetimi.RiskAzaltmaPlani) });
            }

            return Ok(donenDeger);
        }

        /// <summary>
        /// Listeden seçilen kaydýn tüm bilgisini ilgili servis aracýlýðýyla getiren metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RiskAzaltmaPlaniRiskKayitGetir(string kod)
        {
            Sonuc sonuc = await _serviceRisk.KayitGetirAsync(_kullanan, kod);

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
        public async Task<IActionResult> RiskAzaltmaPlaniIliskiKayitGetir(string kod)
        {
            Sonuc sonuc = await _serviceIliski.KayitGetirAsync(_kullanan, kod);

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
        public async Task<IActionResult> RiskAzaltmaPlaniIsbirligiBirimKayitGetir(string kod)
        {
            Sonuc sonuc = await _serviceIsbirligiBirim.KayitGetirAsync(_kullanan, kod);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen azaltma planý sorumlusunun güncellenmesi gonderilmesi iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> AzaltmaPlaniSorumlusuDegistir(RiskAzaltmaPlani form)
        {
            Sonuc sonuc = await _service.AzaltmaPlaniSorumlusuDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }
    }
}
