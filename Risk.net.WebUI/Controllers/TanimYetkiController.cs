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
using Microsoft.AspNetCore.Http;
using Risk.net.WebUI.Classes;
using System.Linq;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Yetki işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    public class TanimYetkiController : GenelController
    {
        /// <summary>
        /// IViewYetkiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewYetkiService _service;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.TanimYetkiController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TanimYetkiController(IViewYetkiService service,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
        }

        /// <summary>
        /// TanimYetki View sayfası açıldığında çalışan metod. <see cref="Risk.net.WebUI.Classes.YetkiKontrol" /> metodu ile sayfayı görme yetkisi olan kullanıcılar belirlenir
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
        public async Task<IActionResult> SelectListesiVer(string koordinatorlukKod, string birimKod, string listeyeEklenecekPersonelkod = "", bool icDenetciGetir = false)
        {
            var donenDeger = new List<SelectListesi>();
            var personelListesi = new List<ViewPersonel>();

            if (!string.IsNullOrWhiteSpace(koordinatorlukKod) || !string.IsNullOrWhiteSpace(birimKod))
            {
                string[] koorlar = koordinatorlukKod.Split(';');

                foreach (var koor in koorlar)
                {
                    if (string.IsNullOrWhiteSpace(koor)) continue;

                    var kriter = new ViewYetki() { KoordinatorlukKod = koor.Replace("_",""), BirimKod = "" };//Eğer tüm personel listenirse koordinatorlukKod='_' olarak gönderiliyor Melih 20.12.2023

                    var sonuc = await _service.ListeleAsync(_kullanan, kriter);

                    foreach (ViewYetki yetki in sonuc.Liste)
                    {
                        if (!personelListesi.Exists(x => x.Kod == yetki.Personel.Kod))
                        {
                            bool ekle = true;
                            foreach (ViewPersonel x in personelListesi)
                            {
                                if (x.Kod == yetki.Personel.Kod) { ekle = false; break; }
                            }

                            if (ekle)
                                personelListesi.Add(yetki.Personel);
                        }
                    }
                }
            }

            foreach (ViewPersonel per in personelListesi)
            {
                if (icDenetciGetir)
                {
                    if (per.UnvanKod != "4" && per.UnvanKod != "8") continue;
                }

                donenDeger.Add(new SelectListesi { id = per.Kod, text = per.AdiSoyadi + " - " + per.UnvanAdi });
            }

            if (!string.IsNullOrWhiteSpace(listeyeEklenecekPersonelkod))
            {
                bool personelVar = false;
                foreach (ViewPersonel per in personelListesi)
                {
                    if (per.Kod == listeyeEklenecekPersonelkod)
                        personelVar = true;
                }

                if (!personelVar)
                {
                    var sonuc2 = await _service.KayitGetirAsync(_kullanan, listeyeEklenecekPersonelkod);
                    if (sonuc2.IslemSonuc)
                    {
                        var viewYetki = (ViewYetki)sonuc2.Nesne;
                        donenDeger.Add(new SelectListesi { id = viewYetki.Personel.Kod, text = viewYetki.Personel.AdiSoyadi });
                    }
                }
            }

            return Ok(donenDeger);
        }

        /// <summary>
        /// Ekranlardaki Personel seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> RiskSahibiListesiVer(string koordinatorlukKod, string birimKod, string listeyeEklenecekPersonelkod = "")
        {
            var donenDeger = new List<SelectListesi>();
            var personelListesi = new List<ViewPersonel>();

            if (!string.IsNullOrWhiteSpace(koordinatorlukKod) || !string.IsNullOrWhiteSpace(birimKod))
            {
                var kriter = new ViewYetki() { KoordinatorlukKod = koordinatorlukKod };

                var sonuc = await _service.ListeleAsync(_kullanan, kriter);

                foreach (ViewYetki yetki in sonuc.Liste)
                {
                    if (!personelListesi.Exists(x => x.Kod == yetki.Personel.Kod))
                    {
                        if (string.IsNullOrWhiteSpace(yetki.BirimKod) || (!string.IsNullOrWhiteSpace(birimKod) && yetki.BirimKod == birimKod))
                            personelListesi.Add(yetki.Personel);
                    }
                }
            }

            foreach (ViewPersonel per in personelListesi)
            {
                donenDeger.Add(new SelectListesi { id = per.Kod, text = per.AdiSoyadi + " - " + per.UnvanAdi });
            }

            if (!string.IsNullOrWhiteSpace(listeyeEklenecekPersonelkod))
            {
                bool personelVar = false;
                foreach (ViewPersonel per in personelListesi)
                {
                    if (per.Kod == listeyeEklenecekPersonelkod)
                        personelVar = true;
                }

                if (!personelVar)
                {
                    var sonuc2 = await _service.KayitGetirAsync(_kullanan, listeyeEklenecekPersonelkod);
                    if (sonuc2.IslemSonuc)
                    {
                        var viewYetki = (ViewYetki)sonuc2.Nesne;
                        donenDeger.Add(new SelectListesi { id = viewYetki.Personel.Kod, text = viewYetki.Personel.AdiSoyadi });
                    }
                }
            }

            return Ok(donenDeger);
        }


        /// <summary>
        /// Ekranlardaki Personel seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> AzaltmaPlaniSorumlusuListesiVer(string koordinatorlukKod, string birimKod, string listeyeEklenecekPersonelkod = "")
        {
            var donenDeger = new List<SelectListesi>();
            var personelListesi = new List<ViewPersonel>();

            if (!string.IsNullOrWhiteSpace(koordinatorlukKod) || !string.IsNullOrWhiteSpace(birimKod))
            {
                var kriter = new ViewYetki() { KoordinatorlukKod = koordinatorlukKod };

                var sonuc = await _service.ListeleAsync(_kullanan, kriter);

                foreach (ViewYetki yetki in sonuc.Liste)
                {
                    if (!personelListesi.Exists(x => x.Kod == yetki.Personel.Kod))
                    {
                        if (string.IsNullOrWhiteSpace(yetki.BirimKod) || (!string.IsNullOrWhiteSpace(birimKod) && yetki.BirimKod == birimKod))
                            personelListesi.Add(yetki.Personel);
                    }
                }
            }

            foreach (ViewPersonel per in personelListesi)
            {
                donenDeger.Add(new SelectListesi { id = per.Kod, text = per.AdiSoyadi + " - " + per.UnvanAdi });
            }

            if (!string.IsNullOrWhiteSpace(listeyeEklenecekPersonelkod))
            {
                bool personelVar = false;
                foreach (ViewPersonel per in personelListesi)
                {
                    if (per.Kod == listeyeEklenecekPersonelkod)
                        personelVar = true;
                }

                if (!personelVar)
                {
                    var sonuc2 = await _service.KayitGetirAsync(_kullanan, listeyeEklenecekPersonelkod);
                    if (sonuc2.IslemSonuc)
                    {
                        var viewYetki = (ViewYetki)sonuc2.Nesne;
                        donenDeger.Add(new SelectListesi { id = viewYetki.Personel.Kod, text = viewYetki.Personel.AdiSoyadi });
                    }
                }
            }

            return Ok(donenDeger);
        }


        /// <summary>
        /// Ekranlardaki İç Kontrol Koordinatörlüğüne ait Personel seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerIcKontrol()
        {
            string koordinatorlukKod = Arac.ConfigOku("Genel:IcDenetimKoordinatorlukKod");
            string birimKod = "";

            return await SelectListesiVer(koordinatorlukKod, birimKod, "", true);
        }
    }
}
