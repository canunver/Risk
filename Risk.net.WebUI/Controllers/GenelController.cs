using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Risk.net.Services;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Controllers
{
    /// <summary>
    /// Genel iþlemlerinin yapýldýðý sayfa
    /// </summary>
    public class GenelController : Controller
    {
        /// <summary>
        /// KullaniciDto servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        public readonly KullaniciDto _kullanan;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        public readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.GenelController" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public GenelController(IHttpContextAccessor httpContextAccessor, IStringLocalizer<CustomResource> sharedResource)
        {
            try
            {
                _kullanan = Arac.KullaniciNesnesiOlustur(httpContextAccessor.HttpContext.User);
                _sharedResource = sharedResource;

                if (_kullanan.Roller == null || _kullanan.Roller.Count == 0)
                {
                    httpContextAccessor.HttpContext.Response.Redirect("Account/AccessDenied", true);
                }
            }
            catch
            {
                httpContextAccessor.HttpContext.Response.Redirect("Account/_401", true);

            }

        }
    }
}
