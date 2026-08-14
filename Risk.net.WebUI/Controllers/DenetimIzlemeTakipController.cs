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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Denetim İzleme Takip işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class DenetimIzlemeTakipController : GenelController
    {
        /// <summary>
        /// IBulguYonetimiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBulguYonetimiService _service;
        /// <summary>
        /// IDenetimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimService _serviceDenetim;
        /// <summary>
        /// IBulguYonetimiCevapService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBulguYonetimiCevapService _serviceCevap;
        /// <summary>
        /// IDosyaService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDosyaService _serviceDosya;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.DenetimIzlemeTakipController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceDenetim"></param>
        /// <param name="serviceCevap"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public DenetimIzlemeTakipController(IBulguYonetimiService service,
                                        IDenetimService serviceDenetim,
                                        IBulguYonetimiCevapService serviceCevap,
                                        IDosyaService serviceDosya,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceDenetim = serviceDenetim;
            _serviceCevap = serviceCevap;
            _serviceDosya = serviceDosya;
        }

        /// <summary>
        /// DenetimIzlemeTakip View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI")]//Yetki Tamam
        public async Task<IActionResult> Index(string denetimKodu)
        {
            if (string.IsNullOrWhiteSpace(denetimKodu)) return RedirectToAction("AccessDenied", "Account");

            Sonuc sonuc = await _serviceDenetim.KayitGetirAsync(_kullanan, denetimKodu);

            string denetimAdi = "";
            string denetimYapanKurumAdi = "";
            int yil = 0;
            if (sonuc.IslemSonuc)
            {
                Denetim denetim = (Denetim)sonuc.Nesne;
                denetimAdi = denetim.DenetimAdi;
                denetimYapanKurumAdi = denetim.DenetimYapanKurum?.Adi;
                yil = denetim.Yil;
            }
            else
                return RedirectToAction("AccessDenied", "Account");

            //BUlgu değiştirme yetkisi vr mı?
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
            }

            ViewBag.KayitYetki = kaydetYetki;

            ViewBag.Yil = yil;
            ViewBag.DenetimYapanKurumAdi = denetimYapanKurumAdi;
            ViewBag.DenetimAdi = denetimAdi;
            ViewBag.DenetimKodu = denetimKodu;

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
        public async Task<IActionResult> TabloDoldur(string denetimKodu)
        {
            DataTablesParam dataTableInfo = new DataTablesParam(Request);
            dataTableInfo.pageName = "IzlemeTakip";

            BulguYonetimi kriter = new BulguYonetimi();
            kriter.DenetimKod = denetimKodu;

            var jsonData = await _service.TabloDoldurAsync(_kullanan, kriter, dataTableInfo);

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
                BulguYonetimiCevap kriter = new BulguYonetimiCevap();
                kriter.BulguYonetimiKod = kod;

                Sonuc sonucCevaplar = await _serviceCevap.ListeleAsync(_kullanan, kriter);

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
        /// Eylem Planı yazdırma işlemini yapan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        public async Task<FileContentResult> Yazdir(string denetimKod, string ciktiTur)
        {
            Sonuc sonuc = await _serviceDenetim.KayitGetirAsync(_kullanan, denetimKod);
            Denetim denetim = (Denetim)sonuc.Nesne;
            DenetimGorevlendirme gorevlendirme = new DenetimGorevlendirme();
            foreach (DenetimGorevlendirme gorev in denetim.Gorevlendirme)
            {
                if (gorev.Tip == 2) gorevlendirme = gorev;
            }

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "BulguTakipFormu.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);
            kaynakSatir = satir;

            string birimler = "";
            foreach (var item in denetim.Birimler)
            {
                if (birimler != "") birimler += ", ";
                var koordinatorluk = item.Koordinatorluk.Adi;
                var birim = item.Birim?.Adi;

                birimler += "* " + koordinatorluk;
                if (!string.IsNullOrWhiteSpace(birim))
                    birimler += "-" + birim;
            }
            string denetciler = "";
            foreach (var item in denetim.Denetciler)
            {
                if (denetciler != "") denetciler += ", ";
                denetciler += item.Denetci.AdiSoyadi;
            }
            string gozetmen = "";
            foreach (var item in denetim.Sorumlular)
            {
                if (gozetmen != "") gozetmen += ", ";
                gozetmen += item.Sorumlu.AdiSoyadi;
            }

            XLS.HucreAdDegerYaz("DenetimAdi", denetim.DenetimAdi);
            XLS.HucreAdDegerYaz("DenetlenenBirim", birimler);
            XLS.HucreAdDegerYaz("Denetciler", denetciler);
            //XLS.HucreAdDegerYaz("DenetimGozetmeni", gozetmen);

            XLS.HucreAdDegerYaz("DenetimGorevlendirmeTarihSayi", Arac.DateTimeToDDMMYYYY(gorevlendirme.GorevYaziTarihi) +"/"+gorevlendirme.GorevYaziSayisi);
            XLS.HucreAdDegerYaz("NihaiDenetimRaporTarihSayi",  Arac.DateTimeToDDMMYYYY(gorevlendirme.RaporTarihi) + "/" + gorevlendirme.RaporSayisi);
            XLS.HucreAdDegerYaz("NihaiDenetimRaporOnayTarihSayi", Arac.DateTimeToDDMMYYYY(gorevlendirme.RaporOnayTarihi) + "/" + gorevlendirme.RaporOnaySayisi);
            XLS.HucreAdDegerYaz("NihaiDenetimRaporDenetleyenTarihSayi", Arac.DateTimeToDDMMYYYY(gorevlendirme.GondermeTarihi) + "/" + gorevlendirme.GondermeSayisi);


            XLS.HucreAdDegerYaz("Tarih", Arac.DateTimeToDDMMYYYY(System.DateTime.Now));
            XLS.HucreAdDegerYaz("Duzenleyen", _kullanan.AdiSoyadi);

            BulguYonetimiCevap kriter = new BulguYonetimiCevap();
            kriter.SorguDenetimKod = denetimKod;
            Sonuc sonucCevap = await _serviceCevap.ListeleAsync(_kullanan, kriter);

            foreach (BulguYonetimiCevap item in sonucCevap.Liste)
            {
                if (kaynakSatir != satir)
                {
                    XLS.SatirAc(satir, 1);
                    XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 10, satir, 0);
                }

                string kanitBelgesi = "";

                var sonucDosya = await _serviceDosya.ListeleAsync(_kullanan, item.Kod);
                if (sonucDosya.IslemSonuc && sonucDosya.Liste.Count > 0)
                {
                    foreach (Dosya dosya in sonucDosya.Liste)
                    {
                        if (kanitBelgesi != "") kanitBelgesi += ", ";
                        kanitBelgesi += dosya.Adi;
                        if (!string.IsNullOrWhiteSpace(dosya.Aciklama))
                            kanitBelgesi += " / " + dosya.Aciklama;
                    }
                }

                XLS.HucreDegerYaz(satir, 0, item.BulguYonetimi.BulguNo);
                XLS.HucreDegerYaz(satir, 1, item.BulguYonetimi.BulguTanimi);
                XLS.HucreDegerYaz(satir, 2, item.Eylem);
                XLS.HucreDegerYaz(satir, 3, item.Sorumlusu);
                XLS.HucreDegerYaz(satir, 4, Arac.DateTimeToDDMMYYYY(item.TamamlamaTarihi));
                XLS.HucreDegerYaz(satir, 5, Arac.DateTimeToDDMMYYYY(item.BulguYonetimi.DenetimTarihi));
                XLS.HucreDegerYaz(satir, 6, item.BulguYonetimi.EylemKarsilama == 1 ? "Evet" : "Hayır");
                XLS.HucreDegerYaz(satir, 7, kanitBelgesi);
                XLS.HucreDegerYaz(satir, 8, _sharedResource["BulguYonetimi.Durum." + item.BulguYonetimi.Durum.ToString()]);

                satir++;
            }

            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "XLSX";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "IzlemeTakip" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }
    }
}
