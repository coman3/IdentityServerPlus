using IdentityServer.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.ThemeProvider.SimpleBox.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExampleController : Controller
    {

        public IActionResult Index()
        {
            var context = HttpContext;
            return View("Test");
        }
    }
}
