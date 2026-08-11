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
    /// Birim iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    public class TanimBirimController : GenelController
    {
        /// <summary>
        /// IViewBirimService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBirimService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.TanimBirimController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TanimBirimController(IViewBirimService service,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// TanimBirim View sayfasý açýldýðýnda çalýþan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayý görme yetkisi olan kullanýcýlar belirlenir
        /// </summary>
        [YetkiKontrol(Yetkiler = "SISTEMYONETICISI")]
        public IActionResult Index()
        {
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
        /// Ekranlardaki Birim seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer(string koordinatorlukKod, bool yetki)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            if (yetki)
            {
                //Sadece kendi birimlerinde iþlem yapabilirler
                if (Arac.YetkisiVarmi("BIRIMAMIRI,UZMAN", _kullanan))
                {

                }
                else
                {
                    //Bütün birimlerde iþlem yapabilirler
                    yetki = false;
                }

            }

            var sonuc = await _service.ListeleAsync(_kullanan, koordinatorlukKod);

            foreach (ViewBirim item in sonuc.Liste)
            {
                if (yetki && _kullanan.BirimKod != item.Kod) continue;

                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Adi });
            }

            return Ok(donenDeger);
        }
    }
}
