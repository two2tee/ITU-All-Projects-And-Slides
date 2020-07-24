// HomeController.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using Microsoft.AspNetCore.Mvc;

namespace LiftLog.Web.Controllers.Web
{
    /// <summary>
    ///     Controller handles requests on default home Index page.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        ///     This action redirects to the default home page.
        /// </summary>
        /// <returns> Index home page. </returns>
        public IActionResult Index(string message)
        {
            ViewData["StatusMessage"] = message;
            return View();
        }

        /// <summary>
        ///     This action handles requests to the About page and redirects to it.
        /// </summary>
        /// <returns> About page. </returns>
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        ///     This action handles requests to Contact Page and redirects to it.
        /// </summary>
        /// <returns> Contact page. </returns>
        public IActionResult Contact()
        {
            return View();
        }
    }
}