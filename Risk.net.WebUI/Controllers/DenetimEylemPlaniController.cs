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
    /// Denetim Eylem Plani işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class DenetimEylemPlaniController : GenelController
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
        /// <see cref="Risk.net.WebUI.Controllers.DenetimEylemPlaniController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceDenetim"></param>
        /// <param name="serviceCevap"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public DenetimEylemPlaniController(IBulguYonetimiService service,
                                        IDenetimService serviceDenetim,
                                        IBulguYonetimiCevapService serviceCevap,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceDenetim = serviceDenetim;
            _serviceCevap = serviceCevap;
        }

        /// <summary>
        /// DenetimEylemPlani View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "*")]//Yetki Tamam
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
            if (Arac.YetkisiVarmi("ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI", _kullanan))
            {
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
            dataTableInfo.pageName = "EylemPlani";

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
                if (gorev.Tip == 1) gorevlendirme = gorev;
            }

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "EylemPlani.xltx";
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
            XLS.HucreAdDegerYaz("DenetimGozetmeni", gozetmen);
            XLS.HucreAdDegerYaz("DenetimGorevlendirme", Arac.DateTimeToDDMMYYYY(gorevlendirme.GorevYaziTarihi) + "/" + gorevlendirme.GorevYaziSayisi);

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

                XLS.HucreDegerYaz(satir, 0, item.BulguYonetimi.BulguNo);
                XLS.HucreDegerYaz(satir, 1, item.BulguYonetimi.BulguTanimi);
                XLS.HucreDegerYaz(satir, 2, item.BulguYonetimi.Oneriler);
                XLS.HucreDegerYaz(satir, 3, item.Sorumlusu);
                XLS.HucreDegerYaz(satir, 4, Arac.DateTimeToDDMMYYYY(item.TamamlamaTarihi));
                XLS.HucreDegerYaz(satir, 5, item.Eylem);
                XLS.HucreDegerYaz(satir, 6, item.BulguYonetimi.NihaiGorus);

                satir++;
            }

            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "XLSX";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "EylemPlani" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }
    }
}
