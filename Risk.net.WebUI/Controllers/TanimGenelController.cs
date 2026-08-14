using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Services;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using Risk.net.WebUI.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Genel tanımlama işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class TanimGenelController : GenelController
    {
        /// <summary>
        /// ITanimGenelService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITanimGenelService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.TanimGenelController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TanimGenelController(ITanimGenelService service,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// TanimGenel View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "SISTEMYONETICISI")]
        public IActionResult Index()
        {
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
        public async Task<ActionResult> Kaydet(TanimGenel form)
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
            Sonuc sonuc = await _service.SilAsync(_kullanan, kod);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Genel Tanımlamalar seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer(string tur)
        {
            var sonuc = await _service.ListeleAsync(_kullanan, tur);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (TanimGenel item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Adi });
            }

            return Ok(donenDeger);
        }
    }
}
