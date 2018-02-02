using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SentryWeb.Models;
using Serilog;

namespace SentryWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            // Explicitly call our error logger
            Log.Error("Intentional error logged at {TimeStamp}", DateTime.Now.ToLongTimeString());

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            // Demonstrate that Sentry will capture uncaught exceptions
            DivByZero();

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        static int DivByZero()
        {
            var i = 0;
            var j = 1 / i;
            return j;
        }
    }
}
