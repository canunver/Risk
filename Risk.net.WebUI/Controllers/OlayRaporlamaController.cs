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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Olay Raporlama işlemlerinin yapıldığı sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class OlayRaporlamaController : GenelController
    {
        /// <summary>
        /// IOlayRaporlamaService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IOlayRaporlamaService _service;
        /// <summary>
        /// IRiskEvreniService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniService _serviceRiskEvreni;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.OlayRaporlamaController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceRiskEvreni"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public OlayRaporlamaController(IOlayRaporlamaService service,
                                        IRiskEvreniService serviceRiskEvreni,
                                        IHttpContextAccessor httpContextAccessor,
                                        IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceRiskEvreni = serviceRiskEvreni;
        }

        /// <summary>
        /// OlayRaporlama View sayfası açıldığında çalışan metod
        /// </summary>
        public IActionResult Index()
        {
            //if (Arac.YetkisiVarmi("BIRIMAMIRI", User) ||
            //    Arac.YetkisiVarmi("ILKOORDINATOR", User) ||
            //    Arac.YetkisiVarmi("MERKEZKOORDINATOR", User) ||
            //    Arac.YetkisiVarmi("GENELKOORDINATOR", User) ||
            //    Arac.YetkisiVarmi("ICDENETIMKOORDINATOR", User) ||
            //    Arac.YetkisiVarmi("BASKAN", User))
            //    ViewBag.OnayYetki = true;
            //else
            //    ViewBag.OnayYetki = false;

            //if (Arac.YetkisiVarmi("BIRIMAMIRI", User))
            //    ViewBag.UsteGonder = true;
            //else
            //    ViewBag.UsteGonder = false;

            ViewBag.DurumListesi = Ortak.DurumListesiVer(_sharedResource, EnumTarihceIslemTur.OlayRaporlama, _kullanan);
            ViewBag.KoordinatorlukKodu = _kullanan.KoordinatorlukKod;
            ViewBag.BirimKodu = _kullanan.BirimKod;

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

            var jsonData = await _service.TabloDoldurAsync(_kullanan, dataTableInfo, false);

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
        public async Task<ActionResult> Kaydet(OlayRaporlama form)
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
            OlayRaporlama form = new OlayRaporlama();
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
            OlayRaporlama form = new OlayRaporlama();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.Onayli;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydın onaya gönderme işlemini sağlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> OnayaGonder(string kod)
        {
            OlayRaporlama form = new OlayRaporlama();
            form.Kod = kod;
            form.Durum = (int)ENUMDurum.OnayaGonderdi;

            Sonuc sonuc = await _service.DurumDegistirAsync(_kullanan, form);

            return Ok(sonuc);
        }

        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <param name="koordinatorlukKod"></param>
        /// <param name="birimKod"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVerRiskEvreni(string koordinatorlukKod, string birimKod)
        {
            RiskEvreni kriter = new RiskEvreni();
            kriter.KoordinatorlukKod = koordinatorlukKod;
            kriter.BirimKod = birimKod;
            kriter.Durum = (int)ENUMDurum.Onayli;

            var sonuc = await _serviceRiskEvreni.ListeleAsync(_kullanan, kriter);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (RiskEvreni item in sonuc.Liste)
            {
                string ekBilgi = "";
                if (item.RiskKategoriler != null)
                {
                    foreach (RiskEvreniRiskKategori rk in item.RiskKategoriler)
                    {
                        if (ekBilgi != "") ekBilgi += ", ";
                        ekBilgi += rk.RiskKategori.Adi;
                    }
                }

                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.RiskAdi, ekBilgi = ekBilgi });
            }

            return Ok(donenDeger);
        }

        /// <summary>
        /// Ekranlardaki Kontrol seçim kutularının doldurulması için çağrılan metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> SelectListesiVer(OlayRaporlama form)
        {
            var sonuc = await _service.ListeleAsync(_kullanan, form);

            List<SelectListesi> donenDeger = new List<SelectListesi>();

            foreach (OlayRaporlama item in sonuc.Liste)
            {
                donenDeger.Add(new SelectListesi { id = item.Kod, text = item.OlayTanimi });
            }

            return Ok(donenDeger);
        }
    }
}
