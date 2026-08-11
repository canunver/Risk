using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Risk.net.Services;
using System.Security.Claims;

/// Identity konusu
/// https://www.youtube.com/watch?v=B0_gM-wBlmE
//Authorize
///https://www.youtube.com/watch?v=BWa7Mu-oMHk&list=PLnearjYoCRfzlf6nIhisLK_1Mv-fjoc47&index=1
/// theme
///https://pixinvent.com/demo/vuexy-html-bootstrap-admin-template/html/ltr/vertical-menu-template/page-auth-login-v2.html
///https://smartadmin-core.azurewebsites.net/
///https://www.freepik.com/search?dates=any&format=search&page=1&query=illustrations&selection=1&sort=popular&type=vector

namespace Risk.net.WebUI.Controllers
{

    public class AccountController : Controller
    {
        /// <summary>
        /// IStringLocalizer servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;
        /// <summary>
        /// IMemoryCache servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// <see cref="Risk.net.WebUI.Controllers.AccountController" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public AccountController(IMemoryCache cache, IStringLocalizer<CustomResource> sharedResource)
        {
            _cache = cache;
            _sharedResource = sharedResource;
        }

        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return Redirect("/");
        }

        /// <summary>
        /// Kullanıcıya ait cache de tutulan bilgilerini boşaltıp logout sayfasına yönlendirilmesini sağlayan metod
        /// </summary>
        /// <remarks></remarks>
        public IActionResult Logout()
        {
            var identity = (ClaimsIdentity)User.Identity;

            string cacheId = identity.FindFirst(ClaimTypes.Name).Value;

            _cache.Remove(cacheId);

            return View();
        }

        /// <summary>
        /// Kullanıcının yetkisi olmayan bir işlem veya ekrana girdiğinde accessDenied sayfasına yönlendirilmesini sağlayan metod
        /// </summary>
        /// <remarks></remarks>
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Kullanıcı yazılımda olmayan bir sayfaya girdiğinde 404 sayfasına yönlendirilmesini sağlayan metod
        /// </summary>
        /// <remarks></remarks>
        public IActionResult _404()
        {
            return View();
        }

        /// <summary>
        /// Kaynağa ulaşılmasının engellendiği zaman 401 sayfasına yönlendirilmesini sağlayan metod
        /// </summary>
        /// <remarks></remarks>
        public IActionResult _401()
        {
            return View();
        }

        /// <summary>
        /// Erişim izni olmayan bir kaynağa ulaşılmak istendiği zaman 403 sayfasına yönlendirilmesini sağlayan metod
        /// </summary>
        /// <remarks></remarks>
        public IActionResult _403()
        {
            return View();
        }
    }
}
