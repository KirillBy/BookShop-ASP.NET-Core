using MVC5_Online_shop.Models.Data;
using MVC5_Online_shop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5_Online_shop.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            //declare list midel CategoryVM
            List<CategoryVM> categoryVMList;

            //init model with data
            using(Db db = new Db() )
            {
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x)).ToList();
            }

            //return partial view
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }
    }
}