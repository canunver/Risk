using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
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
    [Authorize]
    public class ViewComponentController : GenelController
    {
        /// <summary>
        /// IYardimService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>        
        private readonly IYardimService _serviceYardim;
        /// <summary>
        /// IPersonelAktifRolService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>        
        private readonly IPersonelAktifRolService _serviceRol;
        /// <summary>
        /// IDenetimGorevlendirmeService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>        
        private readonly IDenetimGorevlendirmeService _serviceDenetimGorevlendirme;
        /// <summary>
        /// IMemoryCache servisine ulaþmak için kullanýlan deðiþken, Kullanýcýnýn yazýlýmda kullanmakta olduðu rolü saklamak için
        /// </summary>
        /// <remarks></remarks>        
        private readonly IMemoryCache _cache;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.ViewComponentController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="serviceYardim"></param>
        /// <param name="serviceRol"></param>
        /// <param name="cache"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public ViewComponentController(IYardimService serviceYardim,
                                    IPersonelAktifRolService serviceRol,
                                    IDenetimGorevlendirmeService serviceDenetimGorevlendirme,
                                    IMemoryCache cache,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _serviceYardim = serviceYardim;
            _serviceDenetimGorevlendirme = serviceDenetimGorevlendirme;
            _serviceRol = serviceRol;
            _cache = cache;
        }

        //https://visualstudiomagazine.com/articles/2018/02/01/invoking-view-components.aspx
        //https://quizdeveloper.com/tips/aspdotnet-core-create-and-invoke-view-component-with-example-aid72
        //public IActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        /// KontrolKriterMatrisiGoster ViewComponent sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [HttpGet]
        public IActionResult KontrolKriterMatrisiGoster()
        {
            return ViewComponent("KontrolKriterMatrisi");
        }

        /// <summary>
        /// EtkiOlasilikMatrisiGoster ViewComponent sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [HttpGet]
        public IActionResult EtkiOlasilikMatrisiGoster()
        {
            return ViewComponent("EtkiOlasilikMatrisi");
        }

        /// <summary>
        /// TarihceGoster ViewComponent sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [HttpGet]
        public IActionResult TarihceGoster(string ilgiKod)
        {
            return ViewComponent("Tarihce", new { ilgiKod = ilgiKod });
        }

        /// <summary>
        /// AktifRolSecGoster ViewComponent sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [HttpGet]
        public IActionResult AktifRolSecGoster()
        {
            return ViewComponent("AktifRolSec");
        }

        /// <summary>
        /// Kullanýcýnýn AktifRol ViewComponentinden seçerek kullanmak istediði rolü
        /// daha sonra yazýlým açýldýðýnda tekrar kullanmak için ilgili servis aracýlýðý ile kayýt eden metod
        /// </summary>
        /// <param name="rolAdi"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> AktifRolKaydet(string rolAdi, string koordinatorlukKod, string birimKod)
        {
            PersonelAktifRol aktifRol = new PersonelAktifRol();
            aktifRol.PersonelKod = _kullanan.PersonelKod;
            aktifRol.Rol = rolAdi;
            aktifRol.KoordinatorlukKod = koordinatorlukKod;
            aktifRol.BirimKod = birimKod;

            Sonuc sonuc = await _serviceRol.KaydetAsync(_kullanan, aktifRol);

            var identity = (ClaimsIdentity)User.Identity;

            string cacheId = identity.FindFirst(ClaimTypes.Name).Value;

            _cache.Remove(cacheId);

            return Ok(sonuc);
        }

        /// <summary>
        /// YardimGoster ViewComponent sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [HttpGet]
        public IActionResult YardimGoster(string sayfaAdi)
        {
            return ViewComponent("Yardim", new { sayfaAdi = sayfaAdi });
        }

        /// <summary>
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> YardimKaydet(Yardim form)
        {
            Sonuc sonuc = await _serviceYardim.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// DosyaKontrol ViewComponent sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [HttpGet]
        public IActionResult DosyaKontrolGoster(string form)
        {
            return ViewComponent("DosyaKontrol", form);
        }

        /// <summary>
        /// DenetimGorevlendirmeGoster ViewComponent sayfasý açýldýðýnda çalýþan metod
        /// </summary>
        [HttpGet]
        public IActionResult DenetimGorevlendirmeGoster(string denetimKodu, int tip)
        {
            if (!Arac.YetkisiVarmi("ICDENETIMKOORDINATOR,ICDENETIMUZMANI", _kullanan))
            {

                Sonuc sonuc = new Sonuc(ENUMIslemDurum.Uyari, "<li>" + _sharedResource["Kontrol.Duzenle.DenetimGorevlendirmeTipAlaniBos"] + "</li>");
                return Ok(sonuc);
            }
            else
                return ViewComponent("DenetimGorevlendirme", new { denetimKodu = denetimKodu, tip = tip });
        }

        /// <summary>
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> DenetimGorevlendirmeKaydet(DenetimGorevlendirme form)
        {
            Sonuc sonuc = await _serviceDenetimGorevlendirme.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servisden getirilmesi için
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> DenetimGorevlendirmeGetir(DenetimGorevlendirme form)
        {
            Sonuc sonuc = await _serviceDenetimGorevlendirme.KayitGetirAsync(_kullanan, form);

            return Ok(sonuc);
        }

    }
}
