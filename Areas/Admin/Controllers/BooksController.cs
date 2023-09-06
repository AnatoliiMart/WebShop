using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Books;

namespace WebShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        // GET: Admin/Books
        [HttpGet]
        public ActionResult Index()
        {
            // Create BookVM List
            List<BookVM> booksList;

            // Innit list From DB
            using (DB db = new DB())
            {
                booksList = db.Books.
                            ToArray().
                            OrderBy(x => (x.Title)).
                            Select(x => new BookVM(x)).
                            ToList();
                foreach (var item in booksList)
                {
                    if (db.Authors.Find(item.AuthorId) == null)
                        item.AuthorName = "NoAuthor";
                    else
                        item.AuthorName = db.Authors.Find(item.AuthorId).FirstName + " " + db.Authors.Find(item.AuthorId).LastName;

                    if (db.Presses.Find(item.PressId) == null)
                        item.PressName = "NoPress";
                    else
                        item.PressName = db.Presses.Find(item.PressId).PressName;

                    if (db.Genres.Find(item.GenreId) == null)
                        item.GenreName = "NoGenre";
                    else
                        item.GenreName = db.Genres.Find(item.GenreId).GenreName;
                }
            }
            // returns List of imagines 
            return View(booksList);
        }

        // GET: Admin/Books/AddBook
        [HttpGet]
        public ActionResult AddBook()
        {
            BookVM model = new BookVM();
            using (DB db = new DB())
            {
                foreach (var item in db.Authors)
                    item.FullName = item.FirstName + " " + item.LastName;

                model.Authors = new SelectList(db.Authors.OrderBy(x => x.FirstName).ToList(), "Id", "FullName");
                model.Genres = new SelectList(db.Genres.OrderBy(x => x.GenreName).ToList(), "Id", "GenreName");
                model.Presses = new SelectList(db.Presses.OrderBy(x => x.PressName).ToList(), "Id", "PressName");
            }
            return View(model);
        }

        // POST: Admin/Books/AddBook
        [HttpPost]
        public ActionResult AddBook(BookVM model)
        {


            using (DB db = new DB())
            {
                //Create variable for SLUG
                string nameTmp = string.Empty;
                string surnameTmp = string.Empty;

                //Innit MDL classes

                BookMDL book = new BookMDL();

                AuthorMDL author = db.Authors.FirstOrDefault(x => x.Id == model.AuthorId);
                if (author != null)
                    book.Author = author;

                PressMDL press = db.Presses.FirstOrDefault(x => x.Id == model.PressId);
                if (press != null)
                    book.Press = press;

                GenreMDL genre = db.Genres.FirstOrDefault(x => x.Id == model.GenreId);
                if (genre != null)
                    book.Genre = genre;

                string titleTmp = string.Empty;
                int publicationYear = 0;

                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    ModelState.AddModelError("", "Title field is empty!!!");
                    foreach (var item in db.Authors)
                        item.FullName = item.FirstName + " " + item.LastName;

                    model.Authors = new SelectList(db.Authors.OrderBy(x => x.FirstName).ToList(), "Id", "FullName");
                    model.Genres = new SelectList(db.Genres.OrderBy(x => x.GenreName).ToList(), "Id", "GenreName");
                    model.Presses = new SelectList(db.Presses.OrderBy(x => x.PressName).ToList(), "Id", "PressName");
                    return View(model);
                }
                titleTmp = model.Title;

                if (string.IsNullOrWhiteSpace(model.PublicationYear.ToString()))
                {
                    ModelState.AddModelError("", "Publication year field is empty!!!");
                    foreach (var item in db.Authors)
                        item.FullName = item.FirstName + " " + item.LastName;

                    model.Authors = new SelectList(db.Authors.OrderBy(x => x.FirstName).ToList(), "Id", "FullName");
                    model.Genres = new SelectList(db.Genres.OrderBy(x => x.GenreName).ToList(), "Id", "GenreName");
                    model.Presses = new SelectList(db.Presses.OrderBy(x => x.PressName).ToList(), "Id", "PressName");
                    return View(model);
                }
                else if (model.PublicationYear <= 0 || model.PublicationYear > DateTime.Now.Year)
                {
                    ModelState.AddModelError("", "Incorrect publication year!!!");
                    foreach (var item in db.Authors)
                        item.FullName = item.FirstName + " " + item.LastName;

                    model.Authors = new SelectList(db.Authors.OrderBy(x => x.FirstName).ToList(), "Id", "FullName");
                    model.Genres = new SelectList(db.Genres.OrderBy(x => x.GenreName).ToList(), "Id", "GenreName");
                    model.Presses = new SelectList(db.Presses.OrderBy(x => x.PressName).ToList(), "Id", "PressName");
                    return View(model);
                }
                publicationYear = model.PublicationYear;


                if (db.Books.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "This book already exists!");
                    foreach (var item in db.Authors)
                        item.FullName = item.FirstName + " " + item.LastName;

                    model.Authors = new SelectList(db.Authors.OrderBy(x => x.FirstName).ToList(), "Id", "FullName");
                    model.Genres = new SelectList(db.Genres.OrderBy(x => x.GenreName).ToList(), "Id", "GenreName");
                    model.Presses = new SelectList(db.Presses.OrderBy(x => x.PressName).ToList(), "Id", "PressName");

                    return View(model);
                }

                // Wrote left var's in DTO
                book.Title = titleTmp;
                book.PublicationYear = publicationYear;
                book.AuthorId = model.AuthorId;
                book.GenreId = model.GenreId;
                book.PressId = model.PressId;

                // Save model in DB
                db.Books.Add(book);
                db.SaveChanges();
            }

            // Push message through TempData
            TempData["SM"] = "Book was added successfully";

            // Redirecting user on INDEX method
            return RedirectToAction("Index");
        }

      
        // GET: Admin/Book/BookDetails/Id
        [HttpGet]
        public ActionResult BookDetails(int id)
        {
            //Create model(BookVm)
            BookVM model;
            using (DB db = new DB())
            {
                //Take author from DB

                BookMDL book = db.Books.Find(id);
                //Check access to book
                if (book == null)
                    return Content("This book is not available!");

                //Take model from DB
                model = new BookVM(book);


                if (db.Authors.Find(model.AuthorId) == null)
                    model.AuthorName = "NoAuthor";
                else
                    model.AuthorName = model.AuthorName = db.Authors.Find(model.AuthorId).FirstName + " " + db.Authors.Find(model.AuthorId).LastName;

                if (db.Presses.Find(book.PressId) == null)
                    model.PressName = "NoPress";
                else
                    model.PressName = db.Presses.Find(book.PressId).PressName;

                if (db.Genres.Find(book.GenreId) == null)
                    model.GenreName = "NoGenre";
                else
                    model.GenreName = db.Genres.Find(book.GenreId).GenreName;
            }

            //Return model into View
            return View(model);
        }

        // GET: Admin/Books/DeleteBook/Id
        [HttpGet]
        public ActionResult DeleteBook(int id)
        {
            using (DB db = new DB())
            {
                //Take book
                BookMDL book = db.Books.Find(id);

                //Remove book 
                db.Books.Remove(book);

                //Save changes
                db.SaveChanges();
            }

            //Push message through TempData
            TempData["SM"] = "Book was successfully deleted";

            //Redirecting to INDEX
            return RedirectToAction("Index");
        }

    }
}