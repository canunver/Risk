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
    /// Stratejik Plan İzleme işlemlerinin yapıldığı sayfa, <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class StratejikPlanIzlemeController : GenelController
    {
        /// <summary>
        /// IStratejikPlanIzlemeService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanIzlemeService _service;
        /// <summary>
        /// IStratejikPlanHedefGostergeService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanHedefGostergeService _serviceGostege;
        /// <summary>
        /// IStratejikPlanIzlemeGostergeDonemService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanIzlemeDonemService _serviceDonem;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.StratejikPlanIzlemeController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceGostege"></param>
        /// <param name="serviceDonem"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanIzlemeController(IStratejikPlanIzlemeService service,
                                    IStratejikPlanHedefGostergeService serviceGostege,
                                    IStratejikPlanIzlemeDonemService serviceDonem,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceGostege = serviceGostege;
            _serviceDonem = serviceDonem;
        }


        /// <summary>
        /// StratejikPlanIzleme View sayfası açıldığında çalışan metod.
        /// </summary>
        public IActionResult Index()
        {
            if (Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI", _kullanan))
                ViewBag.KayitYetki = true;
            else
                ViewBag.KayitYetki = false;

            if (Arac.YetkisiVarmi("BIRIMAMIRI", _kullanan))
                ViewBag.DegerGirisYetki = true;
            else
                ViewBag.DegerGirisYetki = false;

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
        public async Task<ActionResult> Kaydet(StratejikPlanHedefGosterge form)
        {
            Sonuc sonuc = await _serviceGostege.KaydetIzlemeAsync(_kullanan, form);

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
        public async Task<IActionResult> DonemSil(string kod)
        {
            Sonuc sonuc = await _serviceDonem.SilAsync(_kullanan, new StratejikPlanIzlemeDonem() { Kod = kod });

            return Ok(sonuc);
        }
    }
}
