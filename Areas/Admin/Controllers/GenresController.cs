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
    public class GenresController : Controller
    {
        // GET: Admin/Genres
        [HttpGet]
        public ActionResult Index()
        {
            // Create AuthorVM List
            List<GenreVM> genresList;
            // Innit list From DB
            using (DB db = new DB())
                genresList = db.Genres.
                    ToArray().
                    OrderBy(x => (x.GenreName)).
                    Select(x => new GenreVM(x)).
                    ToList();

            // returns List of imagines 
            return View(genresList);
        }

        // POST: Admin/Genres/AddGenre
        [HttpGet]
        public ActionResult AddGenre() => View();

        // POST: Admin/Genres/AddGenre
        [HttpPost]
        public ActionResult AddGenre(GenreVM model)
        {
            //Check model validity
            if (!ModelState.IsValid)
                return View(model);

            using (DB db = new DB())
            {
                //Create variable for GenreName
                string nameTmp = string.Empty;


                //Innit GenreMDL class
                GenreMDL genre = new GenreMDL();

                //Check Name on existing in Model if NO take it 
                if (string.IsNullOrWhiteSpace(model.GenreName))
                    ModelState.AddModelError("", "Field is empty!!!");
                else
                    nameTmp = model.GenreName;

                //Check What Name is unique
                if (db.Genres.Any(x => x.GenreName == model.GenreName))
                {
                    ModelState.AddModelError("", "This genre already exists!");
                    return View(model);
                }

                // Wrote left var's in DTO
                genre.GenreName = nameTmp;

                // Save model in DB
                db.Genres.Add(genre);
                db.SaveChanges();
            }

            // Push message through TempData
            TempData["SM"] = "Genre was added successfully";

            // Redirecting user on INDEX method
            return RedirectToAction("Index");
        }

        // GET: Admin/Genres/EditGenre/Id
        [HttpGet]
        public ActionResult EditGenre(int id)
        {
            //Create model(GenreVM)
            GenreVM model;

            //Take that 
            using (DB db = new DB())
            {
                GenreMDL genre = db.Genres.Find(id);

                //Check if the genre is available 
                if (genre == null)
                    return Content("The genre does not exists!");

                //Innit model by data 
                model = new GenreVM(genre);
            }

            //return view with model for client
            return View(model);
        }

        // POST: Admin/Genres/EditGenre
        [HttpPost]
        public ActionResult EditGenre(GenreVM model)
        {
            //Check model validity
            if (!ModelState.IsValid)
                return View(model);

            string nameTmp = string.Empty;

            using (DB db = new DB())
            {

                // Take page by ID
                GenreMDL genre = db.Genres.Find(model.Id);

                // Take Name from MODEL to DTO
                genre.GenreName = model.GenreName;

                // Check Name and take that if needed 

                if (string.IsNullOrWhiteSpace(model.GenreName))
                    ModelState.AddModelError("", "Field is empty!!!");
                else
                    nameTmp = model.GenreName;

                // Check Name on unique 
                if (db.Genres.Where(x => x.Id != model.Id).Any(x => x.GenreName == model.GenreName))
                {
                    ModelState.AddModelError("", "That genre already exists!");
                    return View(model);
                }
                // Wrote var in DTO
                genre.GenreName = nameTmp;

                // Save changes
                db.SaveChanges();
            }

            //Push message through TempData
            TempData["SM"] = "You have edited this genre";

            //Redirecting on EditGenre
            return RedirectToAction("EditGenre");
        }

        // GET: Admin/Genres/GenreDetails/Id
        [HttpGet]
        public ActionResult GenreDetails(int id)
        {
            //Create model(AuthorVm)
            GenreVM model;
            using (DB db = new DB())
            {
                //Take genre from DB

                GenreMDL author = db.Genres.Find(id);

                //Check access to genre
                if (author == null)
                    return Content("This genre is not available!");

                //Take model from DB
                model = new GenreVM(author);
            }

            //Return model into View
            return View(model);
        }

        // GET: Admin/Genres/DeleteGenre/Id
        [HttpGet]
        public ActionResult DeleteGenre(int id)
        {
            using (DB db = new DB())
            {
                //Take genre
                GenreMDL genre = db.Genres.Find(id);

                //Remove genre 
                db.Genres.Remove(genre);

                //Save changes
                db.SaveChanges();
            }

            //Push message through TempData
            TempData["SM"] = "Genre was successfully deleted";

            //Redirecting to INDEX
            return RedirectToAction("Index");
        }


    }
}