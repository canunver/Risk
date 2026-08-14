using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.WebUI.ViewComponents
{
    /// <summary>
    /// DosyaKontrol işlemlerinin yapıldığı sayfa
    /// </summary>
    public class DosyaKontrolViewComponent : ViewComponent
    {
        /// <summary>
        /// IDosyaService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDosyaService _service;
        /// <summary>
        /// Kullanıcı bilgisini taşıyan değişken
        /// </summary>
        /// <remarks></remarks>
        public readonly KullaniciDto _kullanan;

        /// <summary>
        /// <see cref="Risk.net.WebUI.ViewComponents.DosyaKontrolViewComponent" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContextAccessor"></param>
        /// <remarks></remarks>
        public DosyaKontrolViewComponent(IDosyaService service,
                                        IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _kullanan = Arac.KullaniciNesnesiOlustur(httpContextAccessor.HttpContext.User);
        }

        /// <summary>
        /// DosyaKontrol ViewComponent sayfası açıldığında çalışan metod.
        /// </summary>
        /// <param name="baglantiKod"></param>
        [HttpPost]
        public async Task<IViewComponentResult> InvokeAsync(string form)
        {
            dynamic gelenKriter = JObject.Parse(form);
            string baglantiKod = "";
            Sonuc sonuc = new Sonuc();

            try
            {
                baglantiKod = gelenKriter.BaglantiKod.Value;
            }
            catch { }

            if (string.IsNullOrWhiteSpace(baglantiKod))
            {
                baglantiKod = "";
                sonuc.Liste = new System.Collections.Generic.List<object>();
            }
            else
                sonuc = await _service.ListeleAsync(_kullanan, gelenKriter.BaglantiKod.Value);

            sonuc.AnahtarAlan = baglantiKod;
            ViewBag.ModalGoster = gelenKriter.ModalGoster.Value;
            ViewBag.Zorunlu = gelenKriter.Zorunlu.Value;
            ViewBag.MaxDosyaSayisi = gelenKriter.MaxDosyaSayisi.Value;
            ViewBag.SilmeYetkisi = gelenKriter.SilmeYetkisi.Value;
            ViewBag.SadeceOkuma = gelenKriter.SadeceOkuma.Value;

            return View(sonuc);
        }
    }
}
