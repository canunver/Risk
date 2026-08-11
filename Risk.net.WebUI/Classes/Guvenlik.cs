using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Functions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Classes
{
    //https://github.com/philipmat/AspNetCoreWindowsAuthClaims/blob/master/Startup.cs
    //https://stackoverflow.com/questions/63303699/net-core-3-1-web-application-with-react-how-to-prevent-access-based-on-active/63441045#63441045

    /// <summary>
    /// Windows Authentication yöntemiyle Kullanýcý yetkilerinin alýnmasýný saðlayan sýnýf
    /// </summary>
    public class ClaimsLoader : IClaimsTransformation
    {
        private readonly IViewYetkiService _serviceYetki;
        private readonly IPersonelAktifRolService _serviceAktifRol;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;

        public ClaimsLoader(IPersonelAktifRolService serviceAktifRol, IViewYetkiService serviceYetki, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _serviceYetki = serviceYetki;
            _serviceAktifRol = serviceAktifRol;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = (ClaimsIdentity)principal.Identity;

            //Daha önce cache'e yazýlmýþsa onu kullan
            if (!string.IsNullOrEmpty(identity.Name)
                 && _cache.TryGetValue(identity.Name, out ClaimsIdentity cachedIdentity))
            {
                return new ClaimsPrincipal(cachedIdentity);
            }

            Risk.net.Data.Entities.ViewYetki kriter = new Risk.net.Data.Entities.ViewYetki();
            kriter.KullaniciKod = identity.Name;
            Sonuc sonuc = await _serviceYetki.ListeleAsync(new Utilities.Objects.KullaniciDto(), kriter);

            if (sonuc.Liste.Count > 0)
            {
                Risk.net.Data.Entities.ViewYetki item = (Risk.net.Data.Entities.ViewYetki)sonuc.Liste[0];

                string resimUrl = "/Dosya/ResimGoster?kod=" + item.Personel.Kod;
                if (string.IsNullOrEmpty(resimUrl))
                {
                    resimUrl = "/Dosya/ResimGoster?kod=Bos";
                }

                identity.AddClaim(new Claim("Adi", item.Personel.Adi));
                identity.AddClaim(new Claim("Soyadi", item.Personel.Soyadi));
                identity.AddClaim(new Claim("AdiSoyadi", item.Personel.AdiSoyadi));
                identity.AddClaim(new Claim("Unvan", item.Personel.UnvanAdi));
                identity.AddClaim(new Claim("ResimUrl", resimUrl));
                identity.AddClaim(new Claim("PersonelKod", item.Personel.Kod));
                identity.AddClaim(new Claim("KullaniciKod", identity.Name));
                identity.AddClaim(new Claim("EPosta", item.Personel.EPosta));
                identity.AddClaim(new Claim("KoordinatorlukKod", Risk.net.Utilities.Functions.Arac.ConvertToStr(item.Personel.KoordinatorlukKod)));
                identity.AddClaim(new Claim("BirimKod", Risk.net.Utilities.Functions.Arac.ConvertToStr(item.Personel.BirimKod)));

                var aktifRolKod = "";
                foreach (Risk.net.Data.Entities.ViewYetki ytk in sonuc.Liste)
                {
                    if (string.IsNullOrWhiteSpace(item.Personel.Rol) ||
                        (item.Personel.Rol == ytk.Rol &&
                        Risk.net.Utilities.Functions.Arac.ConvertToStr(item.Personel.KoordinatorlukKod) == ytk.KoordinatorlukKod &&
                        Risk.net.Utilities.Functions.Arac.ConvertToStr(item.Personel.BirimKod) == ytk.BirimKod))
                    {
                        //item.Personel.Rol boþ ise daha önce iþlem yapýlmamýþtýr. Ýlk tanýmlý rol alýnýr
                        //item.Personel.Rol dolu ise daha önce iþlem yapýlmýþ ama rol geri alýnmýþ ise tekrar kullanmasýn kontrolü

                        aktifRolKod = ytk.Rol;
                        identity.AddClaim(new Claim(ClaimTypes.Role, ytk.Rol));
                        identity.AddClaim(new Claim("Yetki", ytk.Rol + "~" + ytk.KoordinatorlukKod + "~" + ytk.BirimKod));
                        break;
                    }
                }

                //eðer aktifRol doldurulamadý ise ilk rol set edilsin (Daha önce aktif olan rol geri alýnmýþtýr)
                if (aktifRolKod == "")
                {
                    foreach (Risk.net.Data.Entities.ViewYetki ytk in sonuc.Liste)
                    {
                        aktifRolKod = ytk.Rol;
                        if (aktifRolKod != "")
                        {
                            Risk.net.Data.Entities.PersonelAktifRol aRol = new Data.Entities.PersonelAktifRol();
                            aRol.PersonelKod = item.Personel.Kod;
                            aRol.Rol = aktifRolKod;
                            aRol.KoordinatorlukKod = ytk.KoordinatorlukKod;
                            aRol.BirimKod = ytk.BirimKod;
                            await _serviceAktifRol.KaydetAsync(null, aRol);
                        }

                        break;
                    }

                }

                identity.AddClaim(new Claim("AktifRolKod", aktifRolKod));
            }
            else
            {
                //Yazýlýmý kullanan kullanýcýya herhangi bir yetki verilmemiþ ise
                //cache yazýlmasýn
                identity.AddClaim(new Claim("AdiSoyadi", identity.Name));
                identity.AddClaim(new Claim("Unvan", "Yetkisiz kullanýcý"));
                identity.AddClaim(new Claim("ResimUrl", "https://icon-library.com/images/block-user-icon/block-user-icon-16.jpg"));
                identity.AddClaim(new Claim("Adi", ""));
                identity.AddClaim(new Claim("Soyadi", ""));
                identity.AddClaim(new Claim("PersonelKod", ""));
                identity.AddClaim(new Claim("KullaniciKod", ""));
                identity.AddClaim(new Claim("EPosta", ""));
                identity.AddClaim(new Claim("AktifRolKod", ""));

                return new ClaimsPrincipal(identity);
            }

            //Kullanýcýnýn yetkileri alýndýktan sonra cache yazýlsýn
            _cache.Set(identity.Name, identity, DateTime.Now.AddHours(12));

            return new ClaimsPrincipal(identity);
        }
    }

    /// <summary>
    /// sayfalara giriþ sýrasýnda kullanýcýya verilen yetkilerin kontrol edildiði sýnýf
    /// </summary>
    public class YetkiKontrol : AuthorizeAttribute, IAuthorizationFilter
    {
        public string Yetkiler { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (string.IsNullOrEmpty(Yetkiler))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var identity = (ClaimsIdentity)context.HttpContext.User.Identity;

            bool kontrol = false;

            //PlanlamaUnitesi her yetkiye sahiptir
            if (identity.HasClaim(ClaimTypes.Role, "PLANLAMAUNITESI"))
                return;

            if (Yetkiler == "*")//Herhangi bir yetkisi var mý kontrolü
            {
                foreach (string tanimliYetki in Enum.GetNames(typeof(Risk.net.Utilities.Objects.ENUMKullaniciRol)))
                {
                    try
                    {
                        kontrol = identity.HasClaim(ClaimTypes.Role, tanimliYetki);
                        if (kontrol) break;
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            else//Belirli bir yetkisi var mý kontrolü
            {
                string[] parcalar = Yetkiler.Split(',', ';', ' ');
                foreach (string parca in parcalar)
                {
                    if (string.IsNullOrEmpty(parca)) continue;

                    kontrol = identity.HasClaim(ClaimTypes.Role, parca.Trim());
                    if (kontrol) break;
                }
            }

            if (kontrol)
                return;
            else
                context.Result = new ForbidResult();

            return;
        }
    }
}
