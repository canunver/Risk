using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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
    /// Risk Evreni iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class RiskKaydiController : GenelController
    {
        /// <summary>
        /// IRiskEvreniService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RiskKaydiController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskKaydiController(IRiskEvreniService service,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// RiskEvreni View sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.DurumListesi = Ortak.DurumListesiVer(_sharedResource, EnumTarihceIslemTur.RiskEvreni, _kullanan);
            ViewBag.KoordinatorlukKodu = _kullanan.KoordinatorlukKod;
            ViewBag.BirimKodu = _kullanan.BirimKod;
            ViewBag.Pasif = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI", _kullanan);
            ViewBag.Sil = ViewBag.Pasif;
            ViewBag.JenerikRisk = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI", _kullanan) ? "true" : "false";
            ViewBag.RiskSekretaryasi = _kullanan.AktifRolKod == "RISKSEKRETARYASI" ? "true" : "false";

            ViewBag.KopyalaKod = HttpContext.Request.Query["kopyalaKod"];
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

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo, false, false);

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
        public async Task<ActionResult> Kaydet([FromBody] RiskEvreni form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydý pasif yapma iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Pasif(RiskEvreni form)
        {
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

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
        public async Task<IActionResult> Sil(RiskEvreni form)
        {
            form.Durum = (int)ENUMDurum.Sil;

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
            RiskEvreni form = new RiskEvreni();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.OnayaGonderdi;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen risk sahibinin güncellenmesi gonderilmesi iþlemini saðlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> RiskSahibiDegistir(RiskEvreni form)
        {
            Sonuc sonuc = await _service.RiskSahibiDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Verilen koþullara göre Risk evreni listelemesi yapar
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> RiskListesi(RiskEvreni kriter)
        {
            kriter.Durum = (int)ENUMDurum.Onayli;//Sadece onaylý riskler listelensin. !!Denetim Ekranýnda kullanýlýyor Melih 25.10.2023

            Sonuc sonuc = await _service.ListeleAsync(_kullanan, kriter);

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
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerRiskAdi(RiskEvreni form)
        {
            //Sorumlu birim deðiþtirilirse ilgili riski bulmak için
            string riskKod = form.Kod;

            form.Kod = "";
            form.Durum = (int)ENUMDurum.Onayli;

            var sonuc = await _service.ListeleAsync(_kullanan, form);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            var riskVar = false;
            foreach (RiskEvreni item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.RiskAdi });

                if (item.Kod == riskKod)
                    riskVar = true;
            }

            if (!riskVar && !string.IsNullOrWhiteSpace(riskKod))
            {
                sonuc = await _service.KayitGetirAsync(_kullanan, riskKod);

                if (sonuc.Nesne != null)
                {
                    var r = (RiskEvreni)sonuc.Nesne;

                    donenDeger.Add(new SelectListesi { id = r.Kod, text = r.RiskAdi });
                }
            }


            return Ok(donenDeger);
        }

        /// <summary>
        /// Onaylý risklerde deðiþiklik yapýlmak istenildiðinde uyarý bilgisini çaðrýlan metod
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

                //Risk Kaydý ekranýnda Koord Birim Risk Adý alanlarýný riskin sahibi Onaylý bir kayýtta deðiþtirirse süreç en baþtan baþlayacak ve diðer ekranlar da sýfýrlanacak.
                //(Uyarý da versin emin misiniz diye diðer ekranlardaki bilgileri sýfýrlayacaðýna dair) Ama kalan alanlarda deðiþiklik yapýlýrsa diðer ekranlar sýfýrlanmayacak.
                if (eskiKayit.Durum == (int)ENUMDurum.Onayli && (eskiKayit.KoordinatorlukKod + "" != form.KoordinatorlukKod + "" || eskiKayit.BirimKod + "" != form.BirimKod + ""))
                {
                    return Ok(new Sonuc(ENUMIslemDurum.Uyari, "", ""));
                }
            }
            return Ok(new Sonuc(ENUMIslemDurum.Basarili, "", ""));
        }
    }
}
