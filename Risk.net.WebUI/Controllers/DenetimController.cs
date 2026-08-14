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
    /// Denetim işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class DenetimController : GenelController
    {
        /// <summary>
        /// IDenetimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.DenetimController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public DenetimController(IDenetimService service,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// IcDenetim View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI")]//Yetki Tamam
        public IActionResult IcDenetim()
        {
            if (Arac.YetkisiVarmi("ICDENETIMKOORDINATOR", _kullanan))
                ViewBag.SilmeYetki = true;
            else
                ViewBag.SilmeYetki = false;

            return View();
        }

        /// <summary>
        /// DisDenetim View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI")]//Yetki Tamam
        public IActionResult DisDenetim()
        {
            if (Arac.YetkisiVarmi("ICDENETIMKOORDINATOR", _kullanan))
                ViewBag.SilmeYetki = true;
            else
                ViewBag.SilmeYetki = false;

            return View();
        }

        /// <summary>
        /// DenetimListe View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "*")]//Yetki Tamam
        public IActionResult DenetimListe(string tur)
        {
            ViewBag.Gorevlendirme = 0;
            if (Arac.YetkisiVarmi("ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI", _kullanan))
                ViewBag.Gorevlendirme = 1;

            if (tur == "1")
                ViewBag.Baslik = _sharedResource["DenetimEylemPlani.SayfaBaslik"];
            else if (tur == "2")
            {
                if (!Arac.YetkisiVarmi("ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI", _kullanan))
                    return RedirectToAction("AccessDenied", "Account");

                ViewBag.Baslik = _sharedResource["DenetimIzlemeTakip.SayfaBaslik"];
            }
            else
                return RedirectToAction("AccessDenied", "Account");

            ViewBag.Tur = tur;

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
        public async Task<IActionResult> TabloDoldur(int denetimKaynak, int tur)
        {
            DataTablesParam dataTableInfo = new DataTablesParam(Request);

            if (tur == 1)
                dataTableInfo.pageName = "EylemPlani";
            else if (tur == 2)
                dataTableInfo.pageName = "IzlemeTakip";

            var jsonData = await _service.TabloDoldurAsync(_kullanan, denetimKaynak, dataTableInfo);

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

            if (sonuc.IslemSonuc)
            {
                bool kaydetYetki = false;
                Denetim kayit = (Denetim)sonuc.Nesne;
                foreach (DenetimDenetci item in kayit.Denetciler)
                {
                    if (item.DenetciKod == _kullanan.PersonelKod)
                    {
                        kaydetYetki = true;
                        break;
                    }
                }
                foreach (DenetimSorumlu item in kayit.Sorumlular)
                {
                    if (item.SorumluKod == _kullanan.PersonelKod)
                    {
                        kaydetYetki = true;
                        break;
                    }
                }

                if (Arac.YetkisiVarmi("ICDENETIMKOORDINATOR", _kullanan))
                    kaydetYetki = true;

                kayit.KaydetmeYetkisi = kaydetYetki;
            }

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
        public async Task<ActionResult> Kaydet(Denetim form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

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
        public async Task<IActionResult> Sil(string kod)
        {
            Denetim form = new Denetim();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın onaylama işlemini sağlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Onayla(string kod)
        {
            Denetim form = new Denetim();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Rapor No bilgisinin güncellenemesini sağlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> RaporNoKaydet(Denetim form)
        {
            Sonuc sonuc = await _service.RaporNoKaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Rapor No bilgisinin güncellenemesini sağlayan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Tamamla(Denetim form)
        {
            Sonuc sonuc = await _service.TamamlaAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Denetim seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer(string kaynak)
        {
            Denetim nesne = new Denetim();
            nesne.Kaynak = Arac.ConvertToInt(kaynak);
            var sonuc = await _service.ListeleAsync(_kullanan, nesne);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (Denetim item in sonuc.Liste)
            {
                string denetimAdi = item.Kaynak == 1 ? item.DenetimAdi : item.DenetimYapanKurum.Adi;

                donenDeger.Add(new SelectListesi { id = item.Kod, text = "[" + item.Yil + "/" + item.DenetimNo + "] " + denetimAdi });
            }

            return Ok(donenDeger);
        }
    }
}
