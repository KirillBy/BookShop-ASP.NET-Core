using MVC5_Online_shop.Models.Data;
using MVC5_Online_shop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5_Online_shop.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //declare model of type List
            List<CategoryVM> categoryVMList;

            using(Db db = new Db())
            {
                //initialize model with data
                categoryVMList = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }


            //return List to view
            return View(categoryVMList);
        }
    }
}