using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Risk.net.Data.Entities;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using Risk.net.WebUI.Classes;
using Risk.net.WebUI.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Localization;
using Risk.net.Services;
using Risk.net.Services.Objects;
using System.Collections.Generic;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Dosya iþlemlerinin yapýldýðý sayfa
    /// </summary>
    [Authorize]
    [YetkiKontrol(Yetkiler = "*")]
    public class DosyaController : GenelController
    {
        /// <summary>
        /// IDosyaService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDosyaService _service;
        /// <summary>
        /// IViewPersonelService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewPersonelService _serviceViewPersonel;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.DosyaController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceViewPersonel"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public DosyaController(IDosyaService service,
                                IViewPersonelService serviceViewPersonel,
                                IHttpContextAccessor httpContextAccessor,
                                IStringLocalizer<CustomResource> sharedResource) : base(httpContextAccessor, sharedResource)
        {
            _service = service;
            _serviceViewPersonel = serviceViewPersonel;
        }

        /// <summary>
        /// Listeden seçilen kaydýn dosyasýnýn indirme iþlemini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DosyaIndir(string kod)
        {
            try
            {
                //Ýstenen dosyayý oku
                Dosya dosya = await _service.IndirAsync(_kullanan, kod, "");

                //Gönderilecek dosyayý geçici dosyaya yaz
                string geciciDosya = Arac.DosyaAdUret();
                await System.IO.File.WriteAllBytesAsync(geciciDosya, dosya.Icerik);

                return Arac.DosyaGonder(geciciDosya, dosya.Adi, true);
            }
            catch (Exception e)
            {
                Arac.HataStrYaz("Dosya indiriliken hata oluþtu:" + e.Message + "\nDosya parametresi:" + kod);
            }

            return NoContent();
        }

        /// <summary>
        /// Listeden seçilen kaydýn dosyasýnýn indirme iþlemini saðlayan metod
        /// </summary>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DosyaIndirBaglanti(string baglantiKod)
        {
            try
            {
                //Ýstenen dosyayý oku
                Dosya dosya = await _service.IndirAsync(_kullanan, "", baglantiKod);

                //Gönderilecek dosyayý geçici dosyaya yaz
                string geciciDosya = Arac.DosyaAdUret();
                await System.IO.File.WriteAllBytesAsync(geciciDosya, dosya.Icerik);

                return Arac.DosyaGonder(geciciDosya, dosya.Adi, true);
            }
            catch (Exception e)
            {
                Arac.HataStrYaz("Dosya indiriliken hata oluþtu:" + e.Message + "\nDosya parametresi:" + baglantiKod);
            }

            return NoContent();
        }

        /// <summary>
        /// Kullanýcýdan gelen kriterler ile ilgili kayýdýn resim bilgisinin sunucudan getirilmesini saðlayan metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpGet]
        public async Task<FileContentResult> ResimGoster(string kod)
        {
            string sablonYol = Arac.ConfigOku("Genel:RaporSablonYol", "");
            string bosResimAdresi = Path.Combine(sablonYol, "Resim", "bos.jpg");

            if (!string.IsNullOrWhiteSpace(kod) && kod != "bos")
            {
                try
                {
                    //Arac.HataStrYaz("Resim indiriliyor");
                    //Arac.HataStrYaz("boþ resim:" + bosResimAdresi);

                    Sonuc dosya = await _serviceViewPersonel.ResimGetirAsync(kod);

                    byte[] resim = ((ViewPersonelResim)dosya.Nesne)?.Resim;
                    if (resim == null)
                    {
                        Arac.HataStrYaz("resim null");
                        resim = System.IO.File.ReadAllBytes(bosResimAdresi);
                        Arac.HataStrYaz("boþ resim okundu" + resim.Length);
                    }

                    return File(resim, "image/jpg");
                }
                catch (Exception e) { Arac.HataStrYaz(e.Message + " " + e.StackTrace); }
            }

            byte[] bosResim = System.IO.File.ReadAllBytes(bosResimAdresi);

            return File(bosResim, "image/jpg");
        }

        /// <summary>
        /// Kullanýcýdan gelen kriterler ile ilgili kayýdýn dosyasýnýn varlýðýný sorgulayan metod
        /// </summary>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DosyaVarmi(string baglantiKod)
        {
            Sonuc sonuc = await _service.KayitGetirAsync(_kullanan, "", baglantiKod);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanýcýdan gelen kriterler ile ilgili kayýdýn dosyasýnýn varlýðýný sorgulayan metod
        /// </summary>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DosyaSayiVer(string baglantiKod)
        {
            Sonuc sonuc = await _service.SayiVerAsync(_kullanan, baglantiKod);

            return Ok(sonuc);
        }

        /// <summary>
        ///Kullanýcýdan gelen bilgilerinin ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Kaydet()
        {
            Sonuc sonuc = new Sonuc();
            List<Dosya> gidecekDosyalar = new List<Dosya>();

            string baglantiKod = Request.Form["BaglantiKod"];
            if (string.IsNullOrEmpty(baglantiKod))
            {
                sonuc = new Sonuc(ENUMIslemDurum.Uyari, "Kayýt edilecek dosya yok", null);
                return Ok(sonuc);
            }

            if (Request.Form.Files.Count > 0)
            {
                try
                {
                    IFormFileCollection gelenDosyalar = Request.Form.Files;
                    for (int i = 0; i < gelenDosyalar.Count; i++)
                    {
                        IFormFile gelenDosya = gelenDosyalar[i];
                        string gelenDosyaAdi = gelenDosya.FileName;

                        Dosya kayitEdilecekDosya = new Dosya();
                        kayitEdilecekDosya.BaglantiKod = baglantiKod;
                        kayitEdilecekDosya.Adi = gelenDosyaAdi;

                        int dIndex = kayitEdilecekDosya.Adi.IndexOf("\\");
                        if (dIndex > -1)
                        {
                            kayitEdilecekDosya.Adi = kayitEdilecekDosya.Adi.Substring(dIndex + 1);
                        }

                        kayitEdilecekDosya.Icerik = new byte[gelenDosya.Length];
                        gelenDosya.OpenReadStream().Read(kayitEdilecekDosya.Icerik, 0, Arac.ConvertToInt(gelenDosya.Length));

                        sonuc = await _service.KaydetAsync(_kullanan, kayitEdilecekDosya);
                        if (sonuc.IslemSonuc)
                        {

                            Dosya nesne = (Dosya)sonuc.Nesne;
                            Dosya nesneKlon = nesne.Clone();
                            nesneKlon.KayitEden = new ViewPersonel();
                            nesneKlon.KayitEden.Kod = _kullanan.PersonelKod;
                            nesneKlon.KayitEden.Adi = _kullanan.Adi;
                            nesneKlon.KayitEden.Soyadi = _kullanan.Soyadi;

                            gidecekDosyalar.Add(nesneKlon);
                        }
                    }

                    sonuc.Liste = new List<object>();
                    foreach (var item in gidecekDosyalar)
                    {
                        sonuc.Liste.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    sonuc = new Sonuc(ENUMIslemDurum.Hata, ex.Message, null);
                }
            }
            else
            {
                sonuc = new Sonuc(ENUMIslemDurum.Uyari, "Kayýt edilecek dosya yok", null);
            }

            return Ok(sonuc);
        }

        /// <summary>
        /// Listeden seçilen kaydýn silinmesini saðlayan metod
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
        /// Kullanýcýdan gelen eski ve yeni bilgileri ilgili servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="eskiKod"></param>
        /// <param name="yeniKod"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> BaglantiKodGuncelle(string eskiKod, string yeniKod)
        {
            Sonuc sonuc = await _service.BaglantiKodGuncelleAsync(_kullanan, eskiKod, yeniKod);

            return Ok(sonuc);
        }

        /// <summary>
        /// Kullanýcýdan gelen açýklama bilgisini servise kayýt edilmesi için gönderen metod
        /// </summary>
        /// <param name="form"></param>
        /// <returns>
        /// Ok(sunucudan gelen Sonuc tipinde nesne)
        /// </returns>
        /// <remarks></remarks>
        [HttpPost]
        public async Task<IActionResult> AciklamaKaydet(Dosya form)
        {
            Sonuc sonuc = await _service.AciklamaKaydetAsync(_kullanan, form);

            return Ok(sonuc);
        }
    }
}
