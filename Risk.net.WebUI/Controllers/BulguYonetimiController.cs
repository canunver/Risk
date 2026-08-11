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
    /// Bulgu Yönetimi iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    public class BulguYonetimiController : GenelController
    {
        /// <summary>
        /// IBulguYonetimiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBulguYonetimiService _service;
        /// <summary>
        /// IDenetimService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimService _serviceDenetim;
        /// <summary>
        /// IBulguYonetimiCevapService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBulguYonetimiCevapService _serviceCevap;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.BulguYonetimiController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceCevap"></param>
        /// <param name="serviceDenetim"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public BulguYonetimiController(IBulguYonetimiService service,
                                        IBulguYonetimiCevapService serviceCevap,
                                        IDenetimService serviceDenetim,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceDenetim = serviceDenetim;
            _serviceCevap = serviceCevap;
        }

        /// <summary>
        /// BulguYonetimi View sayfasý açýldýðýnda çalýþan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayý görme yetkisi olan kullanýcýlar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI")]//Yetki Tamam
        [Route("/BulguYonetimi/{denetimKodu}")]
        public async Task<IActionResult> Index(string denetimKodu)
        {
            if (string.IsNullOrWhiteSpace(denetimKodu)) return RedirectToAction("AccessDenied", "Account");

            Sonuc sonuc = await _serviceDenetim.KayitGetirAsync(_kullanan, denetimKodu);

            string denetimAdi = "";
            string kaynakAdi = "";
            string denetimYapanKurumAdi = "";
            int yil = 0;
            int kaynak = 0;
            List<SelectListesi> koordinatorlukListe = new List<SelectListesi>();
            if (sonuc.IslemSonuc)
            {
                Denetim denetim = (Denetim)sonuc.Nesne;
                denetimAdi = denetim.DenetimAdi;
                denetimYapanKurumAdi = denetim.DenetimYapanKurum?.Adi;
                yil = denetim.Yil;
                kaynak = denetim.Kaynak;
                if (denetim.Kaynak == 1)
                    kaynakAdi = "Ýç Denetim";
                else
                    kaynakAdi = "Dýþ Denetim";

                foreach (var item in denetim.Birimler)
                {
                    koordinatorlukListe.Add(new SelectListesi { id = item.Koordinatorluk.Kod, text = item.Koordinatorluk.Adi });
                }
            }
            else
                return RedirectToAction("AccessDenied", "Account");


            //BUlgu deðiþtirme yetkisi vr mý?
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
            {
                kaydetYetki = true;
                ViewBag.SilmeYetki = true;
            }
            else
            {
                ViewBag.SilmeYetki = false;
            }

            ViewBag.KayitYetki = kaydetYetki;

            ViewBag.DenetimAdi = denetimAdi;
            ViewBag.DenetimKodu = denetimKodu;
            ViewBag.KaynakAdi = kaynakAdi;

            ViewBag.Yil = yil;
            ViewBag.Kaynak = kaynak;
            ViewBag.DenetimYapanKurumAdi = denetimYapanKurumAdi;
            ViewBag.KoordinatorlukListe = koordinatorlukListe;

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
        [Route("/BulguYonetimi/TabloDoldur/{denetimKodu}")]
        public async Task<IActionResult> TabloDoldur(string denetimKodu)
        {
            DataTablesParam dataTableInfo = new DataTablesParam(Request);

            BulguYonetimi kriter = new BulguYonetimi();
            kriter.DenetimKod = denetimKodu;

            var jsonData = await _service.TabloDoldurAsync(_kullanan, kriter, dataTableInfo);

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
        [Route("/BulguYonetimi/KayitGetir")]
        public async Task<IActionResult> KayitGetir(string kod)
        {
            Sonuc sonuc = await _service.KayitGetirAsync(_kullanan, kod);

            Sonuc sonucCevaplar = await _serviceCevap.ListeleAsync(_kullanan, new BulguYonetimiCevap() { BulguYonetimiKod = kod });
            if (sonucCevaplar.IslemSonuc)
            {
                if (sonucCevaplar.IslemSonuc && sonucCevaplar.Liste.Count > 0)
                {
                    ((BulguYonetimi)sonuc.Nesne).Cevaplar = new List<BulguYonetimiCevap>();
                    foreach (BulguYonetimiCevap item in sonucCevaplar.Liste)
                    {
                        ((BulguYonetimi)sonuc.Nesne).Cevaplar.Add(item);
                    }
                }
            }

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
        [Route("/BulguYonetimi/Kaydet")]
        public async Task<ActionResult> Kaydet(BulguYonetimi form)
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
        [Route("/BulguYonetimi/Sil")]
        public async Task<IActionResult> Sil(string kod)
        {
            BulguYonetimi form = new BulguYonetimi();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await DurumDegistir(form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn onaylama iþlemini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> Onayla(string kod)
        {
            BulguYonetimi form = new BulguYonetimi();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await DurumDegistir(form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn durumunun deðiþtirilmesini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        [Route("/BulguYonetimi/DurumDegistir")]
        public async Task<Sonuc> DurumDegistir(BulguYonetimi form)
        {
            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return sonuc;
        }

        /// <summary>
        /// Listeden seçilen kaydýn Ýzleme Takip bilgilerini deðiþtiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        [Route("/BulguYonetimi/IzlemeTakipKaydet")]
        public async Task<Sonuc> IzlemeTakipKaydet(BulguYonetimi form)
        {
            Sonuc sonuc = await _service.IzlemeTakipKaydetAsync(_kullanan, form);

            return sonuc;
        }

        /// <summary>
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="nesne"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        [Route("/BulguYonetimi/KisiyeGonderKaydet")]
        public async Task<ActionResult> KisiyeGonderKaydet(List<BulguYonetimiCevap> nesne)
        {
            Sonuc sonuc = new Sonuc();
            List<string> personelListe = new List<string>();

            if (nesne == null || nesne?.Count == 0) return Ok(new Sonuc(ENUMIslemDurum.Uyari, "Eklenecek personel yok"));

            string bulguKod = "";
            foreach (BulguYonetimiCevap item in nesne)
            {
                bulguKod = item.BulguYonetimiKod;
                if (string.IsNullOrWhiteSpace(bulguKod) || string.IsNullOrWhiteSpace(item.IlgiliPersonelKod)) continue;

                sonuc = await _serviceCevap.KaydetAsync(_kullanan, item);

                if (sonuc.IslemSonuc)
                    personelListe.Add(item.IlgiliPersonelKod);
            }

            if (string.IsNullOrWhiteSpace(bulguKod) || personelListe.Count == 0) return Ok(new Sonuc(ENUMIslemDurum.Uyari, "Bulgu Kod bilgisi olmadýðý için iþlem yapýlamadý"));

            sonuc = await _serviceCevap.BilgiEPostaGonderAsync(_kullanan, bulguKod, personelListe);

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
        [Route("/BulguYonetimi/KisiyeGonderSil")]
        public async Task<ActionResult> KisiyeGonderSil(string kod, string bulguKod)
        {
            BulguYonetimiCevap nesne = new BulguYonetimiCevap();
            nesne.BulguYonetimiKod = bulguKod;
            nesne.Kod = kod;

            Sonuc sonuc = await _serviceCevap.SilAsync(_kullanan, nesne);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanýcý tarafýndan girilen nihai görüþ bilgilerini ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        [Route("/BulguYonetimi/NihaiGorusKaydet")]
        public async Task<ActionResult> NihaiGorusKaydet(BulguYonetimi form)
        {
            Sonuc sonuc = await _service.NihaiGorusKaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

    }
}
