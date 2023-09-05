using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Pages;

namespace WebShop.Controllers
{
    public class PagesController : Controller
    {
        // GET: Page/Index/page
        [HttpGet]
        public ActionResult Index(string page = "")
        {
            //Set var page is empty or take that if he exists
            if (page == string.Empty)
                page = "home";



            //Create model and data
            PageVM model;
            PageMDL _page;

            //Check what current page is aviliable 
            using (DB db = new DB())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                    return RedirectToAction("Index", new { page = "" });
                //Take MDL of this page
                _page = db.Pages.Where(x => x.Slug == page).FirstOrDefault();

                //Set title  of page
                ViewBag.PageTitle = _page.Title;

                //Check on existing sidebar 
                if (_page.HasSidebar)
                    ViewBag.Sidebar = "Yes";
                else
                    ViewBag.Sidebar = "No";

                // Full model by data 
                model = new PageVM(_page);
            }
            //Retutrn View with model
            return View(model);
        }

        // GET: Page/PagesMenuPart
        public ActionResult PagesMenuPart()
        {
            //Innit List of VM
            List<PageVM> pages = new List<PageVM>();

            using (DB db = new DB())
            {
                //Take all pages without HOME
                pages = db.Pages
                          .ToArray()
                          .OrderBy(x => x.Sorting)
                          .Where(x => x.Slug != "home")
                          .Select(x => new PageVM(x)).ToList();
            }

            // Return part of imagine with data 
            return PartialView("PagesMenuPart", pages);
        }

        // GET: Page/SidebarPart
        public ActionResult SidebarPart()
        {
            SidebarVM model;
            //Create and innit model
            using (DB db = new DB())
            {
                SidebarMDL sidebar = db.Sidebars.Find(1);
                model = new SidebarVM(sidebar);
            }
            //Rerturn View with part
            return PartialView("SidebarPart", model);

        }
    }
}