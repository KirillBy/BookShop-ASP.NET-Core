using MVC5_Online_shop.Models.Data;
using MVC5_Online_shop.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5_Online_shop.Areas.Admin
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageList;

            using(Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();
            }

            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //validation check
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            using(Db db = new Db())
            {
                //declaration variable for slug
                string slug;

                //initialization class PageDTO
                PagesDTO dto = new PagesDTO();

                //Assing header to model
                dto.Title = model.Title.ToUpper();

                //Checking if slug is exist
                if(string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                //Check if header and slug are unique
                if(db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", " That title is already exist.");
                    return View(model);
                }
                else if(db.Pages.Any( x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", " That slug is already exist.");
                    return View(model);
                }

                //Assing all other values
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;

                //save model to Db
                db.Pages.Add(dto);
                db.SaveChanges();

            }

            //Send message throw TempData
            TempData["SM"] = "You have added new page!";

            //Redirect user to INDEX method
            return RedirectToAction("Index");
        }
    }
}