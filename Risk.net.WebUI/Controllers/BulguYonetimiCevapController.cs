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
    /// Bulgu Yönetimi Cevap iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class BulguYonetimiCevapController : GenelController
    {
        /// <summary>
        /// IBulguYonetimiCevapService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBulguYonetimiCevapService _service;
        /// <summary>
        /// IRiskEvreniService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniService _serviceRiskEvreni;
        /// <summary>
        /// IBulguYonetimiBirimService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBulguYonetimiBirimService _serviceBulguBirim;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.BulguYonetimiCevapController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceRiskEvreni"></param>
        /// <param name="serviceBulguBirim"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public BulguYonetimiCevapController(IBulguYonetimiCevapService service,
                                        IRiskEvreniService serviceRiskEvreni,
                                        IBulguYonetimiBirimService serviceBulguBirim,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceRiskEvreni = serviceRiskEvreni;
            _serviceBulguBirim = serviceBulguBirim;
        }

        /// <summary>
        /// BulguYonetimiCevap View sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        public IActionResult Index(int onay)
        {
            int onayMesaj = 0;
            if (Arac.YetkisiVarmi("ILKOORDINATOR,MERKEZKOORDINATOR,GENELKOORDINATOR,ICDENETIMKOORDINATOR", _kullanan))
                onayMesaj = 1;//Koordinator e gelen onay bilgisi bulguya cevap ekranýna girildiðinde de görsün ONAY düðmesini Melih 31.07.2023


            if (onay == 1)
            {
                ViewBag.Baslik = _sharedResource["BulguYonetimiCevapOnay.SayfaBaslik"];

                onayMesaj = 0;

                //UZMAN, BIRIM AMIRI kýsýtlamasý konabilir. 
                //if (!Arac.YetkisiVarmi("ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI", _kullanan))
                //    return RedirectToAction("AccessDenied", "Account");
            }

            ViewBag.Onay = onay;
            ViewBag.OnayMesaj = onayMesaj;

            return View();
        }

        /// <summary>
        /// Kullanýcýdan gelen kriterler ile ilgili kayýtlarýn sunucudan getirilmesini saðlayan metod
        /// </summary>
        /// <returns>
        /// Ok(JSON tipinde sunucudan gelen bilgi)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Listele(string bulguKod, int tur)
        {
            Sonuc sonucBulguBirim = await _serviceBulguBirim.ListeleAsync(_kullanan, bulguKod);
            string birimler = "";
            if (sonucBulguBirim.IslemSonuc)
            {
                foreach (BulguYonetimiBirim item in sonucBulguBirim.Liste)
                {
                    if (birimler != "") birimler += ";";
                    birimler += item.KoordinatorlukKod;
                }
            }

            BulguYonetimiCevap kriter = new BulguYonetimiCevap();
            kriter.BulguYonetimiKod = bulguKod;
            kriter.Tur = tur;

            Sonuc sonuc = await _service.ListeleAsync(_kullanan, kriter);
            if (sonuc.IslemSonuc)
                sonuc.Nesne = birimler;

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanýcý ekranýndan aldýðý sayfa no, kayýt sayýsý, sýralama alaný ve arama kriter 
        /// bilgileriyle ilgili servisten DataTables kontrolüne yüklemek üzere liste olarak getiren metod 
        /// </summary>
        /// <returns>
        /// Ok(JSON tipinde sunucudan gelen bilgi)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> TabloDoldur(int onay)
        {
            DataTablesParam dataTableInfo = new DataTablesParam(Request);

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo, onay == 1);

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
        public async Task<ActionResult> Kaydet(BulguYonetimiCevap form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

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
            var form = new BulguYonetimiCevap();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.OnayaGonderdi;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

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
        public async Task<IActionResult> Onayla(BulguYonetimiCevap form)
        {
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Bulgu paylaþým formu yazdýrma iþlemini yapan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        public async Task<FileContentResult> Yazdir(string kod, string ciktiTur)
        {
            Sonuc sonuc = await _service.KayitGetirAsync(_kullanan, kod);
            var form = (BulguYonetimiCevap)sonuc.Nesne;

            ITablo XLS = Arac.ExcelTablo();

            string sablonAd = "BulguPaylasimFormu.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);

            string birimler = "";
            foreach (var item in form.BulguYonetimi.Birimler)
            {
                if (birimler != "") birimler += ", ";
                var koordinatorluk = item.Koordinatorluk.Adi;
                var birim = item.Birim?.Adi;

                birimler += "* " + koordinatorluk;
                if (!string.IsNullOrWhiteSpace(birim))
                    birimler += "-" + birim;
            }

            XLS.HucreAdDegerYaz("DenetimAdi", form?.BulguYonetimi?.Denetim?.DenetimAdi);
            XLS.HucreAdDegerYaz("BulguNo", form?.BulguYonetimi?.BulguNo);
            XLS.HucreAdDegerYaz("OnemDuzeyi", form?.BulguYonetimi?.OnemDuzeyi?.Adi);
            XLS.HucreAdDegerYaz("BulguTanimi", form?.BulguYonetimi?.BulguTanimi);
            XLS.HucreAdDegerYaz("BulguBirimler", birimler);
            XLS.HucreAdDegerYaz("Bulgular", form?.BulguYonetimi?.BulguAciklama);
            XLS.HucreAdDegerYaz("Nedenler", form?.BulguYonetimi?.Nedenler);
            XLS.HucreAdDegerYaz("Riskler", form?.BulguYonetimi?.Riskler);
            XLS.HucreAdDegerYaz("Kriterler", form?.BulguYonetimi?.Kriterler);
            XLS.HucreAdDegerYaz("Oneriler", form?.BulguYonetimi?.Oneriler);

            XLS.HucreAdDegerYaz("BulguGorusu", form?.BulguGorusu?.Adi);
            XLS.HucreAdDegerYaz("OneriGorusu", form?.OneriGorusu?.Adi);
            XLS.HucreAdDegerYaz("OnemDuzeyiGorusu", form?.OnemDuzeyiGorusu?.Adi);

            XLS.HucreAdDegerYaz("NedenBulguKatilmiyor", form?.NedenBulguKatilmiyor);
            XLS.HucreAdDegerYaz("NedenOneriKatilmiyor", form?.NedenOneriKatilmiyor);
            XLS.HucreAdDegerYaz("NedenOnemKatilmiyor", form?.NedenOnemKatilmiyor);

            XLS.HucreAdDegerYaz("Aciklama", form?.Aciklama);
            XLS.HucreAdDegerYaz("Sorumlusu", form?.Sorumlusu);
            XLS.HucreAdDegerYaz("Eylem", form?.Eylem);
            XLS.HucreAdDegerYaz("TamamlamaTarihi", Arac.DateTimeToDDMMYYYY(form?.TamamlamaTarihi));

            XLS.HucreAdDegerYaz("Tarih", Arac.DateTimeToDDMMYYYY(System.DateTime.Now));
            XLS.HucreAdDegerYaz("Duzenleyen", _kullanan.AdiSoyadi);

            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "XLSX";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "BulguPaylasimFormu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }
    }
}
