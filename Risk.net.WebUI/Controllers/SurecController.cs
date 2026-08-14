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
    /// Süreç işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class SurecController : GenelController
    {
        /// <summary>
        /// ISurecService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ISurecService _service;
        /// <summary>
        /// IAltSurecService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IAltSurecService _serviceAltSurec;
        private readonly IViewKoordinatorlukService _serviceKoordinatorluk;
        private readonly IViewBirimService _serviceBirim;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.SurecController" /> 'ın yeni bir örneğini başlatan sınıf
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
        /// Surec View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
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
        /// Kullanıcı ekranından aldığı sayfa no, kayıt sayısı, sıralama alanı ve arama kriter 
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
        public async Task<ActionResult> Kaydet([FromBody] Surec form)
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
            Surec form = new Surec();
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
            Surec form = new Surec();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Surec seçim kutularının doldurulması için çağrılan metod
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
        /// Listeden seçilen kaydın tüm bilgisini ilgili servis aracılığıyla getiren metod
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
        /// Kullanıcı tarafından girilen bilgileri ilgili servise kayıt edilmesi için gönderen metod
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
        /// Listeden seçilen kaydın silme işlemini sağlayan metod
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
        /// Ekranlardaki Alt Surec seçim kutularının doldurulması için çağrılan metod
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
        /// İl bazında süreç ve alt süreçlerin kayıt edilmesini sağlar
        /// Kurum tarafından kayıt edilmek istenen süreçler olduğu için yapıldı. Silinebilir Melih 12.06.2023
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpGet]
        public async Task<IActionResult> SurecAltSurecIlBazindaKaydet()
        {
            string surecler = "İK.1.1\tİl Koordinatörlüğü\tProje Başvuru Yönetimi Birimi\tProje Başvuru Kayıt İşlemleri";
            surecler += "\nİK.1.2\tİl Koordinatörlüğü\tProje Başvuru Yönetimi Birimi\tTamlık(İdari) / Uygunluk Kontrolleri Öncesi İşlemler";
            surecler += "\nİK.1.3\tİl Koordinatörlüğü\tProje Başvuru Yönetimi Birimi\tProje Başvurularının Tamlık(İdari) / Uygunluk Kontrol İşlemleri";
            surecler += "\nİK.1.4\tİl Koordinatörlüğü\tProje Başvuru Yönetimi Birimi\tİş Planı Kapsamındaki İşlemler";
            surecler += "\nİK.1.5\tİl Koordinatörlüğü\tProje Başvuru Yönetimi Birimi\tBaşvurunun Uygun Kabul Edilmesi, Geri Çekilmesi ve Tamlık, Uygunluk veya İş Planı Analizi Sonucunda Reddedilmesine İlişkin İşlemler";
            surecler += "\nİK.2.1\tİl Koordinatörlüğü\tProje Başvuru Yönetimi Birimi\tSözleşmenin Hazırlanması ve İmzalatılması veya Reddedilmesi";
            surecler += "\nİK.2.2\tİl Koordinatörlüğü\tProje Başvuru Yönetimi Birimi\tZeyilname ve Fesih işlemleri";
            surecler += "\nİK.2.3\tİl Koordinatörlüğü\tProje Başvuru Yönetimi Birimi\tSözleşme ile İlgili Diğer İşlemler";
            surecler += "\nİK.3.1\tİl Koordinatörlüğü\tYerinde Kontrol Birimi\tSözleşme Öncesi Yerinde Kontrol İşlemleri";
            surecler += "\nİK.3.2\tİl Koordinatörlüğü\tYerinde Kontrol Birimi\tÖdeme Öncesi Yerinde Kontrol İşlemleri";
            surecler += "\nİK.3.3\tİl Koordinatörlüğü\tYerinde Kontrol Birimi\tUygulama Sonrası Yerinde Kontrol İşlemleri";
            surecler += "\nİK.3.4\tİl Koordinatörlüğü\tYerinde Kontrol Birimi\tİtirazlar Kapsamında Yapılan İşlemler";
            surecler += "\nİK.3.5\tİl Koordinatörlüğü\tYerinde Kontrol Birimi\tZeyilnameler Kapsamında Yapılan İşlemler";
            surecler += "\nİK.3.6\tİl Koordinatörlüğü\tYerinde Kontrol Birimi\tYerinde Kontrol Verilerinin Kaydedilmesi";
            surecler += "\nİK.4.1\tİl Koordinatörlüğü\tÖdeme Talep İşlemleri Birimi\tÖdeme Talep Paketi(ÖTP) İşlemleri";
            surecler += "\nİK.4.2\tİl Koordinatörlüğü\tÖdeme Talep İşlemleri Birimi\tTahakkuk İşlemleri";
            surecler += "\nİK.4.3\tİl Koordinatörlüğü\tÖdeme Talep İşlemleri Birimi\tÖTP ile ilgili yararlanıcıların bilgilendirilmesi";
            surecler += "\nİK.4.4\tİl Koordinatörlüğü\tÖdeme Talep İşlemleri Birimi\tUsulsüzlük İşlemlerinin Yönetimi";
            surecler += "\nİK.4.5\tİl Koordinatörlüğü\tÖdeme Talep İşlemleri Birimi\tProje Kaynaklı Geri Alım İşlemleri";
            surecler += "\nİK.5.1\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tMaaş, Harcırah ve Satınalmaya Bağlı Ödeme İşlemleri";
            surecler += "\nİK.5.2\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tİK Personel Özlük İşlemleri";
            surecler += "\nİK.5.3\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tİşçi Özlük İşlemleri";
            surecler += "\nİK.5.4\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tİdari İşler";
            surecler += "\nİK.5.5\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tGenel Evrak İşlemlerinin Yürütülmesi";
            surecler += "\nİK.5.6\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tİK'nın Diğer İş ve İşlemleri";
            surecler += "\nİK.5.7\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tBilgi Güvenliği Yönetim Sistemi İşlemleri";
            surecler += "\nİK.5.8\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tAğ, Sistem ve Güvenlik Yönetimi İşlemleri";
            surecler += "\nİK.5.9\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tBasın ve Medya Faaliyetleri";
            surecler += "\nİK.5.10\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tTanıtım ve Organizasyon Hizmetleri";
            surecler += "\nİK.5.11\tİl Koordinatörlüğü\tDestek Hizmetleri Birimi\tHalkla İlişkiler Hizmetleri";

            string altSurecler = "İK.1.1\tİK 1.1.1\tBaşvuru paketinin kontrol edilerek teslim alınması";
            altSurecler += "\nİK.1.1\tİK.1.1.2\tBaşvuru paketinin kaydının yapılması ve arşive teslim edilmesi";
            altSurecler += "\nİK.1.1\tİK.1.1.3\tBaşvuru için  “İl Başvuru Dosyası” ve “Arşiv Klasörü” oluşturulması";
            altSurecler += "\nİK.1.2\tİK.1.2.1\tBaşvuru izleme amaçlı veri tablolarının oluşturulması";
            altSurecler += "\nİK.1.2\tİK.1.2.2\tDanışman Firma ve Tedarikçi Veritabanı (DFTV) formunun doldurulması";
            altSurecler += "\nİK.1.2\tİK.1.2.3\tKopya ve manipülasyon olabilecek riskli başvuruların belirlenmesi ve ek kontrollerinin yapılması";
            altSurecler += "\nİK.1.2\tİK.1.2.4\tRiskli başvuruların yerinde kontrol birimine bildirilmesi";
            altSurecler += "\nİK.1.3\tİK.1.3.1\tBaşvuru paketlerinin tamlık kontrolünün idari kontrol listeleri üzerinden gerçekleştirilmesi";
            altSurecler += "\nİK.1.3\tİK.1.3.2\tBaşvuruların “Uygunluk Kontrol Listelerine” göre kontrolünün yapılması";
            altSurecler += "\nİK.1.3\tİK.1.3.3\tBeyan edilen harcamaların uygunluğunun kontrol edilmesi, gerektiğinde pazar araştırması yapılmasının talep edilmesi";
            altSurecler += "\nİK.1.3\tİK.1.3.4\tBaşvuru paketinde eksik belgesi olan ve/veya ek bilgi alınması gereken başvuru sahiplerinin bilgilendirilmesi ve eksikliklerin tamamlatılması";
            altSurecler += "\nİK.1.3\tİK.1.3.5\tBaşvuru sahipleri tarafından sunulan bilgi ve belgelerin kontrol edilmesi";
            altSurecler += "\nİK.1.3\tİK.1.3.6\tKontroller sırasında tespit edilen muhtemel şüpheli dolandırıcılıkların Hukuk Müşavirliğine bildirilmesi";
            altSurecler += "\nİK.1.3\tİK.1.3.7\tUygun başvuruların listesinin sözleşme öncesi yerinde kontrollerinin yapılabilmesi amacıyla yerinde kontrol birimine iletilmesi";
            altSurecler += "\nİK.1.3\tİK.1.3.8\tYKB'den iletilen ve yerinde kontrolü tamamlanan başvurular listesine göre Harcamaların Uygunluğu, Destek Seviyesi ve Miktarı Kontrol Listesinin (İK–1120) tamamlanması";
            altSurecler += "\nİK.1.4\tİK.1.4.1\tTüm başvurular için başvuru sahibinin finansman kaynaklarının değerlendirilmesi (mali analiz) işlemleri";
            altSurecler += "\nİK.1.4\tİK.1.4.2\tİş planı türünün B3 İş Planı olması durumunda İş Planı Analizinin yapılması";
            altSurecler += "\nİK.1.5\tİK.1.5.1\tUygun olan başvurular listesinin (İl-1125) hazırlanması ve Merkeze (PYK) gönderilmesi";
            altSurecler += "\nİK.1.5\tİK.1.5.2\tBaşvurunun geri çekilmesi veya reddedilmesine ilişkin işlemler";
            altSurecler += "\nİK.1.5\tİK.1.5.3\tRedde itiraz işlemlerinin değerlendirilmesi ve sonucun Merkeze bildirilmesi";
            altSurecler += "\nİK.1.5\tİK.1.5.4\tİşlemi tamamlanan başvuru paketlerinin ve il başvuru dosyalarının arşive teslim edilmesi";
            altSurecler += "\nİK.2.1\tİK.2.1.1\tSözleşmenin hazırlanması, imzalatılması ve arşive gönderilmek üzere dosyanın düzenlenmesi";
            altSurecler += "\nİK.2.1\tİK.2.1.2\t'Sözleşme Bildirim Dokümanı'nın oluşturulması ve başvuru sahibi ile İl Koordinatörlüklerindeki ilgili birimlere bildirilmesi";
            altSurecler += "\nİK.2.1\tİK.2.1.3\tSözleşme aşamasında reddedilen projeler ile ilgili işlemlerin yerine getirilmesi";
            altSurecler += "\nİK.2.2\tİK.2.2.1\tZeyilnamenin hazırlanması ve imzalatılması veya reddedilmesi ile arşive gönderilmek üzere dosyanın düzenlenmesi";
            altSurecler += "\nİK.2.2\tİK.2.2.2\tSözleşme değişikliği ile ilgili olarak diğer birimlere ve faydalanıcıya  bilgi verilmesi";
            altSurecler += "\nİK.2.2\tİK.2.2.3\tSözleşme feshi ile ilgili işlemlerin yerine getirilmesi";
            altSurecler += "\nİK.2.3\tİK.2.3.1\tİmzalanmış sözleşmeler, zeyilname ve fesih ile ilgili gerekli bilgilerin sisteme girilmesi";
            altSurecler += "\nİK.2.3\tİK.2.3.2\tSözleşme prosedürleri ile ilgili olarak başvuru sahibi/faydalanıcının itirazlarının değerlendirilmesi";
            altSurecler += "\nİK.3.1\tİK.3.1.1\tProjelerin risk puanlarının belirlenmesi ve sınıflandırılması";
            altSurecler += "\nİK.3.1\tİK.3.1.2\tHer bir başvuruya ilişkin teknik proje analizinin gerçekleştirilmesi";
            altSurecler += "\nİK.3.1\tİK.3.1.3\tSözleşme Öncesi Yerinde Kontrol İşlemlerinin Gerçekleştirilmesi";
            altSurecler += "\nİK.3.1\tİK.3.1.4\tTeknik proje değerlendirme sonuçları ve diğer ilgili/destekleyici dokümanların hazırlanması";
            altSurecler += "\nİK.3.1\tİK.3.1.5\tKontroller tamamlandıktan sonra, başvuru paketi ve il başvuru dosyasının arşive/e-arşive teslim edilmesi";
            altSurecler += "\nİK.3.1\tİK.3.1.6\tYerinde kontrolü tamamlanan başvurular listesinin PBYB'ye iletilmesi";
            altSurecler += "\nİK.3.2\tİK.3.2.1\tProjelerin risk puanlarının belirlenmesi ve sınıflandırılması";
            altSurecler += "\nİK.3.2\tİK.3.2.2\tAra dönem yerinde kontrol işlemlerinin gerçekleştirilmesi";
            altSurecler += "\nİK.3.2\tİK.3.2.3\tÖdeme öncesi yerinde kontrol işlemlerinin gerçekleştirilmesi";
            altSurecler += "\nİK.3.2\tİK.3.2.4\tYerinde kontrolü tamamlanan başvurular listesinin ÖTİB'e iletilmesi";
            altSurecler += "\nİK.3.3\tİK.3.3.1\tProjelerin risk puanlarının belirlenmesi ve sınıflandırılması";
            altSurecler += "\nİK.3.3\tİK.3.3.2\tYıllık olarak yenilenen sigorta poliçelerinin takibi";
            altSurecler += "\nİK.3.3\tİK.3.3.3\tUygulama sonrası yerinde kontrol işlemlerinin gerçekleştirilmesi";
            altSurecler += "\nİK.3.4\tİK.3.4.1\tBaşvurunun reddine veya taslak sözleşmeye itiraz işlemleri kapsamında, ilgisi nedeniyle talep edilmesi durumunda; “İtiraz Değerlendirme Komisyonu Raporu ve Tutanağı”nın hazırlanması";
            altSurecler += "\nİK.3.5\tİK.3.5.1\tPBYB iş ve işlemlerinde kullanılmak üzere; teknik değişiklik içeren büyük değişiklik kapsamındaki talepleri değerlendirme amaçlı 1225-A/B formlarının ve görüş yazılarının hazırlanması";
            altSurecler += "\nİK.3.6\tİK.3.6.1\tYerinde kontrol verilerinin kaydedilmesi";
            altSurecler += "\nİK.4.1\tİK.4.1.1\tÖdeme Talep Paketinin (ÖTP) kayıt işlemlerinin gerçekleştirilmesi";
            altSurecler += "\nİK.4.1\tİK.4.1.2\tÖTP'ler üzerinde İK İdari (Tamlık) Kontrol İşlemleri";
            altSurecler += "\nİK.4.1\tİK.4.1.3\tÖTP'ler üzerinde İK İdari (Uygunluk) Kontrol İşlemleri";
            altSurecler += "\nİK.4.1\tİK.4.1.4\tKontrol edilen ÖTP'nin yerinde kontrollerinin yapılması için YKB'ye gönderilmesi";
            altSurecler += "\nİK.4.2\tİK.4.2.1\tTahakkuk tutarının hesaplanması";
            altSurecler += "\nİK.4.2\tİK.4.2.2\tYerinde Kontrol Birimi tarafından hazırlanan rapora dayanarak 'Tahakkuk Formu'nun hazırlanması";
            altSurecler += "\nİK.4.2\tİK.4.2.3\tTahakkuka ilişkin belgelerin kontrolünün yapılması";
            altSurecler += "\nİK.4.2\tİK.4.2.4\tGerektiğinde ek tahakkuk işlemlerinin gerçekleştirilmesi";
            altSurecler += "\nİK.4.2\tİK.4.2.5\tİlgili Koordinatörlüklere tahakkuka ilişkin bildirim mektuplarının hazırlanamsı";
            altSurecler += "\nİK.4.3\tİK.4.3.1\tÖTP'nin içeriği ve sözleşmede belirtilen teslim zamanı ile ilgili olarak yararlanıcılara tebligat gönderilmesi";
            altSurecler += "\nİK.4.3\tİK.4.3.2\tYararlanıcıya gönderilmek üzere ÖTP'nin kabulüne ilişkin bildirim mektubunun hazırlanması ve gönderilmesi";
            altSurecler += "\nİK.4.3\tİK.4.3.3\tÖTP'nin tamlık kontrollerinde herhangi bir eksikliğin belirlenmesi durumunda gerekli dokümanların sağlanması için yararlanıcıya bildirim mektubunun gönderilmesi";
            altSurecler += "\nİK.4.3\tİK.4.3.4\tÖTP'nin uygunluk kontrollerinde herhangi bir eksikliğin belirlenmesi durumunda gerekli bilgilerin sağlanması için yararlanıcıya bildirim mektubunun gönderilmesi";
            altSurecler += "\nİK.4.4\tİK.4.4.1\tUsulsüzlük bildirimlerinin Hukuk Müşavirliğine iletilmesi ";
            altSurecler += "\nİK.4.4\tİK.4.4.2\tŞüpheli dolandırıcılık işlemlerinin Hukuk Müşavirliğine bildirilmesi";
            altSurecler += "\nİK.4.4\tİK.4.4.3\tİdari hata bildirimlerinin kaydedilmesi ve Hukuk Müşavirliğine bildirilmesi";
            altSurecler += "\nİK.4.4\tİK.4.4.4\tUsulsüzlük şüphesi içeren şikayet ve ihbarların değerlendirilmesi, ilgili Merkez Birimlerine bildirilmesi";
            altSurecler += "\nİK.4.5\tİK.4.5.1\tBorç bildirim işlemleri";
            altSurecler += "\nİK.4.5\tİK.4.5.2\tİcra ödeme emri işlemleri";
            altSurecler += "\nİK.4.5\tİK.4.5.3\tHaciz İşlemlerinin (Tapu, Trafik, Banka Haciz Bildirileri ve Fiili Haciz İşlemleri) yapılması";
            altSurecler += "\nİK.4.5\tİK.4.5.4\t6183 sayılı AATUHK kapsamında geri alım sürecinin tamamlanmasına yönelik işlemlerin yapılması";
            altSurecler += "\nİK.5.1\tİK.5.1.1\tPersonel maaşlarının hesaplanması ve kontrolü";
            altSurecler += "\nİK.5.1\tİK.5.1.2\tMaaş ödeme işlemlerinin yapılması";
            altSurecler += "\nİK.5.1\tİK.5.1.3\tHarcırah ödeme işlemlerinin yapılması";
            altSurecler += "\nİK.5.1\tİK.5.1.4\tSatınalmaya bağlı ödeme işlemlerinin yapılması";
            altSurecler += "\nİK.5.1\tİK.5.1.5\tMal ve hizmet alımlarıyla ilgili doğrudan temin süreçlerinin yönetilmesi";
            altSurecler += "\nİK.5.1\tİK.5.1.6\tİK'nin elektrik, su, doğalgaz, posta pulu, kargo hizmeti ihtiyaçlarının karşılanması";
            altSurecler += "\nİK.5.1\tİK.5.1.7\tBeyannamelerin süresi içinde bildirimlerinin yapılarak ödemelerin gerçekleştirilmesi ";
            altSurecler += "\nİK.5.1\tİK.5.1.8\tİK hizmet binasının kirasının ödenmesi";
            altSurecler += "\nİK.5.2\tİK.5.2.1\tPersonel özlük dosyası içeriğindeki evrakın düzenlenmesi ve takip edilmesi (Atama, izin, geçici görevlendirme, yer değişiklikleri, iş tanımları, mal beyanları vb.)";
            altSurecler += "\nİK.5.2\tİK.5.2.2\tÖzlük işlemlerinin 'Kurumsal Bilgi Sistemi - KBS üzerinden takip edilmesi";
            altSurecler += "\nİK.5.2\tİK.5.2.3\tDisiplin işlemlerinin takip edilmesi";
            altSurecler += "\nİK.5.2\tİK.5.2.4\tPersonel performans değerlendirme işlemlerinin (Ödül, başarı belgesi vb.) gerçekleştirilmesi";
            altSurecler += "\nİK.5.2\tİK.5.2.5\tSağlık raporlarının SGK sistemine girişi ve gerekli işlemlerin takip edilmesi";
            altSurecler += "\nİK.5.3\tİK.5.3.1\t696 sayılı KHK ile kadroya alınan sürekli işçilerin mali ve sosyal haklarının takip edilmesi";
            altSurecler += "\nİK.5.3\tİK.5.3.2\t696 sayılı KHK ile kadroya alınan sürekli işçilerin disiplin işlemlerinin takip edilmesi";
            altSurecler += "\nİK.5.3\tİK.5.3.3\t696 sayılı KHK ile kadroya alınan sürekli işçiler hakkındaki icra takip işlemlerini yürütülmesi";
            altSurecler += "\nİK.5.3\tİK.5.3.4\tKurumdan ayrılan işçi personelin tazminat iş ve işlemlerinin yürütülmesi";
            altSurecler += "\nİK.5.4\tİK.5.4.1\tSürekli işçilerin (Temizlik ve kat görevlileri, özel güvenlik görevlileri, şoförler, yemekhane personeli, teknik görevliler vb.) sevk ve idaresi";
            altSurecler += "\nİK.5.4\tİK.5.4.2\tHizmet araçlarının sevk ve idaresinin gerçekleştirilmesi";
            altSurecler += "\nİK.5.4\tİK.5.4.3\tİK'nin taşınır ve ambar işlemlerinin gerçekleştirilmesi";
            altSurecler += "\nİK.5.4\tİK.5.4.4\tHizmet araçlarının ve jeneratörlerin akaryakıt sarfiyatının kontrol edilmesi";
            altSurecler += "\nİK.5.4\tİK.5.4.5\tHizmet binasının bakım onarım işlerinin yürütülmesi";
            altSurecler += "\nİK.5.4\tİK.5.4.6\tSivil savunma işlemlerinin yürütülmesi";
            altSurecler += "\nİK.5.5\tİK.5.5.1\tGenel Evrak / Genel Evrak Arşivi / İlgili Birimlere evrak havalesi ile ilgili iş ve işlemlerin Koordinasyonunun gerçekleştirilmesi";
            altSurecler += "\nİK.5.5\tİK.5.5.2\tPostaya düşen evrakın gönderim işlemlerinin yapılması";
            altSurecler += "\nİK.5.5\tİK.5.5.3\tİK (Genel Evrak) Arşivinin ve Arşiv malzemelerinin düzen ve fiziksel koşullarının yönetilmesi";
            altSurecler += "\nİK.5.6\tİK.5.6.1\tProje dosyalarının muhafaza edilmesi ve erişimin sağlanması";
            altSurecler += "\nİK.5.6\tİK.5.6.2\tDosyaların taranarak E-Arşiv sistemine kaydedilmesi";
            altSurecler += "\nİK.5.6\tİK.5.6.3\tİdari davalara ilişkin kişi borç veya alacaklarına yönelik mali hakediş tablolarının oluşturulması ile tahakkuk ve ödeme işlemlerinin gerçekleştirilmesi";
            altSurecler += "\nİK.5.6\tİK.5.6.4\tİK personeline yönelik eğitim faaliyetlerinin takip edilemsi";
            altSurecler += "\nİK.5.7\tİK.5.7.1\t6698 Sayılı Kişisel Verilerin Korunması Kanunu gerekliliklerinin sağlanması";
            altSurecler += "\nİK.5.7\tİK.5.7.2\tISO 27001 Bilgi Güvenliği Yönetim Sistemi Standartlarına yönelik faaliyetlerin gerçekleştirilmesi ve gerekliliklerinin sağlanması";
            altSurecler += "\nİK.5.8\tİK.5.8.1\tİK fiziksel bilişim altyapısının kurulması, yönetilmesi ve güvenliğinin sağlanması";
            altSurecler += "\nİK.5.8\tİK.5.8.2\tE-posta, dosya sunucuları, kullanıcı hesapları, güvenlik kameraları, telefon santrali, sistem odası gibi teknik hizmetlerin yürütülmesi";
            altSurecler += "\nİK.5.9\tİK.5.9.1\tSosyal medya hesaplarının yönetilmesi";
            altSurecler += "\nİK.5.9\tİK.5.9.2\tİK basın faaliyetlerinin yürütülmesi ve takip edilmesi";
            altSurecler += "\nİK.5.9\tİK.5.9.3\tTanıtım ve bilgilendirme amaçlı film vb. görsel yayınların hazırlanması";
            altSurecler += "\nİK.5.10\tİK.5.10.1\tKurumsal organizasyonların (Fuar, Organizasyon, Lansman vb.) gerçekleştirilmesi";
            altSurecler += "\nİK.5.10\tİK.5.10.2\tKurumsal tanıtım faaliyetlerinin planlanması ve gerçekleştirilmesi";
            altSurecler += "\nİK.5.11\tİK.5.11.1\tDilekçe ile  gelen başvuruların yönetilmesi";
            altSurecler += "\nİK.5.11\tİK.5.11.2\tYardım Masası Telefon Hattına (444 85 35) ve Yardım Masası Sistemine gelen başvuruların yönetilmesi ve takip edilmesi";


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
