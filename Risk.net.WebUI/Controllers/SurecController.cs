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
using static System.Net.WebRequestMethods;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Süreç iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    public class SurecController : GenelController
    {
        /// <summary>
        /// ISurecService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ISurecService _service;
        /// <summary>
        /// IAltSurecService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IAltSurecService _serviceAltSurec;
        private readonly IViewKoordinatorlukService _serviceKoordinatorluk;
        private readonly IViewBirimService _serviceBirim;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.SurecController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceAltSurec"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public SurecController(ISurecService service,
                                    IAltSurecService serviceAltSurec,
                                    IViewKoordinatorlukService serviceKoordinatorluk,
                                    IViewBirimService serviceBirim,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceAltSurec = serviceAltSurec;
            _serviceKoordinatorluk = serviceKoordinatorluk;
            _serviceBirim = serviceBirim;
        }

        /// <summary>
        /// Surec View sayfasý açýldýðýnda çalýþan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayý görme yetkisi olan kullanýcýlar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "*")]
        public IActionResult Index()
        {
            ViewBag.KoordinatorlukKodu = _kullanan.KoordinatorlukKod;
            ViewBag.BirimKodu = _kullanan.BirimKod;

            if (Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI", _kullanan))
                ViewBag.KayitYetki = true;
            else
                ViewBag.KayitYetki = false;

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

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo);

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
        public async Task<ActionResult> Kaydet([FromBody] Surec form)
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
            Surec form = new Surec();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

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
        public async Task<IActionResult> Onayla(string kod)
        {
            Surec form = new Surec();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Surec seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer(string koordinatorlukKod, string birimKod)
        {
            var sonuc = await _service.ListeleAsync(_kullanan, koordinatorlukKod, birimKod, (int)ENUMDurum.Aktif);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (Surec item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Numara + " - " + item.Adi });
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
        public async Task<IActionResult> AltSurecKayitGetir(string kod)
        {
            Sonuc sonuc = await _serviceAltSurec.KayitGetirAsync(_kullanan, kod);

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
        public async Task<ActionResult> AltSurecKaydet(AltSurec form)
        {
            form.Durum = (int)ENUMDurum.Aktif;
            Sonuc sonuc = await _serviceAltSurec.KaydetAsync(_kullanan, form);

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
        public async Task<IActionResult> AltSurecSil(string kod)
        {
            AltSurec form = new AltSurec();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _serviceAltSurec.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Alt Surec seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerAltSurec(string surecKod)
        {
            var sonuc = await _serviceAltSurec.ListeleAsync(_kullanan, surecKod, (int)ENUMDurum.Aktif);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (AltSurec item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Numara + " - " + item.Adi });
            }

            return Ok(donenDeger);
        }


        /// <summary>
        /// Ýl bazýnda süreç ve alt süreçlerin kayýt edilmesini saðlar
        /// Kurum tarafýndan kayýt edilmek istenen süreçler olduðu için yapýldý. Silinebilir Melih 12.06.2023
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpGet]
        public async Task<IActionResult> SurecAltSurecIlBazindaKaydet()
        {
            string surecler = "ÝK.1.1\tÝl Koordinatörlüðü\tProje Baþvuru Yönetimi Birimi\tProje Baþvuru Kayýt Ýþlemleri";
            surecler += "\nÝK.1.2\tÝl Koordinatörlüðü\tProje Baþvuru Yönetimi Birimi\tTamlýk(Ýdari) / Uygunluk Kontrolleri Öncesi Ýþlemler";
            surecler += "\nÝK.1.3\tÝl Koordinatörlüðü\tProje Baþvuru Yönetimi Birimi\tProje Baþvurularýnýn Tamlýk(Ýdari) / Uygunluk Kontrol Ýþlemleri";
            surecler += "\nÝK.1.4\tÝl Koordinatörlüðü\tProje Baþvuru Yönetimi Birimi\tÝþ Planý Kapsamýndaki Ýþlemler";
            surecler += "\nÝK.1.5\tÝl Koordinatörlüðü\tProje Baþvuru Yönetimi Birimi\tBaþvurunun Uygun Kabul Edilmesi, Geri Çekilmesi ve Tamlýk, Uygunluk veya Ýþ Planý Analizi Sonucunda Reddedilmesine Ýliþkin Ýþlemler";
            surecler += "\nÝK.2.1\tÝl Koordinatörlüðü\tProje Baþvuru Yönetimi Birimi\tSözleþmenin Hazýrlanmasý ve Ýmzalatýlmasý veya Reddedilmesi";
            surecler += "\nÝK.2.2\tÝl Koordinatörlüðü\tProje Baþvuru Yönetimi Birimi\tZeyilname ve Fesih iþlemleri";
            surecler += "\nÝK.2.3\tÝl Koordinatörlüðü\tProje Baþvuru Yönetimi Birimi\tSözleþme ile Ýlgili Diðer Ýþlemler";
            surecler += "\nÝK.3.1\tÝl Koordinatörlüðü\tYerinde Kontrol Birimi\tSözleþme Öncesi Yerinde Kontrol Ýþlemleri";
            surecler += "\nÝK.3.2\tÝl Koordinatörlüðü\tYerinde Kontrol Birimi\tÖdeme Öncesi Yerinde Kontrol Ýþlemleri";
            surecler += "\nÝK.3.3\tÝl Koordinatörlüðü\tYerinde Kontrol Birimi\tUygulama Sonrasý Yerinde Kontrol Ýþlemleri";
            surecler += "\nÝK.3.4\tÝl Koordinatörlüðü\tYerinde Kontrol Birimi\tÝtirazlar Kapsamýnda Yapýlan Ýþlemler";
            surecler += "\nÝK.3.5\tÝl Koordinatörlüðü\tYerinde Kontrol Birimi\tZeyilnameler Kapsamýnda Yapýlan Ýþlemler";
            surecler += "\nÝK.3.6\tÝl Koordinatörlüðü\tYerinde Kontrol Birimi\tYerinde Kontrol Verilerinin Kaydedilmesi";
            surecler += "\nÝK.4.1\tÝl Koordinatörlüðü\tÖdeme Talep Ýþlemleri Birimi\tÖdeme Talep Paketi(ÖTP) Ýþlemleri";
            surecler += "\nÝK.4.2\tÝl Koordinatörlüðü\tÖdeme Talep Ýþlemleri Birimi\tTahakkuk Ýþlemleri";
            surecler += "\nÝK.4.3\tÝl Koordinatörlüðü\tÖdeme Talep Ýþlemleri Birimi\tÖTP ile ilgili yararlanýcýlarýn bilgilendirilmesi";
            surecler += "\nÝK.4.4\tÝl Koordinatörlüðü\tÖdeme Talep Ýþlemleri Birimi\tUsulsüzlük Ýþlemlerinin Yönetimi";
            surecler += "\nÝK.4.5\tÝl Koordinatörlüðü\tÖdeme Talep Ýþlemleri Birimi\tProje Kaynaklý Geri Alým Ýþlemleri";
            surecler += "\nÝK.5.1\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tMaaþ, Harcýrah ve Satýnalmaya Baðlý Ödeme Ýþlemleri";
            surecler += "\nÝK.5.2\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tÝK Personel Özlük Ýþlemleri";
            surecler += "\nÝK.5.3\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tÝþçi Özlük Ýþlemleri";
            surecler += "\nÝK.5.4\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tÝdari Ýþler";
            surecler += "\nÝK.5.5\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tGenel Evrak Ýþlemlerinin Yürütülmesi";
            surecler += "\nÝK.5.6\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tÝK'nýn Diðer Ýþ ve Ýþlemleri";
            surecler += "\nÝK.5.7\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tBilgi Güvenliði Yönetim Sistemi Ýþlemleri";
            surecler += "\nÝK.5.8\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tAð, Sistem ve Güvenlik Yönetimi Ýþlemleri";
            surecler += "\nÝK.5.9\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tBasýn ve Medya Faaliyetleri";
            surecler += "\nÝK.5.10\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tTanýtým ve Organizasyon Hizmetleri";
            surecler += "\nÝK.5.11\tÝl Koordinatörlüðü\tDestek Hizmetleri Birimi\tHalkla Ýliþkiler Hizmetleri";

            string altSurecler = "ÝK.1.1\tÝK 1.1.1\tBaþvuru paketinin kontrol edilerek teslim alýnmasý";
            altSurecler += "\nÝK.1.1\tÝK.1.1.2\tBaþvuru paketinin kaydýnýn yapýlmasý ve arþive teslim edilmesi";
            altSurecler += "\nÝK.1.1\tÝK.1.1.3\tBaþvuru için  “Ýl Baþvuru Dosyasý” ve “Arþiv Klasörü” oluþturulmasý";
            altSurecler += "\nÝK.1.2\tÝK.1.2.1\tBaþvuru izleme amaçlý veri tablolarýnýn oluþturulmasý";
            altSurecler += "\nÝK.1.2\tÝK.1.2.2\tDanýþman Firma ve Tedarikçi Veritabaný (DFTV) formunun doldurulmasý";
            altSurecler += "\nÝK.1.2\tÝK.1.2.3\tKopya ve manipülasyon olabilecek riskli baþvurularýn belirlenmesi ve ek kontrollerinin yapýlmasý";
            altSurecler += "\nÝK.1.2\tÝK.1.2.4\tRiskli baþvurularýn yerinde kontrol birimine bildirilmesi";
            altSurecler += "\nÝK.1.3\tÝK.1.3.1\tBaþvuru paketlerinin tamlýk kontrolünün idari kontrol listeleri üzerinden gerçekleþtirilmesi";
            altSurecler += "\nÝK.1.3\tÝK.1.3.2\tBaþvurularýn “Uygunluk Kontrol Listelerine” göre kontrolünün yapýlmasý";
            altSurecler += "\nÝK.1.3\tÝK.1.3.3\tBeyan edilen harcamalarýn uygunluðunun kontrol edilmesi, gerektiðinde pazar araþtýrmasý yapýlmasýnýn talep edilmesi";
            altSurecler += "\nÝK.1.3\tÝK.1.3.4\tBaþvuru paketinde eksik belgesi olan ve/veya ek bilgi alýnmasý gereken baþvuru sahiplerinin bilgilendirilmesi ve eksikliklerin tamamlatýlmasý";
            altSurecler += "\nÝK.1.3\tÝK.1.3.5\tBaþvuru sahipleri tarafýndan sunulan bilgi ve belgelerin kontrol edilmesi";
            altSurecler += "\nÝK.1.3\tÝK.1.3.6\tKontroller sýrasýnda tespit edilen muhtemel þüpheli dolandýrýcýlýklarýn Hukuk Müþavirliðine bildirilmesi";
            altSurecler += "\nÝK.1.3\tÝK.1.3.7\tUygun baþvurularýn listesinin sözleþme öncesi yerinde kontrollerinin yapýlabilmesi amacýyla yerinde kontrol birimine iletilmesi";
            altSurecler += "\nÝK.1.3\tÝK.1.3.8\tYKB'den iletilen ve yerinde kontrolü tamamlanan baþvurular listesine göre Harcamalarýn Uygunluðu, Destek Seviyesi ve Miktarý Kontrol Listesinin (ÝK–1120) tamamlanmasý";
            altSurecler += "\nÝK.1.4\tÝK.1.4.1\tTüm baþvurular için baþvuru sahibinin finansman kaynaklarýnýn deðerlendirilmesi (mali analiz) iþlemleri";
            altSurecler += "\nÝK.1.4\tÝK.1.4.2\tÝþ planý türünün B3 Ýþ Planý olmasý durumunda Ýþ Planý Analizinin yapýlmasý";
            altSurecler += "\nÝK.1.5\tÝK.1.5.1\tUygun olan baþvurular listesinin (Ýl-1125) hazýrlanmasý ve Merkeze (PYK) gönderilmesi";
            altSurecler += "\nÝK.1.5\tÝK.1.5.2\tBaþvurunun geri çekilmesi veya reddedilmesine iliþkin iþlemler";
            altSurecler += "\nÝK.1.5\tÝK.1.5.3\tRedde itiraz iþlemlerinin deðerlendirilmesi ve sonucun Merkeze bildirilmesi";
            altSurecler += "\nÝK.1.5\tÝK.1.5.4\tÝþlemi tamamlanan baþvuru paketlerinin ve il baþvuru dosyalarýnýn arþive teslim edilmesi";
            altSurecler += "\nÝK.2.1\tÝK.2.1.1\tSözleþmenin hazýrlanmasý, imzalatýlmasý ve arþive gönderilmek üzere dosyanýn düzenlenmesi";
            altSurecler += "\nÝK.2.1\tÝK.2.1.2\t'Sözleþme Bildirim Dokümaný'nýn oluþturulmasý ve baþvuru sahibi ile Ýl Koordinatörlüklerindeki ilgili birimlere bildirilmesi";
            altSurecler += "\nÝK.2.1\tÝK.2.1.3\tSözleþme aþamasýnda reddedilen projeler ile ilgili iþlemlerin yerine getirilmesi";
            altSurecler += "\nÝK.2.2\tÝK.2.2.1\tZeyilnamenin hazýrlanmasý ve imzalatýlmasý veya reddedilmesi ile arþive gönderilmek üzere dosyanýn düzenlenmesi";
            altSurecler += "\nÝK.2.2\tÝK.2.2.2\tSözleþme deðiþikliði ile ilgili olarak diðer birimlere ve faydalanýcýya  bilgi verilmesi";
            altSurecler += "\nÝK.2.2\tÝK.2.2.3\tSözleþme feshi ile ilgili iþlemlerin yerine getirilmesi";
            altSurecler += "\nÝK.2.3\tÝK.2.3.1\tÝmzalanmýþ sözleþmeler, zeyilname ve fesih ile ilgili gerekli bilgilerin sisteme girilmesi";
            altSurecler += "\nÝK.2.3\tÝK.2.3.2\tSözleþme prosedürleri ile ilgili olarak baþvuru sahibi/faydalanýcýnýn itirazlarýnýn deðerlendirilmesi";
            altSurecler += "\nÝK.3.1\tÝK.3.1.1\tProjelerin risk puanlarýnýn belirlenmesi ve sýnýflandýrýlmasý";
            altSurecler += "\nÝK.3.1\tÝK.3.1.2\tHer bir baþvuruya iliþkin teknik proje analizinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.3.1\tÝK.3.1.3\tSözleþme Öncesi Yerinde Kontrol Ýþlemlerinin Gerçekleþtirilmesi";
            altSurecler += "\nÝK.3.1\tÝK.3.1.4\tTeknik proje deðerlendirme sonuçlarý ve diðer ilgili/destekleyici dokümanlarýn hazýrlanmasý";
            altSurecler += "\nÝK.3.1\tÝK.3.1.5\tKontroller tamamlandýktan sonra, baþvuru paketi ve il baþvuru dosyasýnýn arþive/e-arþive teslim edilmesi";
            altSurecler += "\nÝK.3.1\tÝK.3.1.6\tYerinde kontrolü tamamlanan baþvurular listesinin PBYB'ye iletilmesi";
            altSurecler += "\nÝK.3.2\tÝK.3.2.1\tProjelerin risk puanlarýnýn belirlenmesi ve sýnýflandýrýlmasý";
            altSurecler += "\nÝK.3.2\tÝK.3.2.2\tAra dönem yerinde kontrol iþlemlerinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.3.2\tÝK.3.2.3\tÖdeme öncesi yerinde kontrol iþlemlerinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.3.2\tÝK.3.2.4\tYerinde kontrolü tamamlanan baþvurular listesinin ÖTÝB'e iletilmesi";
            altSurecler += "\nÝK.3.3\tÝK.3.3.1\tProjelerin risk puanlarýnýn belirlenmesi ve sýnýflandýrýlmasý";
            altSurecler += "\nÝK.3.3\tÝK.3.3.2\tYýllýk olarak yenilenen sigorta poliçelerinin takibi";
            altSurecler += "\nÝK.3.3\tÝK.3.3.3\tUygulama sonrasý yerinde kontrol iþlemlerinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.3.4\tÝK.3.4.1\tBaþvurunun reddine veya taslak sözleþmeye itiraz iþlemleri kapsamýnda, ilgisi nedeniyle talep edilmesi durumunda; “Ýtiraz Deðerlendirme Komisyonu Raporu ve Tutanaðý”nýn hazýrlanmasý";
            altSurecler += "\nÝK.3.5\tÝK.3.5.1\tPBYB iþ ve iþlemlerinde kullanýlmak üzere; teknik deðiþiklik içeren büyük deðiþiklik kapsamýndaki talepleri deðerlendirme amaçlý 1225-A/B formlarýnýn ve görüþ yazýlarýnýn hazýrlanmasý";
            altSurecler += "\nÝK.3.6\tÝK.3.6.1\tYerinde kontrol verilerinin kaydedilmesi";
            altSurecler += "\nÝK.4.1\tÝK.4.1.1\tÖdeme Talep Paketinin (ÖTP) kayýt iþlemlerinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.4.1\tÝK.4.1.2\tÖTP'ler üzerinde ÝK Ýdari (Tamlýk) Kontrol Ýþlemleri";
            altSurecler += "\nÝK.4.1\tÝK.4.1.3\tÖTP'ler üzerinde ÝK Ýdari (Uygunluk) Kontrol Ýþlemleri";
            altSurecler += "\nÝK.4.1\tÝK.4.1.4\tKontrol edilen ÖTP'nin yerinde kontrollerinin yapýlmasý için YKB'ye gönderilmesi";
            altSurecler += "\nÝK.4.2\tÝK.4.2.1\tTahakkuk tutarýnýn hesaplanmasý";
            altSurecler += "\nÝK.4.2\tÝK.4.2.2\tYerinde Kontrol Birimi tarafýndan hazýrlanan rapora dayanarak 'Tahakkuk Formu'nun hazýrlanmasý";
            altSurecler += "\nÝK.4.2\tÝK.4.2.3\tTahakkuka iliþkin belgelerin kontrolünün yapýlmasý";
            altSurecler += "\nÝK.4.2\tÝK.4.2.4\tGerektiðinde ek tahakkuk iþlemlerinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.4.2\tÝK.4.2.5\tÝlgili Koordinatörlüklere tahakkuka iliþkin bildirim mektuplarýnýn hazýrlanamsý";
            altSurecler += "\nÝK.4.3\tÝK.4.3.1\tÖTP'nin içeriði ve sözleþmede belirtilen teslim zamaný ile ilgili olarak yararlanýcýlara tebligat gönderilmesi";
            altSurecler += "\nÝK.4.3\tÝK.4.3.2\tYararlanýcýya gönderilmek üzere ÖTP'nin kabulüne iliþkin bildirim mektubunun hazýrlanmasý ve gönderilmesi";
            altSurecler += "\nÝK.4.3\tÝK.4.3.3\tÖTP'nin tamlýk kontrollerinde herhangi bir eksikliðin belirlenmesi durumunda gerekli dokümanlarýn saðlanmasý için yararlanýcýya bildirim mektubunun gönderilmesi";
            altSurecler += "\nÝK.4.3\tÝK.4.3.4\tÖTP'nin uygunluk kontrollerinde herhangi bir eksikliðin belirlenmesi durumunda gerekli bilgilerin saðlanmasý için yararlanýcýya bildirim mektubunun gönderilmesi";
            altSurecler += "\nÝK.4.4\tÝK.4.4.1\tUsulsüzlük bildirimlerinin Hukuk Müþavirliðine iletilmesi ";
            altSurecler += "\nÝK.4.4\tÝK.4.4.2\tÞüpheli dolandýrýcýlýk iþlemlerinin Hukuk Müþavirliðine bildirilmesi";
            altSurecler += "\nÝK.4.4\tÝK.4.4.3\tÝdari hata bildirimlerinin kaydedilmesi ve Hukuk Müþavirliðine bildirilmesi";
            altSurecler += "\nÝK.4.4\tÝK.4.4.4\tUsulsüzlük þüphesi içeren þikayet ve ihbarlarýn deðerlendirilmesi, ilgili Merkez Birimlerine bildirilmesi";
            altSurecler += "\nÝK.4.5\tÝK.4.5.1\tBorç bildirim iþlemleri";
            altSurecler += "\nÝK.4.5\tÝK.4.5.2\tÝcra ödeme emri iþlemleri";
            altSurecler += "\nÝK.4.5\tÝK.4.5.3\tHaciz Ýþlemlerinin (Tapu, Trafik, Banka Haciz Bildirileri ve Fiili Haciz Ýþlemleri) yapýlmasý";
            altSurecler += "\nÝK.4.5\tÝK.4.5.4\t6183 sayýlý AATUHK kapsamýnda geri alým sürecinin tamamlanmasýna yönelik iþlemlerin yapýlmasý";
            altSurecler += "\nÝK.5.1\tÝK.5.1.1\tPersonel maaþlarýnýn hesaplanmasý ve kontrolü";
            altSurecler += "\nÝK.5.1\tÝK.5.1.2\tMaaþ ödeme iþlemlerinin yapýlmasý";
            altSurecler += "\nÝK.5.1\tÝK.5.1.3\tHarcýrah ödeme iþlemlerinin yapýlmasý";
            altSurecler += "\nÝK.5.1\tÝK.5.1.4\tSatýnalmaya baðlý ödeme iþlemlerinin yapýlmasý";
            altSurecler += "\nÝK.5.1\tÝK.5.1.5\tMal ve hizmet alýmlarýyla ilgili doðrudan temin süreçlerinin yönetilmesi";
            altSurecler += "\nÝK.5.1\tÝK.5.1.6\tÝK'nin elektrik, su, doðalgaz, posta pulu, kargo hizmeti ihtiyaçlarýnýn karþýlanmasý";
            altSurecler += "\nÝK.5.1\tÝK.5.1.7\tBeyannamelerin süresi içinde bildirimlerinin yapýlarak ödemelerin gerçekleþtirilmesi ";
            altSurecler += "\nÝK.5.1\tÝK.5.1.8\tÝK hizmet binasýnýn kirasýnýn ödenmesi";
            altSurecler += "\nÝK.5.2\tÝK.5.2.1\tPersonel özlük dosyasý içeriðindeki evrakýn düzenlenmesi ve takip edilmesi (Atama, izin, geçici görevlendirme, yer deðiþiklikleri, iþ tanýmlarý, mal beyanlarý vb.)";
            altSurecler += "\nÝK.5.2\tÝK.5.2.2\tÖzlük iþlemlerinin 'Kurumsal Bilgi Sistemi - KBS üzerinden takip edilmesi";
            altSurecler += "\nÝK.5.2\tÝK.5.2.3\tDisiplin iþlemlerinin takip edilmesi";
            altSurecler += "\nÝK.5.2\tÝK.5.2.4\tPersonel performans deðerlendirme iþlemlerinin (Ödül, baþarý belgesi vb.) gerçekleþtirilmesi";
            altSurecler += "\nÝK.5.2\tÝK.5.2.5\tSaðlýk raporlarýnýn SGK sistemine giriþi ve gerekli iþlemlerin takip edilmesi";
            altSurecler += "\nÝK.5.3\tÝK.5.3.1\t696 sayýlý KHK ile kadroya alýnan sürekli iþçilerin mali ve sosyal haklarýnýn takip edilmesi";
            altSurecler += "\nÝK.5.3\tÝK.5.3.2\t696 sayýlý KHK ile kadroya alýnan sürekli iþçilerin disiplin iþlemlerinin takip edilmesi";
            altSurecler += "\nÝK.5.3\tÝK.5.3.3\t696 sayýlý KHK ile kadroya alýnan sürekli iþçiler hakkýndaki icra takip iþlemlerini yürütülmesi";
            altSurecler += "\nÝK.5.3\tÝK.5.3.4\tKurumdan ayrýlan iþçi personelin tazminat iþ ve iþlemlerinin yürütülmesi";
            altSurecler += "\nÝK.5.4\tÝK.5.4.1\tSürekli iþçilerin (Temizlik ve kat görevlileri, özel güvenlik görevlileri, þoförler, yemekhane personeli, teknik görevliler vb.) sevk ve idaresi";
            altSurecler += "\nÝK.5.4\tÝK.5.4.2\tHizmet araçlarýnýn sevk ve idaresinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.5.4\tÝK.5.4.3\tÝK'nin taþýnýr ve ambar iþlemlerinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.5.4\tÝK.5.4.4\tHizmet araçlarýnýn ve jeneratörlerin akaryakýt sarfiyatýnýn kontrol edilmesi";
            altSurecler += "\nÝK.5.4\tÝK.5.4.5\tHizmet binasýnýn bakým onarým iþlerinin yürütülmesi";
            altSurecler += "\nÝK.5.4\tÝK.5.4.6\tSivil savunma iþlemlerinin yürütülmesi";
            altSurecler += "\nÝK.5.5\tÝK.5.5.1\tGenel Evrak / Genel Evrak Arþivi / Ýlgili Birimlere evrak havalesi ile ilgili iþ ve iþlemlerin Koordinasyonunun gerçekleþtirilmesi";
            altSurecler += "\nÝK.5.5\tÝK.5.5.2\tPostaya düþen evrakýn gönderim iþlemlerinin yapýlmasý";
            altSurecler += "\nÝK.5.5\tÝK.5.5.3\tÝK (Genel Evrak) Arþivinin ve Arþiv malzemelerinin düzen ve fiziksel koþullarýnýn yönetilmesi";
            altSurecler += "\nÝK.5.6\tÝK.5.6.1\tProje dosyalarýnýn muhafaza edilmesi ve eriþimin saðlanmasý";
            altSurecler += "\nÝK.5.6\tÝK.5.6.2\tDosyalarýn taranarak E-Arþiv sistemine kaydedilmesi";
            altSurecler += "\nÝK.5.6\tÝK.5.6.3\tÝdari davalara iliþkin kiþi borç veya alacaklarýna yönelik mali hakediþ tablolarýnýn oluþturulmasý ile tahakkuk ve ödeme iþlemlerinin gerçekleþtirilmesi";
            altSurecler += "\nÝK.5.6\tÝK.5.6.4\tÝK personeline yönelik eðitim faaliyetlerinin takip edilemsi";
            altSurecler += "\nÝK.5.7\tÝK.5.7.1\t6698 Sayýlý Kiþisel Verilerin Korunmasý Kanunu gerekliliklerinin saðlanmasý";
            altSurecler += "\nÝK.5.7\tÝK.5.7.2\tISO 27001 Bilgi Güvenliði Yönetim Sistemi Standartlarýna yönelik faaliyetlerin gerçekleþtirilmesi ve gerekliliklerinin saðlanmasý";
            altSurecler += "\nÝK.5.8\tÝK.5.8.1\tÝK fiziksel biliþim altyapýsýnýn kurulmasý, yönetilmesi ve güvenliðinin saðlanmasý";
            altSurecler += "\nÝK.5.8\tÝK.5.8.2\tE-posta, dosya sunucularý, kullanýcý hesaplarý, güvenlik kameralarý, telefon santrali, sistem odasý gibi teknik hizmetlerin yürütülmesi";
            altSurecler += "\nÝK.5.9\tÝK.5.9.1\tSosyal medya hesaplarýnýn yönetilmesi";
            altSurecler += "\nÝK.5.9\tÝK.5.9.2\tÝK basýn faaliyetlerinin yürütülmesi ve takip edilmesi";
            altSurecler += "\nÝK.5.9\tÝK.5.9.3\tTanýtým ve bilgilendirme amaçlý film vb. görsel yayýnlarýn hazýrlanmasý";
            altSurecler += "\nÝK.5.10\tÝK.5.10.1\tKurumsal organizasyonlarýn (Fuar, Organizasyon, Lansman vb.) gerçekleþtirilmesi";
            altSurecler += "\nÝK.5.10\tÝK.5.10.2\tKurumsal tanýtým faaliyetlerinin planlanmasý ve gerçekleþtirilmesi";
            altSurecler += "\nÝK.5.11\tÝK.5.11.1\tDilekçe ile  gelen baþvurularýn yönetilmesi";
            altSurecler += "\nÝK.5.11\tÝK.5.11.2\tYardým Masasý Telefon Hattýna (444 85 35) ve Yardým Masasý Sistemine gelen baþvurularýn yönetilmesi ve takip edilmesi";


            string[] satirlarSurec = surecler.Split("\n");
            string[] satirlarAltSurec = altSurecler.Split("\n");

            var koordinatorlukListe = await _serviceKoordinatorluk.ListeleAsync(_kullanan);

            foreach (ViewKoordinatorluk item in koordinatorlukListe.Liste)
            {
                if (item.Tur != 40) continue;

                string koorKod = item.Kod;
                var birimListe = await _serviceBirim.ListeleAsync(_kullanan, koorKod);

                foreach (string sSatir in satirlarSurec)
                {
                    if (string.IsNullOrWhiteSpace(sSatir)) continue;
                    string birimKod = "";

                    string[] surec = sSatir.Split("\t");

                    foreach (ViewBirim birim in birimListe.Liste)
                    {
                        if (birim.Adi == surec[2])
                        {
                            birimKod = birim.Kod;
                            break;
                        }
                    }

                    if (birimKod == "") continue;

                    Surec vs = new Surec();
                    vs.Numara = surec[0];
                    vs.KoordinatorlukKod = koorKod;
                    vs.BirimKod = birimKod;
                    vs.Adi = surec[3];
                    vs.Durum = 1;
                    vs.AltSurecler = new List<AltSurec>();

                    foreach (string aSatir in satirlarAltSurec)
                    {
                        if (string.IsNullOrWhiteSpace(aSatir)) continue;
                        string[] aSurec = aSatir.Split("\t");

                        if (vs.Numara != aSurec[0]) continue;

                        AltSurec vas = new AltSurec();
                        vas.Numara = aSurec[1];
                        vas.Adi = aSurec[2];

                        vs.AltSurecler.Add(vas);
                    }

                    Sonuc sonuc = await _service.KaydetAsync(_kullanan, vs);
                }
            }

            return Ok();
        }
    }
}
