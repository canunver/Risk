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
    /// DosyaKontrol iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public class DosyaKontrolViewComponent : ViewComponent
    {
        /// <summary>
        /// IDosyaService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDosyaService _service;
        /// <summary>
        /// Kullanýcý bilgisini taþýyan deðiþken
        /// </summary>
        /// <remarks></remarks>
        public readonly KullaniciDto _kullanan;

        /// <summary>
        /// <see cref="Risk.net.WebUI.ViewComponents.DosyaKontrolViewComponent" /> 'ýn yeni bir örneðini baþlatan sýnýf
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
        /// DosyaKontrol ViewComponent sayfasý açýldýðýnda çalýþan metod.
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
