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
    }
}