using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StripeApp.Models;

namespace StripeApp.Controllers
{
    public class HomeController : Controller
    {
        private static string PUBLISHABLE_KEY_NAME = "PublishableKey";
        private static string US_PUBLISHABLE_KEY_NAME = "US_PublishableKey";
        private IConfiguration configuration;
        private readonly StripeSettingsOptions stripeSettingsOptions;
        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
            stripeSettingsOptions = new StripeSettingsOptions();
            this.configuration.GetSection(StripeSettingsOptions.StripeSettings).Bind(stripeSettingsOptions);
        }
        public IActionResult Index()
        {
            ViewData[PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.PublishableKey;
            ViewData[US_PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.US_PublishableKey;
            return View();
        }

        public IActionResult ConfirmPayment()
        {
            ViewData[PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.PublishableKey;
            ViewData[US_PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.US_PublishableKey;
            return View();
        }

        public IActionResult SetupComplete()
        {
            ViewData[PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.PublishableKey;
            ViewData[US_PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.US_PublishableKey;
            return View();
        }

        public IActionResult Capture()
        {
            ViewData[PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.PublishableKey;
            ViewData[US_PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.US_PublishableKey;
            return View();
        }

        public IActionResult Charge()
        {
            ViewData[PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.PublishableKey;
            ViewData[US_PUBLISHABLE_KEY_NAME] = stripeSettingsOptions.US_PublishableKey;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
