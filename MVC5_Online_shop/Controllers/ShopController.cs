using MVC5_Online_shop.Models.Data;
using MVC5_Online_shop.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
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

        // GET: Shop/Category/name
        public ActionResult Category(string name)
        {
            //declare list 
            List<ProductVM> productVMList;

            using(Db db = new Db())
            {
                //get id category
                CategoryDTO categoryDTO = db.Categories
                    .Where(x => x.Slug == name).FirstOrDefault();

                int catId = categoryDTO.Id;

                //init list with data
                productVMList = db.Products.ToArray().Where(x => x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                //get category name
                var productCat = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                //null check
                if(productCat == null)
                {
                    var catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name)
                        .FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                {
                    ViewBag.CategoryName = productCat.CategoryName;
                }

            }

            //return view with model
            return View(productVMList);

        }

        // GET: Shop/products-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //declare dto and vm models
            ProductDTO dto;
            ProductVM model;

            //init ID of product
            int id = 0;

            using (Db db = new Db())
            {
                //check if product is available
                if(! db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                //init model dto with data
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                //get ID
                id = dto.Id;

                //init model VM with data
                model = new ProductVM(dto);
            }

            //get image from gallery
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id +
                "/Gallery/Thumbs")).Select(fn => Path.GetFileName(fn));

            //return model to view
            return View("ProductDetails", model);
        }
    }
}