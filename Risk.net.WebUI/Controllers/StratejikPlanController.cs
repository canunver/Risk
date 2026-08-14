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
using System.Security.Claims;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// StratejikPlan işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class StratejikPlanController : GenelController
    {
        /// <summary>
        /// IStratejikPlanService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanService _service;
        /// <summary>
        /// IStratejikPlanHedefService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanHedefService _serviceHedef;
        /// <summary>
        /// IStratejikPlanHedefGostergeService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanHedefGostergeService _serviceHedefGosterge;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.StratejikPlanController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceHedef"></param>
        /// <param name="serviceHedefGosterge"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanController(IStratejikPlanService service,
                                        IStratejikPlanHedefService serviceHedef,
                                        IStratejikPlanHedefGostergeService serviceHedefGosterge,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceHedef = serviceHedef;
            _serviceHedefGosterge = serviceHedefGosterge;
        }

        /// <summary>
        /// StratejikPlan View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "*")]
        public IActionResult Index()
        {
            ViewBag.KoordinatorlukKodu = _kullanan.KoordinatorlukKod;
            ViewBag.BirimKodu = _kullanan.BirimKod;

            if (Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI", _kullanan))
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
        public async Task<ActionResult> Kaydet([FromBody] StratejikPlan form)
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
            StratejikPlan form = new StratejikPlan();
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
            StratejikPlan form = new StratejikPlan();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Amac seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <param name="koordinatorlukKod"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer(string koordinatorlukKod, int baslamaYil)
        {
            //Koordinatorlüğe göre listelenmesin
            //var sonuc = await _service.ListeleAsync(_kullanan, (int)ENUMDurum.Aktif);
            //var sonuc = await _service.ListeleAsync(_kullanan, koordinatorlukKod, (int)ENUMDurum.Aktif);

            var sonuc = new Sonuc();
            if (baslamaYil > 0)
                sonuc = await _service.ListeleAsync(_kullanan, (int)ENUMDurum.Aktif, baslamaYil);
            else
                sonuc = await _service.ListeleAsync(_kullanan, (int)ENUMDurum.Aktif);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (StratejikPlan item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.AmacNo + " - " + item.Amac });
            }

            return Ok(donenDeger);
        }


        /// <summary>
        /// Ekranlardaki Amac seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerAmac()
        {
            var sonuc = await _service.ListeleAsync(_kullanan, (int)ENUMDurum.Aktif);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (StratejikPlan item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.AmacNo + " - " + item.Amac });
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
        public async Task<IActionResult> StratejikPlanHedefKayitGetir(string kod)
        {
            Sonuc sonuc = await _serviceHedef.KayitGetirAsync(_kullanan, kod);

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
        public async Task<ActionResult> StratejikPlanHedefKaydet(StratejikPlanHedef form)
        {
            form.Durum = (int)ENUMDurum.Aktif;
            Sonuc sonuc = await _serviceHedef.KaydetAsync(_kullanan, form);

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
        public async Task<IActionResult> StratejikPlanHedefSil(string kod)
        {
            StratejikPlanHedef form = new StratejikPlanHedef();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _serviceHedef.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Hedef seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerHedef(string stratejikPlanKod)
        {
            var sonuc = await _serviceHedef.ListeleAsync(_kullanan, stratejikPlanKod, (int)ENUMDurum.Aktif);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (StratejikPlanHedef item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.HedefNo + " - " + item.Adi });
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
        public async Task<IActionResult> StratejikPlanGostergeKayitGetir(string kod)
        {
            Sonuc sonuc = await _serviceHedefGosterge.KayitGetirAsync(_kullanan, kod);

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
        public async Task<ActionResult> StratejikPlanHedefGostergeKaydet(StratejikPlanHedefGosterge form)
        {
            form.Durum = (int)ENUMDurum.Aktif;
            Sonuc sonuc = await _serviceHedefGosterge.KaydetAsync(_kullanan, form);

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
        public async Task<IActionResult> StratejikPlanHedefGostergeSil(string kod)
        {
            StratejikPlanHedefGosterge form = new StratejikPlanHedefGosterge();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Pasif;

            Sonuc sonuc = await _serviceHedefGosterge.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Gosterge seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerHedefGosterge(string stratejikPlanHedefKod, string eylemMi)
        {
            var sonuc = await _serviceHedefGosterge.ListeleAsync(_kullanan, stratejikPlanHedefKod, (int)ENUMDurum.Aktif);

            List<StratejikPlanHedefGosterge> donenDeger = new List<StratejikPlanHedefGosterge>();

            foreach (StratejikPlanHedefGosterge item in sonuc.Liste)
            {
                if (eylemMi == "1")
                {

                    double yil1 = 0, yil2 = 0, yil3 = 0, yil4 = 0, yil5 = 0;

                    foreach (StratejikPlanIzlemeDonem d in item.Donemler)
                    {
                        if (d.Donem == 1)
                            yil1 = d.GerceklesenDegerYilSonu;
                        else if (d.Donem == 2)
                            yil2 = d.GerceklesenDegerYilSonu;
                        else if (d.Donem == 3)
                            yil3 = d.GerceklesenDegerYilSonu;
                        else if (d.Donem == 4)
                            yil4 = d.GerceklesenDegerYilSonu;
                        else if (d.Donem == 5)
                            yil5 = d.GerceklesenDegerYilSonu;
                    }

                    item.YilDegeri1 = yil1;
                    item.YilDegeri2 = yil2;
                    item.YilDegeri3 = yil3;
                    item.YilDegeri4 = yil4;
                    item.YilDegeri5 = yil5;
                };

                donenDeger.Add(item);
            }




            return Ok(donenDeger);
        }

        /// <summary>
        /// Kullanıcı tarafından girilen bilgileri ilgili servise kayıt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> StratejikPlanHedefBirimKaydet(StratejikPlanHedef form)
        {
            form.Durum = (int)ENUMDurum.Aktif;
            Sonuc sonuc = await _serviceHedef.KaydetBirimAsync(_kullanan, form);

            return Ok(sonuc);
        }
    }
}
