using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Cart;

namespace WebShop.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart/Index
        [HttpGet]
        public ActionResult Index()
        {
            //Innit List CartVM and check session
            List<CartVM> list = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //Check cart OnEmpty
            if (list.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Cart is empty";
                return View();
            }
            //If not empty send through ViewBag
            decimal total = 0m;
            foreach (var item in list)
                total += item.Total;

            ViewBag.GrandTotal = total;
            //Return list in View
            return View(list);
        }

        // GET: Cart/CartPart
        public ActionResult CartPart()
        {
            //Create model CartVM
            CartVM model = new CartVM();
            //Create var quantity
            int qty = 0;
            //Create var price
            decimal prc = 0m;

            //Check session on data exists
            if (Session["cart"] != null)
            {
                //Take quantity and price if cart not empty
                List<CartVM> list = Session["cart"] as List<CartVM>;
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    prc += item.Quantity * item.Price;
                }

                model.Quantity = qty;
                model.Price = prc;
                //Or set 0 both
            }
            else
            {
                model.Quantity = 0;
                model.Price = 0m;
            }

            //Return part view with model
            return PartialView("CartPart", model);
        }

        public ActionResult AddToCartPart(int id)
        {
            //Create list CartVM 
            List<CartVM> list = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            //CreateModel
            CartVM model = new CartVM();


            using (DB db = new DB())
            {
                //TakeProduct
                ProductMDL product = db.Products.Find(id);
                //Check if product is in cart
                CartVM productInCart = list.FirstOrDefault(x => x.ProductId == id);
                //If no add prod to cart
                if (productInCart == null)
                {
                    list.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Title,
                        Quantity = 1,
                        Price = (decimal)product.Price,
                        Image = product.ImageName
                    });
                    //Else increment quantity
                }
                else
                    productInCart.Quantity++;
            }
            //Take all quantity and price and add it to model
            int qty = 0;
            decimal price = 0m;
            foreach (var item in list)
            {
                price += item.Price * item.Quantity;
                qty += item.Quantity;
            }
            model.Quantity = qty;
            model.Price = price;
            //Save session changes 
            Session["cart"] = list;
            //Return part with model 
            return PartialView("AddToCartPart", model);
        }

        //GET: Cart/IncrementProduct
        public JsonResult IncrementProduct(int productId)
        {
            //Create list CartVM
            List<CartVM> list = Session["cart"] as List<CartVM>;


            using (DB db = new DB())
            {
                //Get CartVM from list
                CartVM model = list.FirstOrDefault(x => x.ProductId == productId);

                //Increment quantity
                model.Quantity++;

                //Save Session
                var res = new { qty = model.Quantity, price = model.Price };
                //Return JSON callback with data 
                return Json(res, JsonRequestBehavior.AllowGet);
            }

        }

        //GET: Cart/DecrementProduct

        public ActionResult DecrementProduct(int productId)
        {
            //Create list CartVM
            List<CartVM> list = Session["cart"] as List<CartVM>;


            using (DB db = new DB())
            {
                //Get CartVM from list
                CartVM model = list.FirstOrDefault(x => x.ProductId == productId);

                //Decrement quantity
                if (model.Quantity > 1)
                    model.Quantity--;
                else
                {
                    model.Quantity = 0;
                    list.Remove(model);
                }


                //Save Session
                var res = new { qty = model.Quantity, price = model.Price };
                //Return JSON callback with data 
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }

        //GET: Cart/RemoveProduct
        public void RemoveProduct(int productId)
        {
            //Create list CartVM
            List<CartVM> list = Session["cart"] as List<CartVM>;

            using (DB db = new DB())
            {
                //Get CartVM from list
                CartVM model = list.FirstOrDefault(x => x.ProductId == productId);

                list.Remove(model);
            }
        }

        //POST: Cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            //Take list of products in cart
            List<CartVM> listCart = Session["cart"] as List<CartVM>;

            //Take user name 
            string userName = User.Identity.Name;

            //Innit var for OrderId
            int orderId = 0;

            using (DB db = new DB())
            {
                //Create and innit model by data 
                OrderMDL order = new OrderMDL();

                //Take UserId
                UserMDL user = db.Users.FirstOrDefault(x => x.Login == userName);
                int userId = user.Id;

                //Full data model and save data 
                order.UserId = userId;
                order.OrderDate = DateTime.Now;

                db.Orders.Add(order);
                db.SaveChanges();

                //Take OrderId
                orderId = order.OrderId;

                //Innit OrderDetails model 
                OrderDetailsMDL orderDetails = new OrderDetailsMDL();

                //Add data in model
                foreach (var item in listCart)
                {
                    orderDetails.OrderId = orderId;
                    orderDetails.UserId = userId;
                    orderDetails.ProductId = item.ProductId;
                    orderDetails.Quantity = item.Quantity;
                    db.OrdersDetails.Add(orderDetails);
                    db.SaveChanges();
                }
            }

            //Send message for order on admin EMail USING MAILTRAP FOR Tests 
            //Code generated authomaticly
            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("1002046380b92b", "9ced3d7182b04e"),
                EnableSsl = true
            };
            client.Send("Shop@example.com", "toAdminOrder@example.com", "NEW ORDER", $"You have new order number: {orderId}");

            //Make cart session NULL because be Exeptions
            Session["cart"] = null;
        }
    }
}