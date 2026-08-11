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
    /// Koordinatorluk iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    public class TanimKoordinatorlukController : GenelController
    {
        /// <summary>
        /// IViewKoordinatorlukService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewKoordinatorlukService _service;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTE;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.TanimKoordinatorlukController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceCTE"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TanimKoordinatorlukController(IViewKoordinatorlukService service,
                                            ICTEKoordinatorlukService serviceCTE,
                                            IHttpContextAccessor httpContextAccessor,
                                            IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceCTE = serviceCTE;
        }

        /// <summary>
        /// TanimKoordinatorluk View sayfasý açýldýðýnda çalýþan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayý görme yetkisi olan kullanýcýlar belirlenir
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
        /// Ekranlardaki Koordinatorluk seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer(bool yetki)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();
            List<string> gorebilecekleri = new List<string>();
            if (yetki)
            {
                //Bütün Ýl koordinatörlüklerde iþlem yapabilirler
                if (Arac.YetkisiVarmi("RISKSEKRETARYASI,PLANLAMAUNITESI,YETKILIRISKGOREVLISI,BASKAN,ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI", _kullanan))
                    yetki = false;
                else if (Arac.YetkisiVarmi("GENELKOORDINATOR", _kullanan))//Kendi ve altýndaki koordinatörlüklerde iþlem yapabilirler
                {
                    gorebilecekleri.Add(_kullanan.KoordinatorlukKod);

                    CTEKoordinatorluk kriter = new CTEKoordinatorluk();
                    kriter.BagliKod = _kullanan.KoordinatorlukKod;
                    Sonuc sonucListe = await _serviceCTE.ListeleAsync(_kullanan, kriter);
                    foreach (CTEKoordinatorluk item in sonucListe.Liste)
                    {
                        gorebilecekleri.Add(item.Kod);
                    }
                }
                else
                {
                    //Sadece kendi koordinatorlüklerinde iþlem yapabilirler
                    //MERKEZKOORDINATOR,ILKOORDINATOR,BIRIMAMIRI,UZMAN
                    gorebilecekleri.Add(_kullanan.KoordinatorlukKod);
                }
            }

            var sonuc = await _service.ListeleAsync(_kullanan);

            foreach (ViewKoordinatorluk item in sonuc.Liste)
            {
                if (item.Kod == "9999") continue;


                if (yetki)
                {
                    bool goster = false;
                    foreach (var yk in gorebilecekleri)
                    {
                        if (yk == item.Kod) goster = true;
                    }
                    if (!goster) continue;
                }

                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Adi });
            }

            return Ok(donenDeger);
        }

        /// <summary>
        /// Ekranlardaki Koordinatorluk seçim kutularýnýn doldurulmasý için çaðrýlan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerTumu(bool yetki)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();
            List<string> gorebilecekleri = new List<string>();
            if (yetki)
            {
                //Bütün Ýl koordinatörlüklerde iþlem yapabilirler
                if (Arac.YetkisiVarmi("RISKSEKRETARYASI,PLANLAMAUNITESI,YETKILIRISKGOREVLISI,BASKAN,ICDENETIMUZMANI,ICDENETIMKOORDINATOR,SISTEMYONETICISI", _kullanan))
                    yetki = false;
                else if (Arac.YetkisiVarmi("GENELKOORDINATOR", _kullanan))//Kendi ve altýndaki koordinatörlüklerde iþlem yapabilirler
                {
                    gorebilecekleri.Add(_kullanan.KoordinatorlukKod);

                    CTEKoordinatorluk kriter = new CTEKoordinatorluk();
                    kriter.BagliKod = _kullanan.KoordinatorlukKod;
                    Sonuc sonucListe = await _serviceCTE.ListeleAsync(_kullanan, kriter);
                    foreach (CTEKoordinatorluk item in sonucListe.Liste)
                    {
                        gorebilecekleri.Add(item.Kod);
                    }
                }
                else
                {
                    //Sadece kendi koordinatorlüklerinde iþlem yapabilirler
                    //MERKEZKOORDINATOR,ILKOORDINATOR,BIRIMAMIRI,UZMAN
                    gorebilecekleri.Add(_kullanan.KoordinatorlukKod);
                }
            }

            var sonuc = await _service.ListeleAsync(_kullanan);

            if (sonuc.Liste.Count > 0)
            { 
                donenDeger.Add(new SelectListesi { id = "-1", text = "Tüm Koordinatörlükler" });
                donenDeger.Add(new SelectListesi { id = "-42", text = "42 Ýl Koordinatörlüðü" });
            }

            foreach (ViewKoordinatorluk item in sonuc.Liste)
            {
                if (item.Kod == "9999") continue;


                if (yetki)
                {
                    bool goster = false;
                    foreach (var yk in gorebilecekleri)
                    {
                        if (yk == item.Kod) goster = true;
                    }
                    if (!goster) continue;
                }

                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.Adi });
            }

            return Ok(donenDeger);
        }

    }
}
