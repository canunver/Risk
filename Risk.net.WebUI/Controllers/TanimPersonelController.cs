using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Services;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Risk.net.Utilities.Objects;
using Risk.net.WebUI.Classes;
using Microsoft.AspNetCore.Http;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Personel işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class TanimPersonelController : GenelController
    {
        /// <summary>
        /// IViewPersonelService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewPersonelService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.TanimPersonelController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TanimPersonelController(IViewPersonelService service,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// TanimPersonel View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
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
        /// Ekranlardaki Personel seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer(string koordinatorlukKod, string birimKod, string listeyeEklenecekPersonelkod = "")
        {
            var sonuc = await _service.ListeleAsync(_kullanan, koordinatorlukKod, birimKod);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (ViewPersonel item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.AdiSoyadi + " - " + item.UnvanAdi });
            }

            if (!string.IsNullOrWhiteSpace(listeyeEklenecekPersonelkod))
            {
                bool personelVar = false;
                foreach (ViewPersonel p in sonuc.Liste)
                {
                    if (p.Kod == listeyeEklenecekPersonelkod)
                        personelVar = true;
                }

                if (!personelVar)
                {
                    var sonuc2 = await _service.KayitGetirAsync(_kullanan, listeyeEklenecekPersonelkod);
                    if (sonuc2.IslemSonuc)
                    {
                        var per = (ViewPersonel)sonuc2.Nesne;
                        donenDeger.Add(new SelectListesi { id = per.Kod, text = per.AdiSoyadi });
                    }
                }
            }

            return Ok(donenDeger);
        }

        /// <summary>
        /// Parametre olarak verilen kod ile uyuşan personelin unvan bilgisini veren metod
        /// </summary>
        /// <param name="personelKod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> PersonelUnvaniVer(string personelKod)
        {
            var sonuc = await _service.KayitGetirAsync(_kullanan, personelKod);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            ViewUnvan unvan = new ViewUnvan();

            if (sonuc.IslemSonuc)
            {
                unvan.Adi = ((ViewPersonel)sonuc.Nesne).UnvanAdi;
                unvan.Kod = ((ViewPersonel)sonuc.Nesne).UnvanKod;
            }

            sonuc.Nesne = unvan;

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki İç Kontrol Koordinatörlüğüne ait Personel seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        //[HttpPost]
        //public async Task<IActionResult> SelectListesiVerIcKontrol()
        //{
        //    string koordinatorlukKod = Arac.ConfigOku("Genel:IcDenetimKoordinatorlukKod");
        //    string birimKod = "";

        //    return await SelectListesiVer(koordinatorlukKod, birimKod);
        //}
    }
}
