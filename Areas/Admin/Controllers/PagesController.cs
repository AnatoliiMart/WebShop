using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Pages;

namespace WebShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            // Create PagesVM List
            List<PageVM> pageList;
            // Innit list From DB
            using (DB db = new DB())
                pageList = db.Pages.ToArray().OrderBy(x => (x.Sorting)).Select(x => new PageVM(x)).ToList();
            // returns List of Imagins 
            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage() => View();

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check model validity
            if (!ModelState.IsValid)
                return View(model);
            using (DB db = new DB())
            {
                //Create variable for SLUG
                string slugTmp;

                //Innit PageDTO class
                PageMDL page = new PageMDL();

                //Give SLUG on model
                page.Title = model.Title.ToUpper();

                //Check SLUG on existing in DB if NO take it 
                if (string.IsNullOrWhiteSpace(model.Slug))
                    slugTmp = model.Title.Replace(' ', '-').ToLower();
                else
                    slugTmp = model.Slug.Replace(' ', '-').ToLower();
                //Check What SLUG and TITLE is unique
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exists!");
                    return View(model);
                }
                else if (db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That slug already exists!");
                    return View(model);
                }

                // Wrote left var's in DTO
                page.Slug = slugTmp;
                page.Body = model.Body;
                page.Sorting = 100;
                page.HasSidebar = model.HasSidebar;

                // Save model in DB
                db.Pages.Add(page);
                db.SaveChanges();
            }

            // Push message through TempData
            TempData["SM"] = "You add a new page";

            // Redirecting user on INDEX method
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/EditPage/Id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Create model(PageVM)
            PageVM model;
            //Take that 
            using (DB db = new DB())
            {
                PageMDL page = db.Pages.Find(id);

                //Check if the page is available 
                if (page == null)
                    return Content("The page does not exists!");

                //Innit model by data 
                model = new PageVM(page);
            }

            //return view with model for client
            return View(model);
        }

        // POST: Admin/Pages/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Check model validity
            if (!ModelState.IsValid)
                return View(model);

            using (DB db = new DB())
            {
                // Create variable for SLUG 
                string slugTmp = "home";

                // Take page by ID
                PageMDL page = db.Pages.Find(model.Id);

                // Take TITLE from MODEL to DTO
                page.Title = model.Title;

                // Check SLUG and take that if needed 
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                        slugTmp = model.Title.Replace(" ", "-").ToLower();
                    else
                        slugTmp = model.Slug.Replace(" ", "-").ToLower();
                }

                // Check SLUG and TITLE on unique 
                if (db.Pages.Where(x => x.Id != model.Id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That title already exists!");
                    return View(model);
                }
                else if (db.Pages.Where(x => x.Id != model.Id).Any(x => x.Slug == slugTmp))
                {
                    ModelState.AddModelError("", "That slug already exists!");
                    return View(model);
                }
                // Wrote left var's in DTO
                page.Slug = slugTmp;
                page.Body = model.Body;
                page.HasSidebar = model.HasSidebar;

                // Save changes
                db.SaveChanges();
            }

            //Push message through TempData
            TempData["SM"] = "You have edited this page";

            //Redirecting on EDITpAGE
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/Id
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            //Create model(PageVM)
            PageVM model;
            using (DB db = new DB())
            {
                //Take page from DB

                PageMDL page = db.Pages.Find(id);

                //Check access to page
                if (page == null)
                {
                    return Content("This page is not available!");
                }
                //Take model from DB
                model = new PageVM(page);
            }

            //Return model into View
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/Id
        [HttpGet]
        public ActionResult DeletePage(int id)
        {
            using (DB db = new DB())
            {
                //Take page
                PageMDL page = db.Pages.Find(id);
                //Remove page 
                db.Pages.Remove(page);
                //Save changes
                db.SaveChanges();
            }
            //Push message through TempData
            TempData["SM"] = "Page was succesfully deleted";
            //Redirecting to INDEX
            return RedirectToAction("Index");
        }

        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (DB dB = new DB())
            {
                //Create counter 
                int count = 1;
                // Innit model(PagesMDL)
                PageMDL page;

                // Set sorting for each page
                foreach (var item in id)
                {
                    page = dB.Pages.Find(item);
                    page.Sorting = count;
                    dB.SaveChanges();
                    count++;
                }
            }
        }

        // GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Create model
            SidebarVM model;
            using (DB db = new DB())
            {
                //Take data from DTO
                SidebarMDL sidebar = db.Sidebars.Find(1);

                //Full model with data 
                model = new SidebarVM(sidebar);
            }

            //Return View with model
            return View(model);
        }

        // POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (DB db = new DB())
            {
                //Take data from DTO
                SidebarMDL sidebar = db.Sidebars.Find(1);

                //Full sidebar with data 
                sidebar.Body = model.Body;

                //Save changes
                db.SaveChanges();
            }

            //Push message through TempData
            TempData["SM"] = "The Sidebar was successfully edited";

            //Redirect to INDEX
            return RedirectToAction("EditSidebar");
        }

    }
}