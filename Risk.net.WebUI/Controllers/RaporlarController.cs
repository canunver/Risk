using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.IO;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Rapor işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class RaporlarController : GenelController
    {
        /// <summary>
        /// IRaporlamaService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRaporlamaService _service;

        /// <summary>
        /// ITanimGenelService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITanimGenelService _serviceTanimGenel;

        /// <summary>
        /// ITanimIlIrtibatOfisiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITanimIlIrtibatOfisiService _serviceIlIrtibatOfisi;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.RaporlarController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RaporlarController(IRaporlamaService service,
                                ITanimGenelService serviceTanimGenel,
                                ITanimIlIrtibatOfisiService serviceIlIrtibatOfisi,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceIlIrtibatOfisi = serviceIlIrtibatOfisi;
            _serviceTanimGenel = serviceTanimGenel;
        }

        /// <summary>
        /// Raporlama View sayfası açıldığında çalışan metod
        /// </summary>
        public IActionResult Index()
        {
            ViewBag.RaporListe = JsonConvert.SerializeObject(RaporListe());
            ViewBag.Yil = DateTime.Now.Year;

            return View();
        }

        /// <summary>
        /// Rapor isimlerinin getirilmesini sağlayan metod
        /// </summary>
        /// <returns>
        /// List<object> tipinde değer döndürür
        /// </returns>
        public List<object> RaporListe()
        {
            List<object> raporlar = new List<object>();

            //if (Arac.YetkisiVarmi("BASKAN", _kullanan))
            //else if (Arac.YetkisiVarmi("GENELKOORDINATOR,MERKEZKOORDINATOR,ILKOORDINATOR,ICDENETIMKOORDINATOR", _kullanan))
            //else if (Arac.YetkisiVarmi("RISKSEKRETARYASI,YETKILIRISKGOREVLISI,PLANLAMAUNITESI", _kullanan))
            raporlar.Add(new { kod = "RPT001", adi = "" + _sharedResource["Raporlar.RPT001"] + "", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI,GENELKOORDINATOR,BASKAN" });    //Üst Yönetim Risk Raporu
            raporlar.Add(new { kod = "RPT002", adi = "" + _sharedResource["Raporlar.RPT002"] + "", Yetki = "MERKEZKOORDINATOR,ILKOORDINATOR,ICDENETIMKOORDINATOR,BIRIMAMIRI" });  //Koordinatörler Risk Raporu
            raporlar.Add(new { kod = "RPT003", adi = "" + _sharedResource["Raporlar.RPT003"] + "", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI" });    //"Risk Sekretaryası Risk Raporu"
            raporlar.Add(new { kod = "RPT004", adi = "" + _sharedResource["Raporlar.RPT004"] + "", Yetki = "*" });    //"Risk Sahipleri Risk Raporu"
            raporlar.Add(new { kod = "RPT005", adi = "" + _sharedResource["Raporlar.RPT005"] + "", Yetki = "*" });    //"Trend Raporu"
            raporlar.Add(new { kod = "RPT006", adi = "" + _sharedResource["Raporlar.RPT006"] + "", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI" }); //"İç Kontrol Zayıflıkları Raporu"
            raporlar.Add(new { kod = "RPT007", adi = "" + _sharedResource["Raporlar.RPT007"] + "", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI" }); //"Yıllık Risk Planı (Özet Risk Kütüğü)"
            raporlar.Add(new { kod = "RPT008", adi = "" + _sharedResource["Raporlar.RPT008"] + "", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI" }); //"Yarı Yıl Risk Azaltma Planı"
            raporlar.Add(new { kod = "RPT009", adi = "" + _sharedResource["Raporlar.RPT009"] + "", Yetki = "GENELKOORDINATOR,BASKAN" });  //"Üst Yönetim Stratejik Planlama Raporu"
            raporlar.Add(new { kod = "RPT010", adi = "" + _sharedResource["Raporlar.RPT010"] + "", Yetki = "MERKEZKOORDINATOR,ILKOORDINATOR" }); //"Koordinatörler Stratejik Planlama Raporu"
            raporlar.Add(new { kod = "RPT011", adi = "" + _sharedResource["Raporlar.RPT011"] + "", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI,ICDENETIMKOORDINATOR,ICDENETIMUZMANI" }); //"Süreç/Alt Süreç Raporu"
            raporlar.Add(new { kod = "RPT013", adi = "" + _sharedResource["Raporlar.RPT013"] + "", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI,ICDENETIMKOORDINATOR,ICDENETIMUZMANI" }); //"Anahtar Risk Göstergesi Raporu"
            raporlar.Add(new { kod = "RPT014", adi = "" + _sharedResource["Raporlar.RPT014"] + "", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI,ICDENETIMKOORDINATOR,ICDENETIMUZMANI" }); //"Olay Raporu"
            //raporlar.Add(new { kod = "RPT015", adi = "Yıllık Risk Yönetimi Beyannamesi", Yetki = "RISKSEKRETARYASI,YETKILIRISKGOREVLISI" });


            List<object> gorulecekRaporlar = new List<object>();
            dynamic raporListe = raporlar;

            foreach (var item in raporListe)
            {
                if (Arac.YetkisiVarmi(item.Yetki, _kullanan))
                    gorulecekRaporlar.Add(item);
            }

            return gorulecekRaporlar;
        }

        /// <summary>
        /// Ekranlardaki Donem seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns>
        /// Ok(Sonuc tipinde sunucudan gelen bilgi)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerDonem()
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            donenDeger.Add(new SelectListesi { id = "1", text = "1. " + _sharedResource["Raporlar.Alan.Donem"] });
            donenDeger.Add(new SelectListesi { id = "2", text = "2. " + _sharedResource["Raporlar.Alan.Donem"] });

            return Ok(donenDeger);
        }

        /// <summary>
        /// Ekranlardaki İl İrtibat Ofislerinin seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns>
        /// Ok(Sonuc tipinde sunucudan gelen bilgi)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerIlIrtibatOfisi()
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            donenDeger.Add(new SelectListesi { id = "-1", text = "Tümü" });

            var sonuc = await _serviceIlIrtibatOfisi.ListeleAsync(_kullanan, (int)ENUMDurum.Onayli);
            foreach (TanimIlIrtibatOfisi item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Adi });
            }

            //var sonuc = await _serviceTanimGenel.ListeleAsync(_kullanan, "ILIRTIBATOFISI");
            //foreach (TanimGenel item in sonuc.Liste)
            //{
            //    donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Adi });
            //}

            return Ok(donenDeger);
        }

        /// <summary>
        /// Listeden seçilen raporu ilgili servis aracılığıyla getiren metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> RaporHazirla(string form)
        {
            FileContentResult sonuc;
            try
            {
                dynamic gelenKriter = JObject.Parse(form);

                string kod = gelenKriter.Kod.Value;

                if (kod == "RPT001")
                    sonuc = await UstYonetimRiskRaporu(gelenKriter);
                else if (kod == "RPT002")
                    sonuc = await KoordinatorlerRiskRaporu(gelenKriter);
                else if (kod == "RPT003")
                    sonuc = await RiskSekretaryasiRiskRaporu(gelenKriter);
                else if (kod == "RPT004")
                    sonuc = await RiskSahipleriRiskRaporu(gelenKriter);
                else if (kod == "RPT005")
                    sonuc = await TrendRaporu(gelenKriter);
                else if (kod == "RPT006")
                    sonuc = await IcKontrolZayifliklariRaporu(gelenKriter);
                else if (kod == "RPT007")
                    sonuc = await YillikRiskPlaniRaporu(gelenKriter);
                else if (kod == "RPT008")
                    sonuc = await YariYilRiskAzaltmaPlaniRaporu(gelenKriter);
                else if (kod == "RPT009")
                    sonuc = await UstYonetimStratejikPlanlamaRaporu(gelenKriter);
                else if (kod == "RPT010")
                    sonuc = await KoordinatorlerStratejikPlanlamaRaporu(gelenKriter);
                else if (kod == "RPT011")
                    sonuc = await SurecAltSurecRaporu(gelenKriter);
                else if (kod == "RPT012")
                    sonuc = await RiskIzlemeRaporu(gelenKriter);
                else if (kod == "RPT013")
                    sonuc = await AnahtarRiskGostergesiRaporu(gelenKriter);
                else if (kod == "RPT014")
                    sonuc = await OlayRaporu(gelenKriter);
                else if (kod == "RPT015")
                    sonuc = await RiskYonetimiBeyannamesiRaporu(gelenKriter);
                else
                    return NoContent();

                //sonuc = TestBirMilyon(gelenKriter);
            }
            catch (Exception e)
            {
                Arac.HataStrYaz("Rapor oluşturulurken hata meydana geldi:" + e.Message + "\nRapor parametresi:" + form);
                return NoContent();
            }

            return sonuc;
        }

        //public FileContentResult TestBirMilyon(dynamic gelenKriter)
        //{
        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();

        //    ITablo XLS = Arac.ExcelTablo();
        //    string sonucDosyaAdi = System.IO.Path.GetTempFileName();
        //    XLS.BosDosyaAc(sonucDosyaAdi);

        //    for (int i = 0; i < 100; i++)
        //    {
        //        //for (int j = 0; j < 5; j++)
        //        {
        //            XLS.HucreDegerYaz(i, 1, i * 1);
        //        }
        //    }

        //    XLS.DosyaSaklaTamYol();

        //    double sure = stopWatch.ElapsedMilliseconds / 1000;
        //    Arac.HataStrYaz("Bitti:" + sure + " saniye");

        //    return Arac.DosyaGonder(sonucDosyaAdi, "RaporAdi.xlsx", true);
        //}

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> UstYonetimRiskRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.UstYonetimRiskHazirlaAsync(_kullanan, new RaporUstYonetimRisk()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                Donem = gelenKriter.Donem,
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "UstYonetimRisk.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int siraNo = 1;
            foreach (RaporUstYonetimRisk item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                string artikRiskSeviyesi = "Yüksek";
                if (item.ArtikRiskSeviyesi == 5)
                    artikRiskSeviyesi = "Çok Yüksek";

                var sira = 0;

                //XLS.HucreDegerYaz(satir, 0, item.Kod);
                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.Yil));
                XLS.HucreDegerYaz(satir, sira++, item.Donem > 0 ? item.Donem + ". " + _sharedResource["Raporlar.Alan.Donem"] : "");
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskNumarasi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.IlIrtibatOfisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskTanimi);
                XLS.HucreDegerYaz(satir, sira++, artikRiskSeviyesi);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlani);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesiDonem);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "UstYonetimRiskRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> KoordinatorlerRiskRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.KoordinatorlerRiskHazirlaAsync(_kullanan, new RaporKoordinatorlerRisk()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "KoordinatorlerRisk.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int ArtikRiskSeviyesi = 0;

            int siraNo = 1;
            foreach (RaporKoordinatorlerRisk item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                var sira = 0;

                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.IlIrtibatOfisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.Hedef);
                XLS.HucreDegerYaz(satir, sira++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskTanimi);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesi);
                XLS.HucreDegerYaz(satir, sira++, item.MevcutDeger.GetValueOrDefault());
                XLS.HucreDegerYaz(satir, sira++, YapisalRiskSeviyesiGetir(item.Etki, item.Olasilik, item.EtkiOlasilikMatrisi));
                XLS.HucreDegerYaz(satir, sira++, Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref ArtikRiskSeviyesi));
                XLS.HucreDegerYaz(satir, sira++, RiskeVerilecekCevapAdGetir(item.RiskeVerilecekCevap));
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlani);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniSorumlusu);
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                XLS.HucreDegerYaz(satir, sira++, RiskAzaltmaPlaniMevcutDurumAdGetir(item.MevcutDurum));
                XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlani);
                XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlaniNedeni);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniNotlar);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "KoordinatorlerRiskRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> RiskSekretaryasiRiskRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.RiskSekretaryasiRiskHazirlaAsync(_kullanan, new RaporRiskSekretaryasiRisk()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "RiskSekretaryasiRisk.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int ArtikRiskSeviyesi = 0;

            int siraNo = 1;
            foreach (RaporRiskSekretaryasiRisk item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                var sira = 0;

                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, item.Yil);
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.IlIrtibatOfisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.StratejikPlanDonemAdi);
                XLS.HucreDegerYaz(satir, sira++, item.Amac);
                XLS.HucreDegerYaz(satir, sira++, item.Hedef);
                XLS.HucreDegerYaz(satir, sira++, item.Surec);
                XLS.HucreDegerYaz(satir, sira++, item.AltSurec);
                XLS.HucreDegerYaz(satir, sira++, item.RiskNo);
                XLS.HucreDegerYaz(satir, sira++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskTanimi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskSahibi);
                XLS.HucreDegerYaz(satir, sira++, RiskTuruAdGetir(item.TehditFirsat));
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesi);
                XLS.HucreDegerYaz(satir, sira++, item.MevcutDeger.GetValueOrDefault());
                XLS.HucreDegerYaz(satir, sira++, item.YapisalRiskPuani);
                XLS.HucreDegerYaz(satir, sira++, YapisalRiskSeviyesiGetir(item.Etki, item.Olasilik, item.EtkiOlasilikMatrisi));
                XLS.HucreDegerYaz(satir, sira++, item.ArtikRiskPuani);
                XLS.HucreDegerYaz(satir, sira++, Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref ArtikRiskSeviyesi));
                XLS.HucreDegerYaz(satir, sira++, RiskeVerilecekCevapAdGetir(item.RiskeVerilecekCevap));
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlani);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniSorumlusu);
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                XLS.HucreDegerYaz(satir, sira++, RiskAzaltmaPlaniMevcutDurumAdGetir(item.MevcutDurum));
                XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlani);
                XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlaniNedeni);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniNotlar);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "RiskSekretaryasiRiskRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> RiskSahipleriRiskRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.RiskSahipleriRiskHazirlaAsync(_kullanan, new RaporRiskSahipleriRisk()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "RiskSahipleriRisk.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int ArtikRiskSeviyesi = 0;

            int siraNo = 1;
            foreach (RaporRiskSahipleriRisk item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 40, satir, 0);

                var sira = 0;

                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.Yil));
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.IlIrtibatOfisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.StratejikPlanDonemAdi);
                XLS.HucreDegerYaz(satir, sira++, item.Amac);
                XLS.HucreDegerYaz(satir, sira++, item.Hedef);
                XLS.HucreDegerYaz(satir, sira++, item.Surec);
                XLS.HucreDegerYaz(satir, sira++, item.AltSurec);
                XLS.HucreDegerYaz(satir, sira++, item.RiskNo);
                XLS.HucreDegerYaz(satir, sira++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskTanimi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskSahibi);
                XLS.HucreDegerYaz(satir, sira++, RiskTuruAdGetir(item.TehditFirsat));
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesi);
                XLS.HucreDegerYaz(satir, sira++, item.MevcutDeger.GetValueOrDefault());
                XLS.HucreDegerYaz(satir, sira++, YapisalRiskSeviyesiGetir(item.Etki, item.Olasilik, item.EtkiOlasilikMatrisi));
                XLS.HucreDegerYaz(satir, sira++, Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref ArtikRiskSeviyesi));
                XLS.HucreDegerYaz(satir, sira++, RiskeVerilecekCevapAdGetir(item.RiskeVerilecekCevap));
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlani);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniSorumlusu);
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                XLS.HucreDegerYaz(satir, sira++, RiskAzaltmaPlaniMevcutDurumAdGetir(item.MevcutDurum));
                XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlani);
                XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlaniNedeni);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniNotlar);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "RiskSahipleriRiskRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> TrendRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.TrendHazirlaAsync(_kullanan, new RaporTrend()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "Trend.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int siraNo = 1;
            foreach (RaporTrend item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int sira = 0;
                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.Yil));
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskNo);
                XLS.HucreDegerYaz(satir, sira++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskTanimi);
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.ArtikRiskPuaniOncekiDeger));
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.ArtikRiskPuaniGuncelDeger));
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.ArtikRiskSeviyesiGirisTarihi));
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.GuncellemeTarihi));
                XLS.HucreDegerYaz(satir, sira++, item.TrendPuan);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesiDurum);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesiTrend);

                XLS.ArkaPlanRenk(satir, 7, ArtikRiskSeviyesiRenkGetir(Arac.DegerAl(item.ArtikRiskSeviyesiOncekiDeger)));
                XLS.ArkaPlanRenk(satir, 8, ArtikRiskSeviyesiRenkGetir(Arac.DegerAl(item.ArtikRiskSeviyesiGuncelDeger)));
                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "TrendRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> IcKontrolZayifliklariRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.IcKontrolZayifliklariHazirlaAsync(_kullanan, new RaporIcKontrolZayifliklari()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "IcKontrolZayifliklari.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int siraNo = 1;
            foreach (RaporIcKontrolZayifliklari item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int sira = 0;
                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(DateTime.Now));
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.IlIrtibatOfisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.KontrolTanimi);
                XLS.HucreDegerYaz(satir, sira++, item.KontrolEtkinligi);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlani);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniSorumlusu);
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                XLS.HucreDegerYaz(satir, sira++, RiskAzaltmaPlaniMevcutDurumAdGetir(item.MevcutDurum));
                XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlani);
                XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlaniNedeni);
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniNotlar);


                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "IcKontrolZayifliklariRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> YillikRiskPlaniRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.YillikRiskPlaniHazirlaAsync(_kullanan, new RaporYillikRiskPlani()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "YillikRiskPlani.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int sayac = 0;
            XLS.HucreDegerYaz(satir - 3, 0, _sharedResource["RaporSablon.YillikRiskPlani.Baslik"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.SiraNo"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskKayitYili"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Koordinatorluk"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.BirimUnite"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.IlIrtibatOfisi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Donemi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Amac"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Hedef"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Surec"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.AltSurec"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskNo"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskAdi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskKokNedeni"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskinNiteligi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskKategorisi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskGecerlilikTarihi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.IliskiliOlay"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YapisalRiskPuani"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.MevcutKontroller"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.ArtikRiskPuani"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskeVerilecekCevap"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.AzaltmaPlani"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.AzaltmaPlaniSorumlusu"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.BitisTarihi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.MevcutDurum"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.ErtelenmisDegistirilmisZamanPlani"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.ZamanPlanindakiDegisiklikNedeni"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.AlzatmaPlaniDetayları"]);

            int siraNo = 1;
            foreach (RaporYillikRiskPlani item in sonuc.Liste)
            {
                sayac = 0;

                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int ArtikRiskSeviyesi = 0;
                Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref ArtikRiskSeviyesi);

                string mevcutKontroller = "";
                if (!string.IsNullOrWhiteSpace(item.MevcutKontroller))
                {
                    foreach (var deger in item.MevcutKontroller.Split('|'))
                    {
                        if (mevcutKontroller != "")
                            mevcutKontroller += "\n";
                        mevcutKontroller += deger.Replace("@", "/");
                    }
                }

                XLS.HucreDegerYaz(satir, sayac++, siraNo++);
                XLS.HucreDegerYaz(satir, sayac++, Arac.DegerAl(item.Yil));
                XLS.HucreDegerYaz(satir, sayac++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.IlIrtibatOfisiAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.StratejikPlanDonemAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.Amac);
                XLS.HucreDegerYaz(satir, sayac++, item.Hedef);
                XLS.HucreDegerYaz(satir, sayac++, item.Surec);
                XLS.HucreDegerYaz(satir, sayac++, item.AltSurec);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskNo);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskTanimi);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskinNiteligi);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskGecerlilikTarihi);
                XLS.HucreDegerYaz(satir, sayac++, item.IliskiliOlay);
                
                XLS.ArkaPlanRenk(satir, sayac, YapisalRiskSeviyesiRenkGetir(item.Etki, item.Olasilik, item.EtkiOlasilikMatrisi));
                XLS.HucreDegerYaz(satir, sayac++, item.YapisalRiskPuani);

                XLS.HucreDegerYaz(satir, sayac++, mevcutKontroller);

                XLS.ArkaPlanRenk(satir, sayac, ArtikRiskSeviyesiRenkGetir(ArtikRiskSeviyesi));
                XLS.HucreDegerYaz(satir, sayac++, item.ArtikRiskPuani);

                //XLS.HucreDegerYaz(satir, sayac++, item.MevcutDeger.GetValueOrDefault());
                //XLS.HucreDegerYaz(satir, sayac++, YapisalRiskSeviyesiGetir(item.Etki, item.Olasilik, item.EtkiOlasilikMatrisi));
                //XLS.HucreDegerYaz(satir, sayac++, Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref ArtikRiskSeviyesi));
                XLS.HucreDegerYaz(satir, sayac++, RiskeVerilecekCevapAdGetir(item.RiskeVerilecekCevap));
                XLS.HucreDegerYaz(satir, sayac++, item.AzaltmaPlani);
                XLS.HucreDegerYaz(satir, sayac++, item.AzaltmaPlaniSorumlusu);
                XLS.HucreDegerYaz(satir, sayac++, Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                XLS.HucreDegerYaz(satir, sayac++, RiskAzaltmaPlaniMevcutDurumAdGetir(Arac.DegerAl(item.MevcutDurum)));
                XLS.HucreDegerYaz(satir, sayac++, item.ErtlenmisZamanPlani);
                XLS.HucreDegerYaz(satir, sayac++, item.ErtlenmisZamanPlaniNedeni);
                XLS.HucreDegerYaz(satir, sayac++, item.AzaltmaPlaniNotlar);

                //XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesi);
                //XLS.HucreDegerYaz(satir, sira++, item.MevcutDeger.GetValueOrDefault());
                //XLS.HucreDegerYaz(satir, sira++, YapisalRiskSeviyesiGetir(item.Etki, item.Olasilik, item.EtkiOlasilikMatrisi));
                //XLS.HucreDegerYaz(satir, sira++, item.YapisalRiskPuani);
                //XLS.HucreDegerYaz(satir, sira++, Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref ArtikRiskSeviyesi));
                //XLS.HucreDegerYaz(satir, sira++, item.ArtikRiskPuani);
                //XLS.HucreDegerYaz(satir, sira++, RiskeVerilecekCevapAdGetir(item.RiskeVerilecekCevap));
                //XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlani);
                //XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniSorumlusu);
                //XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                //XLS.HucreDegerYaz(satir, sira++, RiskAzaltmaPlaniMevcutDurumAdGetir(item.MevcutDurum));
                //XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlani);
                //XLS.HucreDegerYaz(satir, sira++, item.ErtlenmisZamanPlaniNedeni);
                //XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlaniNotlar);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "YillikRiskPlaniRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> YariYilRiskAzaltmaPlaniRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.YariYilRiskAzaltmaPlaniHazirlaAsync(_kullanan, new RaporYariYilRiskAzaltmaPlani()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                Donem = gelenKriter.Donem,
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "YariYilRiskAzaltmaPlani.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int sayac = 0;
            XLS.HucreDegerYaz(satir - 3, 0, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.Baslik"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.SiraNo"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskKayitYili"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Koordinatorluk"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.BirimUnite"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskKategorisi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.IlIrtibatOfisi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Donemi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Amac"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.Hedef"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskNo"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskAdi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.RiskKokNedeni"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YapisalRiskPuani"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.MevcutKontroller"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.ArtikRiskPuani"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.AzaltmaPlani"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.SorumluKoordinatorluk"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.SorumluBirimUnite"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.BitisTarihi"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.MevcutDurum"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.ErtelenmisDegistirilmisZamanPlani"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.ZamanPlanindakiDegisiklikNedeni"]);
            XLS.HucreDegerYaz(satir - 1, sayac++, _sharedResource["RaporSablon.YariYilRiskAzaltmaPlani.AlzatmaPlaniDetayları"]);


            int siraNo = 1;
            foreach (RaporYariYilRiskAzaltmaPlani item in sonuc.Liste)
            {
                sayac = 0;

                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int ArtikRiskSeviyesi = 0;
                Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref ArtikRiskSeviyesi);

                string mevcutKontroller = "";
                if (!string.IsNullOrWhiteSpace(item.MevcutKontroller))
                {
                    foreach (var deger in item.MevcutKontroller.Split('|'))
                    {
                        if (mevcutKontroller != "")
                            mevcutKontroller += "\n";
                        mevcutKontroller += deger.Replace("@", "/");
                    }
                }


                XLS.HucreDegerYaz(satir, sayac++, siraNo++);
                XLS.HucreDegerYaz(satir, sayac++, Arac.DegerAl(item.Yil));
                //XLS.HucreDegerYaz(satir, sayac++, item.Donem > 0 ? item.Donem + ". " + _sharedResource["Raporlar.Alan.Donem"] : "");
                XLS.HucreDegerYaz(satir, sayac++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.IlIrtibatOfisiAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.StratejikPlanDonemAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.Amac);
                XLS.HucreDegerYaz(satir, sayac++, item.Hedef);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskNo);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.RiskTanimi);
                //XLS.HucreDegerYaz(satir, sayac++, item.AnahtarRiskGostergesi);
                //XLS.HucreDegerYaz(satir, sayac++, item.MevcutDeger.GetValueOrDefault());
                XLS.ArkaPlanRenk(satir, sayac, YapisalRiskSeviyesiRenkGetir(item.Etki, item.Olasilik, item.EtkiOlasilikMatrisi));
                XLS.HucreDegerYaz(satir, sayac++, item.YapisalRiskPuani);

                XLS.HucreDegerYaz(satir, sayac++, mevcutKontroller);

                XLS.ArkaPlanRenk(satir, sayac, ArtikRiskSeviyesiRenkGetir(ArtikRiskSeviyesi));
                XLS.HucreDegerYaz(satir, sayac++, item.ArtikRiskPuani);

                //XLS.HucreDegerYaz(satir, sayac++, item.MevcutDeger.GetValueOrDefault());
                //XLS.HucreDegerYaz(satir, sayac++, YapisalRiskSeviyesiGetir(item.Etki, item.Olasilik, item.EtkiOlasilikMatrisi));
                //XLS.HucreDegerYaz(satir, sayac++, Ortak.ArtikRiskSeviyesiGetir(item.Etki, item.Olasilik, item.KontrolKriteriAgirligi, _sharedResource, ref ArtikRiskSeviyesi));
                //XLS.HucreDegerYaz(satir, sayac++, RiskeVerilecekCevapAdGetir(item.RiskeVerilecekCevap));
                XLS.HucreDegerYaz(satir, sayac++, item.AzaltmaPlani);
                //XLS.HucreDegerYaz(satir, sayac++, item.AzaltmaPlaniSorumlusu);
                XLS.HucreDegerYaz(satir, sayac++, item.SorumluKoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sayac++, item.SorumluBirimAdi);
                XLS.HucreDegerYaz(satir, sayac++, Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                XLS.HucreDegerYaz(satir, sayac++, RiskAzaltmaPlaniMevcutDurumAdGetir(Arac.DegerAl(item.MevcutDurum)));
                XLS.HucreDegerYaz(satir, sayac++, item.ErtlenmisZamanPlani);
                XLS.HucreDegerYaz(satir, sayac++, item.ErtlenmisZamanPlaniNedeni);
                XLS.HucreDegerYaz(satir, sayac++, item.AzaltmaPlaniNotlar);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "YariYilRiskAzaltmaPlaniRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> UstYonetimStratejikPlanlamaRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.UstYonetimStratejikPlanlamaHazirlaAsync(_kullanan, new RaporUstYonetimStratejikPlanlama()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "UstYonetimStratejikPlanlama.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int maxDonem = 1;
            foreach (RaporUstYonetimStratejikPlanlama item in sonuc.Liste)
            {
                if (maxDonem < item.Donemler.Count)
                    maxDonem = item.Donemler.Count;
            }

            if (maxDonem > 1)
            {
                int izlemeDonemiSatir = 0;
                int izlemeDonemiSutun = 0;
                XLS.HucreAdAdresCoz("IzlemeDonemi", ref izlemeDonemiSatir, ref izlemeDonemiSutun);

                XLS.SutunAc(izlemeDonemiSutun + 3, (maxDonem - 1) * 3);

                for (int i = 0; i < maxDonem - 1; i++)
                {
                    izlemeDonemiSutun += 3;
                    XLS.HucreBirlestir(izlemeDonemiSatir, izlemeDonemiSutun, izlemeDonemiSatir, izlemeDonemiSutun + 2);
                    XLS.HucreDegerYaz(izlemeDonemiSatir, izlemeDonemiSutun, i + 2 + ". İzleme Dönemi");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun, "Planlanan Değer");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun + 1, "Gerçekleşen Değer 1");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun + 2, "Gerçekleşen Değer 2");
                }
            }

            int siraNo = 1;
            foreach (RaporUstYonetimStratejikPlanlama item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int sira = 0;
                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.IsbirligiYapacakKoordinatorlukler);
                XLS.HucreDegerYaz(satir, sira++, item.IsbirligiYapacakBirimler);
                XLS.HucreDegerYaz(satir, sira++, item.StratejikPlanDonemAdi);
                XLS.HucreDegerYaz(satir, sira++, item.Amac);
                XLS.HucreDegerYaz(satir, sira++, item.Hedef);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesi);
                XLS.HucreDegerYaz(satir, sira++, item.GostergeNo);
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.HedefeEtkisi));
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.BaslangicTarihi) + " " + Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.BaslangicDegeri));

                foreach (var donem in item.Donemler)
                {
                    XLS.HucreDegerYaz(satir, sira++, donem.PlanlananDeger);
                    XLS.HucreDegerYaz(satir, sira++, donem.GerceklesenDeger);
                    XLS.HucreDegerYaz(satir, sira++, donem.GerceklesenDegerYilSonu);
                }

                if (maxDonem > item.Donemler.Count)
                {
                    for (int i = 0; i < maxDonem - item.Donemler.Count; i++)
                    {
                        XLS.HucreDegerYaz(satir, sira++, "");
                        XLS.HucreDegerYaz(satir, sira++, "");
                        XLS.HucreDegerYaz(satir, sira++, "");
                    }
                }

                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.SapmaOrani));
                XLS.HucreDegerYaz(satir, sira++, item.SapmaNedeni);
                XLS.HucreDegerYaz(satir, sira++, item.Trend);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "UstYonetimStratejikPlanlamaRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> KoordinatorlerStratejikPlanlamaRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.KoordinatorlerStratejikPlanlamaHazirlaAsync(_kullanan, new RaporKoordinatorlerStratejikPlanlama()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "KoordinatorlerStratejikPlanlama.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);


            int maxDonem = 1;
            foreach (RaporKoordinatorlerStratejikPlanlama item in sonuc.Liste)
            {
                if (maxDonem < item.Donemler.Count)
                    maxDonem = item.Donemler.Count;
            }

            if (maxDonem > 1)
            {
                int izlemeDonemiSatir = 0;
                int izlemeDonemiSutun = 0;
                XLS.HucreAdAdresCoz("IzlemeDonemi", ref izlemeDonemiSatir, ref izlemeDonemiSutun);

                XLS.SutunAc(izlemeDonemiSutun + 2, (maxDonem - 1) * 2);

                for (int i = 0; i < maxDonem - 1; i++)
                {
                    izlemeDonemiSutun += 2;
                    XLS.HucreBirlestir(izlemeDonemiSatir, izlemeDonemiSutun, izlemeDonemiSatir, izlemeDonemiSutun + 1);
                    XLS.HucreDegerYaz(izlemeDonemiSatir, izlemeDonemiSutun, i + 2 + ". İzleme Dönemi");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun, "Planlanan Değer");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun + 1, "Gerçekleşen Değer");
                }
            }

            int siraNo = 1;
            foreach (RaporKoordinatorlerStratejikPlanlama item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int sira = 0;
                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.IsbirligiYapacakKoordinatorlukler);
                XLS.HucreDegerYaz(satir, sira++, item.IsbirligiYapacakBirimler);
                XLS.HucreDegerYaz(satir, sira++, item.StratejikPlanDonemAdi);
                XLS.HucreDegerYaz(satir, sira++, item.Amac);
                XLS.HucreDegerYaz(satir, sira++, item.Hedef);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesi);
                XLS.HucreDegerYaz(satir, sira++, item.GostergeNo);
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.HedefeEtkisi));
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.BaslangicTarihi) + " " + Arac.DateTimeToDDMMYYYY(item.BitisTarihi));
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.BaslangicDegeri));

                foreach (var donem in item.Donemler)
                {
                    XLS.HucreDegerYaz(satir, sira++, donem.PlanlananDeger);
                    XLS.HucreDegerYaz(satir, sira++, donem.GerceklesenDeger);
                }

                if (maxDonem > item.Donemler.Count)
                {
                    for (int i = 0; i < maxDonem - item.Donemler.Count; i++)
                    {
                        XLS.HucreDegerYaz(satir, sira++, "");
                        XLS.HucreDegerYaz(satir, sira++, "");
                    }
                }

                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.SapmaOrani));
                XLS.HucreDegerYaz(satir, sira++, item.SapmaNedeni);
                XLS.HucreDegerYaz(satir, sira++, item.Trend);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "KoordinatorlerStratejikPlanlamaRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> SurecAltSurecRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.SurecAltSurecHazirlaAsync(_kullanan, new RaporSurecAltSurec()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                lIlIrtibatOfisiKod = gelenKriter.lIlIrtibatOfisiKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "SurecAltSurec.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int siraNo = 1;
            foreach (RaporSurecAltSurec item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 25, satir, 0);

                XLS.HucreDegerYaz(satir, 0, siraNo++);
                XLS.HucreDegerYaz(satir, 1, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, 2, item.BirimAdi);
                XLS.HucreDegerYaz(satir, 3, item.SurecKod);
                XLS.HucreDegerYaz(satir, 4, item.SurecAdi);
                XLS.HucreDegerYaz(satir, 5, item.AltSurecAdi);
                XLS.HucreDegerYaz(satir, 6, item.SurecSahibi);
                XLS.HucreDegerYaz(satir, 7, item.SurecSahibiUnvan);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "SurecAltSurecRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> RiskIzlemeRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.RiskIzlemeHazirlaAsync(_kullanan, new RaporRiskIzleme()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
                DurumSorgu = gelenKriter.DurumSorgu
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "RiskIzleme.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);


            int maxDonem = 1;
            foreach (RaporRiskIzleme item in sonuc.Liste)
            {
                if (maxDonem < item.Donemler.Count)
                    maxDonem = item.Donemler.Count;
            }

            if (maxDonem > 1)
            {
                int izlemeDonemiSatir = 0;
                int izlemeDonemiSutun = 0;
                XLS.HucreAdAdresCoz("IzlemeDonemi", ref izlemeDonemiSatir, ref izlemeDonemiSutun);

                XLS.SutunAc(izlemeDonemiSutun + 2, (maxDonem - 1) * 2);

                for (int i = 0; i < maxDonem - 1; i++)
                {
                    izlemeDonemiSutun += 2;
                    XLS.HucreBirlestir(izlemeDonemiSatir, izlemeDonemiSutun, izlemeDonemiSatir, izlemeDonemiSutun + 1);
                    XLS.HucreDegerYaz(izlemeDonemiSatir, izlemeDonemiSutun, i + 2 + ". İzleme Dönemi");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun, "Gerçekleşen Değer");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun + 1, "Sıklık");
                }
            }


            int artikRiskSeviyesi = 0;

            int siraNo = 1;
            foreach (RaporRiskIzleme item in sonuc.Liste)
            {
                var etki = item.Etki.HasValue ? item.Etki.Value : 0;
                var olasilik = item.Olasilik.HasValue ? item.Olasilik.Value : 0;
                var kontrolKriteriAgirligi = item.KontrolKriteriAgirligi.HasValue ? item.KontrolKriteriAgirligi.Value : 0;
                var riskeVerilecekCevap = item.RiskeVerilecekCevap.HasValue ? item.RiskeVerilecekCevap.Value : 0;

                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int sira = 0;

                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, item.StratejikPlanDonemAdi);
                XLS.HucreDegerYaz(satir, sira++, item.Amac);
                XLS.HucreDegerYaz(satir, sira++, item.Hedef);
                XLS.HucreDegerYaz(satir, sira++, item.SurecAdi);
                XLS.HucreDegerYaz(satir, sira++, item.AltSurecAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskNo);
                XLS.HucreDegerYaz(satir, sira++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskTanimi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.IlIrtibatOfisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskGecerlilikTarihi);
                XLS.HucreDegerYaz(satir, sira++, item.IliskiliOlaylarAdi);
                XLS.HucreDegerYaz(satir, sira++, YapisalRiskSeviyesiGetir(etki, olasilik, item.EtkiOlasilikMatrisi));
                XLS.HucreDegerYaz(satir, sira++, item.KontrolAdi);
                XLS.HucreDegerYaz(satir, sira++, Ortak.ArtikRiskSeviyesiGetir(etki, olasilik, kontrolKriteriAgirligi, _sharedResource, ref artikRiskSeviyesi));
                XLS.HucreDegerYaz(satir, sira++, RiskeVerilecekCevapAdGetir(riskeVerilecekCevap));
                XLS.HucreDegerYaz(satir, sira++, item.AzaltmaPlani);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesi);

                foreach (var donem in item.Donemler)
                {
                    XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(donem.Deger));
                    XLS.HucreDegerYaz(satir, sira++, AnahtarRiskGostergesiDonemPeriyotAdGetir(donem.Periyot));
                }

                if (maxDonem > item.Donemler.Count)
                {
                    for (int i = 0; i < maxDonem - item.Donemler.Count; i++)
                    {
                        XLS.HucreDegerYaz(satir, sira++, "");
                        XLS.HucreDegerYaz(satir, sira++, "");
                    }
                }

                XLS.HucreDegerYaz(satir, sira++, item.Durum);
                XLS.HucreDegerYaz(satir, sira++, item.Trend);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "RiskEvreni" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> AnahtarRiskGostergesiRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.AnahtarRiskGostergesiHazirlaAsync(_kullanan, new RaporAnahtarRiskGostergesi()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "AnahtarRiskGostergesi.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int maxDonem = 1;
            foreach (RaporAnahtarRiskGostergesi item in sonuc.Liste)
            {
                if (maxDonem < item.Donemler.Count)
                    maxDonem = item.Donemler.Count;
            }

            if (maxDonem > 1)
            {
                int izlemeDonemiSatir = 0;
                int izlemeDonemiSutun = 0;
                XLS.HucreAdAdresCoz("IzlemeDonemi", ref izlemeDonemiSatir, ref izlemeDonemiSutun);

                XLS.SutunAc(izlemeDonemiSutun + 2, (maxDonem - 1) * 2);

                for (int i = 0; i < maxDonem - 1; i++)
                {
                    izlemeDonemiSutun += 2;
                    XLS.HucreBirlestir(izlemeDonemiSatir, izlemeDonemiSutun, izlemeDonemiSatir, izlemeDonemiSutun + 1);
                    XLS.HucreDegerYaz(izlemeDonemiSatir, izlemeDonemiSutun, i + 2 + ". İzleme Dönemi");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun, "Gerçekleşen Değer");
                    XLS.HucreDegerYaz(izlemeDonemiSatir + 1, izlemeDonemiSutun + 1, "Sıklık");
                }
            }

            int siraNo = 1;
            foreach (RaporAnahtarRiskGostergesi item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int sira = 0;
                //XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                //XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, item.Surec);
                XLS.HucreDegerYaz(satir, sira++, item.AltSurec);
                XLS.HucreDegerYaz(satir, sira++, item.RiskAdi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskTanimi);
                XLS.HucreDegerYaz(satir, sira++, item.RiskKategorisiAdi);
                XLS.HucreDegerYaz(satir, sira++, item.AnahtarRiskGostergesi);
                XLS.HucreDegerYaz(satir, sira++, item.VeriDayanagi);
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.YesilDeger));
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.SariDeger));
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.KirmiziDeger));
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.HedefDeger));

                foreach (var donem in item.Donemler)
                {
                    XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(donem.Deger));
                    XLS.HucreDegerYaz(satir, sira++, AnahtarRiskGostergesiDonemPeriyotAdGetir(donem.Periyot));
                }

                if (maxDonem > item.Donemler.Count)
                {
                    for (int i = 0; i < maxDonem - item.Donemler.Count; i++)
                    {
                        XLS.HucreDegerYaz(satir, sira++, "");
                        XLS.HucreDegerYaz(satir, sira++, "");
                    }
                }

                XLS.HucreDegerYaz(satir, sira++, item.Durum);
                XLS.HucreDegerYaz(satir, sira++, item.Trend);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "AnahtarRiskGostergesiRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> OlayRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.OlayRaporlamaHazirlaAsync(_kullanan, new RaporOlay()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "OlayRaporu.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int siraNo = 1;
            foreach (RaporOlay item in sonuc.Liste)
            {
                var riskArg = "";
                if (!string.IsNullOrWhiteSpace(item.Riskler))
                {
                    foreach (var r in item.Riskler.Split(';'))
                    {
                        if (riskArg != "")
                            riskArg += "\r\n";
                        riskArg += r.Replace("|", " - ");
                    }
                }

                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int sira = 0;
                XLS.HucreDegerYaz(satir, sira++, siraNo++);
                XLS.HucreDegerYaz(satir, sira++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sira++, item.BirimAdi);
                XLS.HucreDegerYaz(satir, sira++, Arac.DateTimeToDDMMYYYY(item.OlayTarihi));
                XLS.HucreDegerYaz(satir, sira++, item.OlayYeri);
                XLS.HucreDegerYaz(satir, sira++, item.OlayTanimi);
                XLS.HucreDegerYaz(satir, sira++, item.OlayKategorisi);
                XLS.HucreDegerYaz(satir, sira++, Arac.DegerAl(item.Tutari));
                XLS.HucreDegerYaz(satir, sira++, riskArg);

                satir++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "OlayRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }

        /// <summary>
        /// Kullanıcıdan gelen kriterlere göre rapor dosyası oluşturan metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns></returns>
        public async Task<FileContentResult> RiskYonetimiBeyannamesiRaporu(dynamic gelenKriter)
        {
            Sonuc sonuc = await _service.RiskYonetimiBeyannamesiHazirlaAsync(_kullanan, new RaporRiskYonetimiBeyannamesi()
            {
                KriterYil = Arac.ConvertToInt(gelenKriter.Yil, 0),
                KoordinatorlukAdi = gelenKriter.KoordinatorlukKod,
                BirimAdi = gelenKriter.BirimKod,
            });

            ITablo XLS = Arac.ExcelTablo();
            int sutun = 0;
            int satir = 0;
            int kaynakSatir = 0;

            string sablonAd = "RiskYonetimiBeyannamesiRaporu.xltx";
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            sablonAd = Path.Combine(sablonYol, sablonAd);

            string sablonDosyaAc = System.IO.Path.GetTempFileName();

            XLS.DosyaAc(sablonAd, sablonDosyaAc);
            XLS.HucreAdAdresCoz("baslamaSatiri", ref satir, ref sutun);

            int siraNo = 0;
            foreach (RaporRiskYonetimiBeyannamesi item in sonuc.Liste)
            {
                XLS.HucreKopyala(kaynakSatir, 0, kaynakSatir, 35, satir, 0);

                int sutunIc = 0;
                XLS.HucreDegerYaz(satir, sutunIc++, siraNo + 1);
                XLS.HucreDegerYaz(satir, sutunIc++, item.Yil);
                XLS.HucreDegerYaz(satir, sutunIc++, item.KoordinatorlukAdi);
                XLS.HucreDegerYaz(satir, sutunIc++, item.IslemYapanAdi);
                XLS.HucreDegerYaz(satir, sutunIc++, Arac.DateTimeToDDMMYYYY(item.IslemTarihi));

                satir++;
                siraNo++;
            }

            string ciktiTur = gelenKriter.CiktiTur.Value;
            if (string.IsNullOrWhiteSpace(ciktiTur)) ciktiTur = "PDF";

            XLS.DosyaSaklamaFormatAta(ciktiTur);
            XLS.DosyaSaklaTamYol();

            return Arac.DosyaGonder(sablonDosyaAc, "RiskYonetimiBeyannamesiRaporu" + Arac.DosyaAdUretSade() + "." + ciktiTur, true);
        }



        /// <summary>
        /// Gönderilen kritere göre AnahtarRiskGostergesiDonemPeriyot adını getiren metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns>
        /// string türünde değer döndürür
        /// </returns>
        private string AnahtarRiskGostergesiDonemPeriyotAdGetir(int gelenKriter)
        {
            if (gelenKriter == (int)EnumAnahtarRiskGostergesiDonemPeriyot.Aylik)
                return _sharedResource["AnahtarRiskGostergesiDonemPeriyot.Aylik"];
            else if (gelenKriter == (int)EnumAnahtarRiskGostergesiDonemPeriyot.Aylik3)
                return _sharedResource["AnahtarRiskGostergesiDonemPeriyot.Aylik3"];
            else if (gelenKriter == (int)EnumAnahtarRiskGostergesiDonemPeriyot.Aylik6)
                return _sharedResource["AnahtarRiskGostergesiDonemPeriyot.Aylik6"];
            else if (gelenKriter == (int)EnumAnahtarRiskGostergesiDonemPeriyot.Yillik)
                return _sharedResource["AnahtarRiskGostergesiDonemPeriyot.Yillik"];
            return "";
        }

        /// <summary>
        /// Gönderilen kritere göre riske verilecek cevap adını getiren metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns>
        /// string türünde değer döndürür
        /// </returns>
        private string RiskeVerilecekCevapAdGetir(int gelenKriter)
        {
            if (gelenKriter == (int)EnumRiskYonetimiRiskeVerilecekCevap.TransferEt)
                return _sharedResource["RiskYonetimi.RiskeVerilecekCevap.TransferEt"];
            else if (gelenKriter == (int)EnumRiskYonetimiRiskeVerilecekCevap.KabulEt)
                return _sharedResource["RiskYonetimi.RiskeVerilecekCevap.KabulEt"];
            else if (gelenKriter == (int)EnumRiskYonetimiRiskeVerilecekCevap.Reddet)
                return _sharedResource["RiskYonetimi.RiskeVerilecekCevap.Reddet"];
            else if (gelenKriter == (int)EnumRiskYonetimiRiskeVerilecekCevap.Azalt)
                return _sharedResource["RiskYonetimi.RiskeVerilecekCevap.Azalt"];
            return "";
        }

        /// <summary>
        /// Gönderilen kritere göre risk azaltma plani mevcut durum adını getiren metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns>
        /// string türünde değer döndürür
        /// </returns>
        private string RiskAzaltmaPlaniMevcutDurumAdGetir(int gelenKriter)
        {
            if (gelenKriter == (int)EnumRiskAzaltmaPlaniMevcutDurum.DevamEdiyor)
                return _sharedResource["RiskAzaltmaPlani.MevcutDurum.DevamEdiyor"];
            else if (gelenKriter == (int)EnumRiskAzaltmaPlaniMevcutDurum.Ertelendi)
                return _sharedResource["RiskAzaltmaPlani.MevcutDurum.Ertelendi"];
            else if (gelenKriter == (int)EnumRiskAzaltmaPlaniMevcutDurum.Durduruldu)
                return _sharedResource["RiskAzaltmaPlani.MevcutDurum.Durduruldu"];
            else if (gelenKriter == (int)EnumRiskAzaltmaPlaniMevcutDurum.IptalEdildi)
                return _sharedResource["RiskAzaltmaPlani.MevcutDurum.IptalEdildi"];
            else if (gelenKriter == (int)EnumRiskAzaltmaPlaniMevcutDurum.Tamamlandi)
                return _sharedResource["RiskAzaltmaPlani.MevcutDurum.Tamamlandi"];
            return "";
        }

        /// <summary>
        /// Gönderilen kritere göre risk adını getiren metod
        /// </summary>
        /// <param name="gelenKriter"></param>
        /// <returns>
        /// string türünde değer döndürür
        /// </returns>
        private string RiskTuruAdGetir(int gelenKriter)
        {
            if (gelenKriter == (int)EnumRiskEvreniRiskTuru.Tehdit)
                return _sharedResource["RiskEvreni.RiskTuru.Tehdit"];
            else if (gelenKriter == (int)EnumRiskEvreniRiskTuru.Firsat)
                return _sharedResource["RiskEvreni.RiskTuru.Firsat"];
            return "";
        }

        /// <summary>
        /// Gönderilen kritere göre yapısal risk seviyesini getiren metod
        /// </summary>
        /// <param name="etki"></param>
        /// <param name="olasilik"></param>
        /// <param name="etkiOlasilikMatrisi"></param>
        /// <returns>
        /// string türünde değer döndürür
        /// </returns>
        private string YapisalRiskSeviyesiGetir(int etki, int olasilik, string etkiOlasilikMatrisi)
        {
            string deger = "";

            if (!string.IsNullOrWhiteSpace(etkiOlasilikMatrisi))
            {
                dynamic[] obj = JsonConvert.DeserializeObject<dynamic[]>(etkiOlasilikMatrisi);
                foreach (var item in obj)
                {
                    if (Arac.ConvertToInt(item["etki"]) == etki && Arac.ConvertToInt(item["olasilik"]) == olasilik)
                    {
                        string seviyeAdi = _sharedResource["Konfigurasyon.YapisalRiskSeviyeAdi." + item["seviye"]];

                        deger = seviyeAdi;
                        break;
                    }
                }
            }
            return deger;
        }

        /// <summary>
        /// Gönderilen kritere göre yapısal risk seviyesinine ait renk bilgisini getiren metod
        /// </summary>
        /// <param name="etki"></param>
        /// <param name="olasilik"></param>
        /// <param name="etkiOlasilikMatrisi"></param>
        /// <returns>
        /// string türünde değer döndürür
        /// </returns>
        private System.Drawing.Color YapisalRiskSeviyesiRenkGetir(decimal etki, int olasilik, string etkiOlasilikMatrisi)
        {
            System.Drawing.Color donenDeger = System.Drawing.Color.Transparent;

            if (!string.IsNullOrWhiteSpace(etkiOlasilikMatrisi))
            {
                dynamic[] obj = JsonConvert.DeserializeObject<dynamic[]>(etkiOlasilikMatrisi);
                foreach (var item in obj)
                {
                    if (Arac.ConvertToInt(item["etki"]) == etki && Arac.ConvertToInt(item["olasilik"]) == olasilik)
                    {
                        string renk = item["renk"] + "";
                        var rgb = renk.Replace("rgb", "").Replace("(", "").Replace(")", "").Split(',');

                        int red = System.Convert.ToInt32(rgb[0]);
                        int green = System.Convert.ToInt32(rgb[1]);
                        int blue = System.Convert.ToInt32(rgb[2]);

                        donenDeger = System.Drawing.Color.FromArgb(red, green, blue);
                        break;
                    }
                }
            }
            return donenDeger;
        }

        /// <summary>
        /// Gönderilen kritere göre renk bilgisi getiren metod
        /// </summary>
        /// <param name="artikRiskSeviyesi"></param>
        /// <returns>
        /// System.Drawing.Color türünde nesne döndürür
        /// </returns>
        private System.Drawing.Color ArtikRiskSeviyesiRenkGetir(int artikRiskSeviyesi)
        {
            System.Drawing.Color donenDeger = System.Drawing.Color.Transparent;

            if (artikRiskSeviyesi == 1)
                donenDeger = System.Drawing.Color.FromArgb(147, 209, 76);
            else if (artikRiskSeviyesi == 2)
                donenDeger = System.Drawing.Color.FromArgb(0, 178, 78);
            else if (artikRiskSeviyesi == 3)
                donenDeger = System.Drawing.Color.FromArgb(255, 192, 1);
            else if (artikRiskSeviyesi == 4)
                donenDeger = System.Drawing.Color.FromArgb(253, 0, 3);
            else if (artikRiskSeviyesi == 5)
                donenDeger = System.Drawing.Color.FromArgb(192, 0, 2);

            return donenDeger;
        }

    }
}
