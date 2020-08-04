using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Areas.Admin.Controllers
{
    public class TESTController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "For testing";
            return View("Index");
        }
    }
}