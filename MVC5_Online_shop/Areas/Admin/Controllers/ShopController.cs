using MVC5_Online_shop.Models.Data;
using MVC5_Online_shop.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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

        //GET: Admin/Shop/AddProduct/
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

        //POST: Admin/Shop/AddProduct/
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //validation check
            if(!ModelState.IsValid)
            {
                using(Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            //unique check
            using(Db db = new Db())
            {
                if(db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }

            //declare product id variable
            int id;

            //initialize and saving model based on productdto
            using(Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }

            //add message to tempdata
            TempData["SM"] = "New product add succesfully";

            #region Upload Image

            //create links on directory
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString()+ "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            //checking if directory exist(if doesn't exist - create)
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            //checking if file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                //Get exstension of file
                string ext = file.ContentType.ToLower();

                //checking extension of file
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image wasn't uploaded caused by incorrect extension");
                        return View(model);
                    }
                }


                //declare variable with image name
                string imageName = file.FileName;

                //saving image name to model dtp
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                //set routes to original and small image
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                //save origin image
                file.SaveAs(path);

                //create and save small copy
                WebImage img = new WebImage(file.InputStream);
                img.Resize(width: 200, height: 200);
                img.Save(path2);
            }
            #endregion

            //redirect to user action
            return RedirectToAction("AddProduct");
        }

        //Get: Admin/Shop/Product/
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            //declare ProductVM List
            List<ProductVM> listOfProductVM;

            //set number of page
            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                //initialize list and fill it with data
                listOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                //fill categories with data
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                //set chosen category
                ViewBag.SelectedCat = catId.ToString();

            }

            //set paged navigation
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.onePageOfProducts = onePageOfProducts;

            //return view with data
            return View(listOfProductVM);
        }

        //Get: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            //declare model productVM
            ProductVM model;

            using (Db db = new Db())
            {
                //get product
                ProductDTO dto = db.Products.Find(id);

                //check if product available
                if(dto == null)
                {
                    return Content("That product doesn't exist");
                }

                //initialize model with data
                model = new ProductVM(dto);

                //generate category list
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                //get all images from gallery
                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            //return model to view
            return View(model);
        }

        //POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            //get an id of product
            int id = model.Id;

            //fill list with categories and images
            using(Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            //check validation of model
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            //check name of product for unique
            using(Db db = new Db())
            {
                if(db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is already taken");
                    return View(model);
                }
            }    

            //update product
            using(Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();

            }

            //set message to tempDate
            TempData["SM"] = "You have edited the product!"; 

            //logik of working with image
            #region Image Upload
            //Check if file is loaded
            if(file != null && file.ContentLength > 0)
            {
                //Get extension of file
                string ext = file.ContentType.ToLower();
                //Check extension of file
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image wasn't uploaded caused by incorrect extension");
                        return View(model);
                    }
                }
                //Set routes for loading
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                //Delete existed file and directory
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach(var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                //Save image name
                string imageName = file.FileName;

                using(Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                //Save origin and preview version
                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                //save origin image
                file.SaveAs(path);

                //create and save small copy
                WebImage img = new WebImage(file.InputStream);
                img.Resize(width: 200, height: 200);
                img.Save(path2);
            }
            #endregion

            //redirect of user
            return RedirectToAction("EditProduct");
        }

        //GET: Admin/Shop/DeleteProduct/id
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            //Delete product from db
            using(Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);

                db.SaveChanges();
            }

            //Delete directory of image
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            //Redirect user
            return RedirectToAction("Products");
        }
    }
}