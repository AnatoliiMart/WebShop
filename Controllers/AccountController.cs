using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Account;
using WebShop.Models.ViewModels.Shop;

namespace WebShop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        //GET: Account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount() => View("CreateAccount");

        //POST: Account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //Check model on valid 
            if (!ModelState.IsValid)
                return View("CreateAccount", model);

            //Check password
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match!");
                return View("CreateAccount", model);
            }
            using (DB db = new DB())
            {
                //Check name on unique
                if (db.Users.Any(x => x.Login.Equals(model.Login)))
                {
                    ModelState.AddModelError("", $"Login {model.Login} is taken!");
                    model.Login = string.Empty;
                    return View("CreateAccount", model);
                }

                //Create Data model
                UserMDL user = new UserMDL()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Login = model.Login,
                    Password = model.Password
                };

                //Add data to model
                db.Users.Add(user);
                db.SaveChanges();

                //Add role for user 
                int id = user.Id;
                UserRoleMDL userRole = new UserRoleMDL() { UserId = id, RoleId = 2 };
                db.UserRoles.Add(userRole);

                //Save data
                db.SaveChanges();
            }

            //Send message through TempData
            TempData["SM"] = "You are now registered and can login";

            //Redirect to action 
            return RedirectToAction("Login");
        }

        //GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            //Confirm what user is not authorized
            string userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");

            //return View
            return View();
        }

        //POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //Check model valid
            if (!ModelState.IsValid)
                return View(model);

            //Check user valid
            bool isValid = false;
            using (DB db = new DB())
            {
                if (db.Users.Any(x => x.Login.Equals(model.Login) && x.Password.Equals(model.Password)))
                    isValid = true;

                if (!isValid)
                {
                    ModelState.AddModelError("", "Incorrect username or password!");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Login, model.RememberMe));
                }
            }
        }

        //GET: /account/logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult UserNavPart()
        {
            //Take User Name
            string userName = User.Identity.Name;
            //Innit model
            UserNavPartVM model;
            //Take user
            using (DB db = new DB())
            {
                //Full model by data 
                UserMDL user = db.Users.FirstOrDefault(x => x.Login == userName);
                model = new UserNavPartVM() { FirstName = user.FirstName, LastName = user.LastName };
            }
            //Return part of view with model
            return PartialView(model);
        }

        //GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            //Take username
            string userName = User.Identity.Name;
            //Create model
            UserProfileVM model;

            using (DB db = new DB())
            {
                //Take user from db
                UserMDL user = (db.Users.FirstOrDefault(x => x.Login == userName));
                //Innit model by data 
                model = new UserProfileVM(user);
            }
            //Return view with model
            return View("UserProfile", model);
        }

        //POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;

            //Check model validity
            if (!ModelState.IsValid)
                return View("UserProfile", model);

            //Check password (if it changes)
            if (!string.IsNullOrWhiteSpace(model.Password))
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not matches");
                    return View("UserProfile", model);
                }

            using (DB db = new DB())
            {
                //Take username
                string userName = User.Identity.Name;

                //Check on userName change
                if (userName != model.Login)
                {
                    userName = model.Login;
                    userNameIsChanged = true;
                }
                //Check name on unique
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Login == userName))
                {
                    ModelState.AddModelError("", $"Login {model.Login} already exists");
                    model.Login = string.Empty;
                    return View("UserProfile", model);
                }
                //Change MDL data
                UserMDL user = db.Users.Find(model.Id);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.EmailAdress = model.EmailAdress;
                user.Login = model.Login;

                if (!string.IsNullOrWhiteSpace(model.Password))
                    user.Password = model.Password;

                //Save changes
                db.SaveChanges();
            }
            //Push message through TempData
            TempData["SM"] = "You have edited your profile";

            if (!userNameIsChanged)
            {
                //Return View with model
                return View("UserProfile", model);
            }
            else
                return RedirectToAction("Logout");

        }

        //GET: /account/Orders
        [Authorize(Roles = "User")]
        public ActionResult Orders()
        {
            //innit OrdersForUserVM
            List<OrdersForUserVM> ordersForUsers = new List<OrdersForUserVM>();

            using (DB db = new DB())
            {
                //take user Id
                UserMDL user = db.Users.FirstOrDefault(x => x.Login == User.Identity.Name);
                int userId = user.Id;

                //Innit OrderVM model
                List<OrderVM> orders = db.Orders
                                         .ToArray()    
                                         .Where(x => x.UserId == userId)
                                         .Select(x => new OrderVM(x))
                                         .ToList();

                //Check product list in OrderVM
                foreach (var order in orders)
                {
                    //Innit Dict of products
                    Dictionary<string, int> prodsAndQty = new Dictionary<string, int>();

                    //Innit total price 
                    decimal total = 0m;

                    //Innit OrderDetailsMDL list
                    List<OrderDetailsMDL> ordersDetails = db.OrdersDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    //Check OrderDetailsMDL list
                    foreach (var orderDetails in ordersDetails)
                    {
                        //Take product
                        ProductMDL product = db.Products.FirstOrDefault(x => x.Id == orderDetails.ProductId);
                        //take prod price
                        decimal price = (decimal)product.Price;

                        //Take prod Name 
                        string prodName = product.Title;

                        //Add prod to dictionary
                        prodsAndQty.Add(prodName, orderDetails.Quantity);
                        //Take total cost
                        total += price * orderDetails.Quantity;
                    }
                    //Add data to model OrdersForUserVM
                    ordersForUsers.Add(new OrdersForUserVM()
                    {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = prodsAndQty,
                        OrderDate = order.OrderDate
                    });
                }
            }

            //Return View with model
            return View(ordersForUsers);
        }
    }
}