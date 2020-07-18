using MVC5_Online_shop.Models.Data;
using MVC5_Online_shop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5_Online_shop.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            //get/set slug header
            if (page == "")
                page = "home";


            //declare model and dto
            PageVM model;
            PagesDTO dto;

            //check if page is available
            using(Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                    return RedirectToAction("Index", new { page = "" });
            }

            //get context dto of page
            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            //set title of page
            ViewBag.PageTitle = dto.Title;

            //check if sidebar is exist
            if(dto.HasSidebar == true)
            {
                ViewBag.Sidebar = "Yes";
            }
            else
            {
                ViewBag.Sidebar = "No";
            }

            //fill model with data
            model = new PageVM(dto);

            //return view with model
            return View(model);
        }

        public ActionResult PagesMenuPartial()
        {
            //init list PageVM
            List<PageVM> pageVMList;

            //get all pages except HOME
            using( Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().OrderBy(x => x.Sorting)
                    .Where(x => x.Slug != "home")
                    .Select(x => new PageVM(x)).ToList();

            }

            //return partial view and list with data
            return PartialView("_PagesMenuPartial", pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            //declare model
            SidebarVM model;

            //init model 
            using(Db db = new Db())
            {
                SidebarDTO dto = db.Sidebars.Find(1);

                model = new SidebarVM(dto);
            }

            //return model to partial view
            return PartialView("_SidebarPartial", model);
        }
    }
}