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

        // GET: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //declare string variable ID
            string id;

            using(Db db = new Db())
            {
                //check category for unique
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                //initialize model DTO
                CategoryDTO dto = new CategoryDTO();

                //add data to model
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                //save
                db.Categories.Add(dto);
                db.SaveChanges();

                //get id to return to view
                id = dto.Id.ToString();
            }

            //return id to view
            return id;
        }

        //POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                //Realization of counter
                int count = 1;

                //Initialize data model
                CategoryDTO dto;

                //Setup sorting for each page
                foreach (var catit in id)
                {
                    dto = db.Categories.Find(catit);
                    dto.Sorting = count;

                    db.SaveChanges();

                    count++;
                }
            }

        }

        //GET: Admin/Shop/DeleteCategory/id
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //Getting category
                CategoryDTO dto = db.Categories.Find(id);

                //Deleting category
                db.Categories.Remove(dto);

                //Saving changes to db
                db.SaveChanges();
            }

            //Send message about succesfull deleting
            TempData["SM"] = "You have delete category succesfully!";


            //Redirect user
            return RedirectToAction("Categories");
        }

        //Post: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory (string newCatName, int id)
        {
            using(Db db = new Db())
            {
                //checking name for unique
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                //get data from dto
                CategoryDTO dto = db.Categories.Find(id);

                //edit model dto
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //saving changes
                db.SaveChanges();
            }

            //return string
            return "ok";
        }

        //GET: Admin/Shop/AddProduct/id
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Declare data model
            ProductVM model = new ProductVM();

            //add list of categories from db to model
            using(Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            //return model to view
            return View(model);

        }
    }
}