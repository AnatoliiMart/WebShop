using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Shop;

namespace WebShop.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop/Index
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        // GET: Shop/CategoryMenuPart
        public ActionResult CategoryMenuPart()
        {
            //Innit list CategoryVM
            List<CategoryVM> categories;
            //Innit model dy data
            using(DB db = new DB())
            {
                categories = db.Categories
                               .ToArray()
                               .OrderBy(x => x.Sorting)
                               .Select(x => new CategoryVM(x))
                               .ToList();
            }
            //Return part View with model
            return PartialView("CategoryMenuPart", categories);
        }

        // GET: Shop/Category/name
        public ActionResult Category(string name) 
        {
            //create list ProductVM
            List<ProductVM> products;

            using (DB db = new DB())
            {
                //Take id of category
                CategoryMDL category = db.Categories.Where(x => x.Slug == name).FirstOrDefault();
                int catId = category.Id;
                //Innit list by Data
                products = db.Products.ToArray()
                             .Where(x => x.CategoryId == catId)
                             .Select(x => new ProductVM(x))
                             .ToList();

                //Take name of Category
                ProductMDL productCategory = db.Products.Where(x => x.CategoryId == catId).FirstOrDefault();

                //Check what category not null
                if (productCategory == null)
                {
                    string catName = db.Categories.Where(x => x.Slug == name).Select(x => x.Name).FirstOrDefault();
                    ViewBag.CategoryName = catName;
                }
                else
                    ViewBag.CategoryName = productCategory.CategoryName;
            }

            //Return View with model
            return View(products);
        }

        // GET: Shop/product-details/name
        [HttpGet]
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            //Create model and Data model
            ProductVM model;
            ProductMDL product;
            string newName = name.Replace('-', ' ');
            //Innit prod Id
            int productId = 0;

            //check what product exists 
            using (DB db = new DB())
            {
                //Innit datamodel dy data
                if (!db.Products.Any(x=>x.Title.ToLower().Equals(newName)))
                    return RedirectToAction("Index", "Shop");
                product = db.Products.Where(x=> x.Title.ToLower() == newName).FirstOrDefault();
                //Take prod Id
                productId = product.Id;
                //Innit VM by data 
                model = new ProductVM(product);
            }
            //Take gallery images
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath($"~/Images/Uploads/Products/{productId}/Gallery/Thumbs"))
                                                           .Select(fileName => Path.GetFileName(fileName));
            //Returm View with model
            return View("ProductDetails", model);
        }
    }
}