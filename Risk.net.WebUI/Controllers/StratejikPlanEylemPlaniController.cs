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
    /// Stratejik Plan Ýzleme iþlemlerinin yapýldýðý sayfa, <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayý görme yetkisi olan kullanýcýlar belirlenir
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class StratejikPlanEylemPlaniController : GenelController
    {
        /// <summary>
        /// IStratejikPlanEylemPlaniService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanEylemPlaniService _service;


        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.StratejikPlanIzlemeController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceGostege"></param>
        /// <param name="serviceDonem"></param>
        /// <param name="serviceHedefGosterge"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanEylemPlaniController(IStratejikPlanEylemPlaniService service,
                                    IHttpContextAccessor httpContextAccessor,
                                    IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }


        /// <summary>
        /// StratejikPlanIzleme View sayfasý açýldýðýnda çalýþan metod.
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

            return View();
        }

        /// <summary>
        /// Kullanýcý tarafýndan girilen bilgileri ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<ActionResult> Kaydet(StratejikPlanHedef form)
        {
            Sonuc sonuc = await _service.KaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn tüm bilgisini ilgili servis aracýlýðýyla getiren metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Listele(string hedefKod)
        {
            Sonuc sonuc = await _service.ListeleAsync(_kullanan, hedefKod);

            return Ok(sonuc);
        }


        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerTamamlanmaDurumu()
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            donenDeger.Add(new SelectListesi { id = (int)EnumStratejikPlanEylemPlaniTamamlanmaDurumu.DevamEdiyor + "", text = _sharedResource["StratejikPlanEylemPlani.TamamlanmaDurumu.DevamEdiyor"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumStratejikPlanEylemPlaniTamamlanmaDurumu.Ertelendi + "", text = _sharedResource["StratejikPlanEylemPlani.TamamlanmaDurumu.Ertelendi"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumStratejikPlanEylemPlaniTamamlanmaDurumu.IptalEdildi + "", text = _sharedResource["StratejikPlanEylemPlani.TamamlanmaDurumu.IptalEdildi"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumStratejikPlanEylemPlaniTamamlanmaDurumu.Tamamlandi + "", text = _sharedResource["StratejikPlanEylemPlani.TamamlanmaDurumu.Tamamlandi"] });

            return Ok(donenDeger);
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
        public async Task<IActionResult> EylemPlaniSil(string kod)
        {
            Sonuc sonuc = await _service.SilAsync(_kullanan, new StratejikPlanEylemPlani() { Kod = kod });

            return Ok(sonuc);
        }
    }
}
