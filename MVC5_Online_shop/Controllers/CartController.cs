using Logic.Services;
using MVC5_Online_shop.Models.Data;
using MVC5_Online_shop.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC5_Online_shop.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService cartservice;

        public CartController(ICartService cartservice)
        {
            this.cartservice = cartservice;
        }
        // GET: Cart
        public ActionResult Index()
        {
            //declare list of type CartVM
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //check if cart is empty or not
            if(cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty";
                return View();
            }

            //if not empty put summ to viewbag
            decimal total = 0m;

            foreach(var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            //return list to view
            return View(cart);
        }

        public ActionResult CartPartial()
        {
            //declare CartVM model
            CartVM model = new CartVM();

            //declare variavle of quantity
            int qty = 0;

            //declare variable of price
            decimal price = 0m;

            //check session of cart
            if(Session["cart"] != null)
            {
                //get total numbers of products and price
                var list = (List<CartVM>)Session["cart"];

                foreach(var item in list)
                {
                    qty += item.Quantity;
                    price += item.Quantity * item.Price;
                }


                model.Quantity = qty;
                model.Price = price;
            }
            else
            {
                //or set number and price to 0
                model.Quantity = 0;
                model.Price = 0m;
            }

            //return partial view
            return PartialView("_CartPartial",model);
        }

        public ActionResult AddToCartPartial(int id)
        {
            //declare List<CartVM>
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //declare model CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())

            {
                //get product
                ProductDTO product = db.Products.Find(id);

                //check if product is already in cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                //if no, than add 
                if(productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }

                //if yes, add one more
                else
                {
                    productInCart.Quantity++;
                }

            }

            //get total number of products, price and add data to model
            int qty = 0;
            decimal price = 0m;

            foreach(var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            //save cart to session
            Session["cart"] = cart;

            //return partial view with model
            return PartialView("_AddToCartPartial", model);

        }

        //GET:/cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            // declare List<CartVM>
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using(Db db = new Db())
            {
                //get CartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //add quantity
                model.Quantity++;

                //save data
                var result = new { qty = model.Quantity, price = model.Price };

                //return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        //GET:/cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            // declare List<CartVM>
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //get CartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                //remove quantity
                if (model.Quantity > 1)
                    model.Quantity--;
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                //save data
                var result = new { qty = model.Quantity, price = model.Price };

                //return json with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //GET:/cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            // declare List<CartVM>
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                //get CartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                cart.Remove(model);
            }
        }
    }
}