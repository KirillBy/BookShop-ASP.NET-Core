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

        // GET: Admin/Pages/EditPafe/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //declaration of model PageVM
            PageVM model;

            using(Db db = new Db())
            {
                //Getting page by id
                PagesDTO dto = db.Pages.Find(id);

                //Checkig if the page is available
                if(dto == null)
                {
                    return Content("The page doesn't exist.");
                }

                //Initialize model with data
                model = new PageVM(dto);
            }


            //Return model to view
            return View(model);
        }

        // POST: Admin/Pages/EditPage/model
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //validation model check
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            using(Db db = new Db())
            {
                //getting ID of page
                int id = model.Id;

                //declare variable for slug
                string slug = "home";

                //getting page by id
                PagesDTO dto = db.Pages.Find(id);

                //assign name from received model DTO
                dto.Title = model.Title.ToUpper();

                //checking if the slug exist and assign him if it's nessesary
                if(model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                //checking slug and title for unique
                if(db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title is already exist");
                    return View(model);
                }
                else if(db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                    {
                    ModelState.AddModelError("", "That slug is already exist");
                    return View(model);
                    }

                //assing other value from DTO
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                //saving to Db
                db.SaveChanges();
            }


            //set up message in TempData
            TempData["SM"] = "You have edited the page.";

            //redirect user to 
            return RedirectToAction("EditPage");
        }

        //GET: Admin/Pages/PageDetails/id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            //Declare model PageVM
            PageVM model;

            using(Db db = new Db())
            {
                //Getting page 
                PagesDTO dto = db.Pages.Find(id);

                //Checkint if page is available
                if(dto == null)
                {
                    return Content("The page doesn't exist");
                }
                //Assing fields from db to dto model
                model = new PageVM(dto);
            }

            //return model to view
            return View(model);
        }

        //GET: Admin/Pages/DeletePage/id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Getting page
                PagesDTO dto = db.Pages.Find(id);

                //Deleting page
                db.Pages.Remove(dto);

                //Saving changes to db
                db.SaveChanges();
            }

            //Send message about succesfull deleting
            TempData["SM"] = "You have delete page succesfully!";


            //Redirect user
            return RedirectToAction("Index");
        }

        //GET: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                //Realization of counter
                int count = 1;

                //Initialize data model
                PagesDTO dto;

                //Setup sorting for each page
                foreach(var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }

        }
    }
}