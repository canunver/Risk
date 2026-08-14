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
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using Risk.net.WebUI.Classes;
using Risk.net.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Home işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class HomeController : GenelController
    {
        /// <summary>
        /// IGrafikService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IGrafikService _service;
        /// <summary>
        /// IViewBildirimSistemiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBildirimSistemiService _serviceViewBildirim;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.HomeController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceViewBildirim"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public HomeController(IGrafikService service,
                                IViewBildirimSistemiService serviceViewBildirim,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceViewBildirim = serviceViewBildirim;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Home View sayfası açıldığında çalışan metod.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                ViewBag.KoordinatorlukKodu = _kullanan.KoordinatorlukKod;
                ViewBag.BirimKodu = _kullanan.BirimKod;
                //ViewBag.BildirimSayisi = await _serviceViewBildirim.BildirimSayisiAsync(_kullanan);
            }
            catch
            {

                _httpContextAccessor.HttpContext.Response.Redirect("Account/Logout", true);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> BildirimSayisi()
        {
            var bildirimSayisi = await _serviceViewBildirim.BildirimSayisiAsync(_kullanan);

            return Ok(bildirimSayisi);
        }

        [HttpPost]
        public async Task<IActionResult> RiskListesi(string tur, string secilen)
        {
            //var bildirimSayisi = await _serviceViewBildirim.BildirimSayisiAsync(_kullanan);

            return Ok(tur + " " + secilen);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre risk paneli için gereken bilgiyi ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RiskPaneli(Grafik form)
        {
            Sonuc sonuc1 = await _service.YapisalRiskSeviyesiHazirlaAsync(_kullanan, form);
            Sonuc sonuc2 = await _service.ArtikRiskSeviyesiHazirlaAsync(_kullanan, form);

            Sonuc sonuc = new(sonuc1.Durum, new List<object>() { sonuc1.Liste, sonuc2.Liste });

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre bulgu durumunu ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> BulguDurumu(Grafik form)
        {
            Sonuc sonuc = await _service.BulguDurumuHazirlaAsync(_kullanan, form);

            foreach (Grafik item in sonuc.Liste)
            {
                item.Aciklama = _sharedResource["BulguYonetimi.Durum." + item.Aciklama];
            }

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre süreç için gereken bilgiyi ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Surec(Grafik form)
        {
            Sonuc sonuc = await _service.SurecHazirlaAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre bulgu önem düzeyini ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> BulguOnemDuzeyi(Grafik form)
        {
            Sonuc sonuc = await _service.BulguOnemDuzeyiHazirlaAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre yapısal risk seviyesini ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> YapisalRiskSeviyesi(Grafik form)
        {
            Sonuc sonuc = await _service.YapisalRiskSeviyesiHazirlaAsync(_kullanan, form);

            foreach (Grafik item in sonuc.Liste)
            {
                item.Aciklama = _sharedResource["Konfigurasyon.YapisalRiskSeviyeAdi." + item.Aciklama];
            }

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre artık risk seviyesini ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> ArtikRiskSeviyesi(Grafik form)
        {
            Sonuc sonuc = await _service.ArtikRiskSeviyesiHazirlaAsync(_kullanan, form);

            foreach (Grafik item in sonuc.Liste)
            {
                item.Aciklama = _sharedResource["Konfigurasyon.YapisalRiskSeviyeAdi." + item.Aciklama];
            }

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre risk puanını ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RiskPuani(Grafik form)
        {
            Sonuc sonuc = await _service.RiskPuaniHazirlaAsync(_kullanan, form);

            return Ok(sonuc);
        }

        //[HttpPost]
        //public async Task<IActionResult> RiskIzlemeMatrisi(Grafik form)
        //{
        //    //https://stackoverflow.com/questions/16989977/highcharts-scatter-with-4-quadrants/43440149

        //    Sonuc sonuc = await _service.RiskIzlemeMatrisiHazirlaAsync(_kullanan, form);

        //    List<object> bilgiler = new List<object>();

        //    foreach (GrafikIzlemeMatrisi item in sonuc.Liste)
        //    {
        //        int KontrolSeviyesi = 0;
        //        string artikRiskSeviyesiAdi = Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref KontrolSeviyesi);

        //        var YapisalValue = new
        //        {
        //            v = item.YapisalRiskSeviyesi,
        //            f = _sharedResource["Konfigurasyon.Alan.Etki" + item.YapisalRiskSeviyesi].Value
        //        };
        //        var KontrolValue = new
        //        {
        //            v = KontrolSeviyesi,
        //            f = artikRiskSeviyesiAdi + $" ({KontrolSeviyesi})"
        //        };

        //        var grup = "Riskleri izle";
        //        if (item.YapisalRiskSeviyesi < 3 && KontrolSeviyesi < 3)
        //            grup = "Riskleri izle";
        //        else if (item.YapisalRiskSeviyesi < 3 && KontrolSeviyesi > 2)
        //            grup = "Optimize Et/Vazgeç";
        //        else if (item.YapisalRiskSeviyesi > 2 && KontrolSeviyesi < 3)
        //            grup = "İyileştir";
        //        else if (item.YapisalRiskSeviyesi > 2 && KontrolSeviyesi > 2)
        //            grup = "Kontrolleri İzle";

        //        bilgiler.Add(new
        //        {
        //            Kod = item.RiskNo,
        //            Kontrol = KontrolValue,
        //            Yapisal = YapisalValue,
        //            Grup = grup,

        //        });
        //    }

        //    sonuc.Liste = bilgiler;

        //    return Ok(sonuc);
        //}

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre risk kategorilerini ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RiskKategorileri(Grafik form)
        {
            Sonuc sonuc = await _service.RiskKategorileriHazirlaAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre bulgu önem düzeyini ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AnahtarRiskGostergesi(Grafik form)
        {
            List<GrafikAnahtarRiskGostergesiDto> donenDeger = new List<GrafikAnahtarRiskGostergesiDto>();


            Sonuc sonuc = await _service.AnahtarRiskGostergesiHazirlaAsync(_kullanan, form);
            //'Kategori', 'Yeşil', 'Sarı', 'Kırmızı'

            GrafikAnahtarRiskGostergesiDto gr = new GrafikAnahtarRiskGostergesiDto();
            foreach (Grafik item in sonuc.Liste)
            {
                gr = new GrafikAnahtarRiskGostergesiDto();
                bool yeni = true;
                foreach (GrafikAnahtarRiskGostergesiDto gr2 in donenDeger)
                {
                    if (gr2.Aciklama == item.Aciklama)
                    {
                        gr = gr2;
                        yeni = false;
                        break;
                    }
                }

                gr.Aciklama = item.Aciklama;
                if (item.EkAciklama == "YESIL")
                    gr.Yesil += item.Deger1;
                else if (item.EkAciklama == "SARI")
                    gr.Sari += item.Deger1;
                else if (item.EkAciklama == "KIRMIZI")
                    gr.Kirmizi += item.Deger1;

                if (yeni)
                    donenDeger.Add(gr);
            }

            sonuc.Liste = donenDeger.Cast<object>().ToList();
            return Ok(sonuc);
        }


        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre süreç için gereken bilgiyi ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RiskeVerilenYanit(Grafik form)
        {
            Sonuc sonuc = await _service.RiskeVerilenYanitHazirlaAsync(_kullanan, form);

            foreach (Grafik item in sonuc.Liste)
            {
                if (item.Aciklama == "1")
                    item.Aciklama = "Transfer Et";
                else if (item.Aciklama == "2")
                    item.Aciklama = "Kabul Et";
                else if (item.Aciklama == "3")
                    item.Aciklama = "Reddet";
                else if (item.Aciklama == "4")
                    item.Aciklama = "Azalt";
            }

            return Ok(sonuc);
        }

        //[HttpPost]
        //public async Task<IActionResult> EylemDurumu(Grafik form)
        //{
        //    Sonuc sonuc = await _service.EylemDurumuHazirlaAsync(_kullanan, form);

        //    return Ok(sonuc);
        //}

        /// <summary>
        /// Kullanıcı ekrandan dil değişikliği yaptığında cookie ye dil ile iligli bilgileri yazan metod
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="returnUrl"></param>
        /// <returns>
        /// Parametre olarak gelen returnUrl adresine Redirect eder
        /// </returns>
        [HttpPost]
        public IActionResult CultureManagement(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                                        new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

            return LocalRedirect(returnUrl);

        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre risk paneli için gereken bilgiyi ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RiskYonetimPaneli(GrafikRiskYonetimi form)
        {
            Sonuc sonuc = await _service.RiskYonetimiHazirlaAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre performans izleme paneli için gereken bilgiyi ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> PerformansIzlemePaneli(Grafik form)
        {
            Sonuc sonuc = await _service.PerformansIzlemeHazirlaAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre KRITolerans grafiği için gereken bilgiyi ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> KRITolerans(Grafik form)
        {
            Sonuc sonuc = await _service.KRIToleransHazirlaAsync(_kullanan, form);

            var liste = new List<object>();

            dynamic kri = null;
            foreach (Grafik item in sonuc.Liste)
            {
                kri = new { Aciklama = "", Sari = 0, SariBilgi = "", Yesil = 0, YesilBilgi = "", Kirmizi = 0, KirmiziBilgi = "" };
                bool yeni = true;
                int sira = 0;
                foreach (dynamic kri2 in liste)
                {
                    if (kri2.Aciklama == item.Aciklama)
                    {
                        kri = kri2;
                        yeni = false;
                        break;
                    }
                    sira++;
                }

                var Sari = kri.Sari;
                var Yesil = kri.Yesil;
                var Kirmizi = kri.Kirmizi;

                if (item.EkAciklama == "SARI")
                    Sari += item.Deger1;
                else if (item.EkAciklama == "YESIL")
                    Yesil += item.Deger1;
                else if (item.EkAciklama == "KIRMIZI")
                    Kirmizi += item.Deger1;

                kri = new { item.Aciklama, Sari, SariBilgi = Sari > 0 ? Sari + "" : "", Yesil, YesilBilgi = Yesil > 0 ? Yesil + "" : "", Kirmizi, KirmiziBilgi = Kirmizi > 0 ? Kirmizi + "" : "" };

                if (yeni)
                    liste.Add(kri);
                else
                    liste[sira] = kri;

            }

            sonuc.Liste = liste;
            return Ok(sonuc);
        }


        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre KRIToleransPie grafiği için gereken bilgiyi ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> KRIToleransPie(Grafik form)
        {
            Sonuc sonuc = await _service.KRIToleransPieHazirlaAsync(_kullanan, form);

            var liste = new List<object>();

            dynamic kri = null;
            foreach (Grafik item in sonuc.Liste)
            {
                kri = new { Aciklama = "", Deger1 = 0 };
                bool yeni = true;
                int sira = 0;
                foreach (dynamic kri2 in liste)
                {
                    if (kri2.Aciklama == item.EkAciklama)
                    {
                        kri = kri2;
                        yeni = false;
                        break;
                    }
                    sira++;
                }

                kri = new { Aciklama = item.EkAciklama, Deger1 = kri.Deger1 + item.Deger1 };

                if (yeni)
                    liste.Add(kri);
                else
                    liste[sira] = kri;

            }

            sonuc.Liste = liste;
            return Ok(sonuc);
        }


        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre risk paneli için gereken bilgiyi ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> KRIToleransPaneli(GrafikRiskYonetimi form)
        {
            Sonuc sonuc = await _service.KRIToleransPaneliHazirlaAsync(_kullanan, form);

            return Ok(sonuc);
        }
    }
}
