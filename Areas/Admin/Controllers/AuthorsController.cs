using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Books;

namespace WebShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuthorsController : Controller
    {
        // GET: Admin/Authors
        [HttpGet]
        public ActionResult Index()
        {
            // Create AuthorVM List
            List<AuthorVM> authorsList;

            // Innit list From DB
            using (DB db = new DB())
                authorsList = db.Authors.
                              ToArray().
                              OrderBy(x => (x.FirstName)).
                              Select(x => new AuthorVM(x)).
                              ToList();

            // returns List of imagines 
            return View(authorsList);
            
        }

        // POST: Admin/Authors/AddAuthor
        [HttpGet]
        public ActionResult AddAuthor() => View();

        // POST: Admin/Authors/AddAuthor
        [HttpPost]
        public ActionResult AddAuthor(AuthorVM model) 
        {
            //Check model validity
            if (!ModelState.IsValid)
                return View(model);

            using (DB db = new DB())
            {
                //Create variable for SLUG
                string nameTmp = string.Empty;
                string surnameTmp = string.Empty;

                //Innit AuthorMDL class
                AuthorMDL author = new AuthorMDL();

                //Check Name and Surname on existing in Model if NO take it 
                if (string.IsNullOrWhiteSpace(model.FirstName))
                    ModelState.AddModelError("", "Field is empty!!!");
                else
                    nameTmp = model.FirstName;

                if (string.IsNullOrWhiteSpace(model.LastName)) 
                    ModelState.AddModelError("", "Field is empty!!!");
                else
                {
                    surnameTmp = model.LastName;
                }
                
                //Check What Name and Surname is unique
                if (db.Authors.Any(x => x.FirstName == model.FirstName) &&
                    db.Authors.Any(x => x.LastName == model.LastName))
                {
                    ModelState.AddModelError("", "This author already exists!");
                    return View(model);
                }

                // Wrote left var's in DTO
                author.FirstName = nameTmp;
                author.LastName = surnameTmp;

                // Save model in DB
                db.Authors.Add(author);
                db.SaveChanges();
            }

            // Push message through TempData
            TempData["SM"] = "Author was added successfully";

            // Redirecting user on INDEX method
            return RedirectToAction("Index");
        }

        // GET: Admin/Authors/EditAuthor/Id
        [HttpGet]
        public ActionResult EditAuthor(int id)
        {
            //Create model(AuthorVM)
            AuthorVM model;

            //Take that 
            using (DB db = new DB())
            {
                AuthorMDL author = db.Authors.Find(id);

                //Check if the author is available 
                if (author == null)
                    return Content("The author does not exists!");

                //Innit model by data 
                model = new AuthorVM(author);
            }

            //return view with model for client
            return View(model);
        }

        // POST: Admin/Authors/EditAuthor
        [HttpPost]
        public ActionResult EditAuthor(AuthorVM model)
        {
            //Check model validity
            if (!ModelState.IsValid)
                return View(model);

            string nameTmp = string.Empty;
            string surnameTmp = string.Empty;

            using (DB db = new DB())
            {

                // Take Author by ID
                AuthorMDL author = db.Authors.Find(model.Id);

                // Check Name and Surname and take that if needed 
                if (string.IsNullOrWhiteSpace(model.FirstName))
                    ModelState.AddModelError("", "Field is empty!!!");
                else
                    nameTmp = model.FirstName;

                if (string.IsNullOrWhiteSpace(model.LastName))
                    ModelState.AddModelError("", "Field is empty!!!");
                else
                {
                    surnameTmp = model.LastName;
                }

                // Check Name and Surname on unique 
                if (db.Authors.Where(x => x.Id != model.Id).Any(x => x.FirstName == model.FirstName) &&
                    db.Authors.Where(x => x.Id != model.Id).Any(x => x.LastName == model.LastName))
                {
                    ModelState.AddModelError("", "That author already exists!");
                    return View(model);
                }
                // Wrote left var's in DTO
                author.FirstName = nameTmp;
                author.LastName = surnameTmp;

                // Save changes
                db.SaveChanges();
            }

            //Push message through TempData
            TempData["SM"] = "You have edited this author";

            //Redirecting on EditAuthor
            return RedirectToAction("EditAuthor");
        }

        // GET: Admin/Authors/AuthorDetails/Id
        [HttpGet]
        public ActionResult AuthorDetails(int id)
        {
            //Create model(AuthorVm)
            AuthorVM model;
            using (DB db = new DB())
            {
                //Take author from DB

                AuthorMDL author = db.Authors.Find(id);

                //Check access to author
                if (author == null)
                    return Content("This author is not available!");

                //Take model from DB
                model = new AuthorVM(author);
            }

            //Return model into View
            return View(model);
        }

        // GET: Admin/Authors/DeleteAuthor/Id
        [HttpGet]
        public ActionResult DeleteAuthor(int id)
        {
            using (DB db = new DB())
            {
                //Take author
                AuthorMDL author = db.Authors.Find(id);
                //Remove author 
                db.Authors.Remove(author);
                //Save changes
                db.SaveChanges();
            }
            //Push message through TempData
            TempData["SM"] = "Author was successfully deleted";
            //Redirecting to INDEX
            return RedirectToAction("Index");
        }

    }
}