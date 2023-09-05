using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Books;

namespace WebShop.Areas.Admin.Controllers
{
    public class PressesController : Controller
    {
        // GET: Admin/Presses
        [HttpGet]
        public ActionResult Index()
        {
            // Create PressVM List
            List<PressVM> pressesList;
            // Innit list From DB
            using (DB db = new DB())
                pressesList = db.Presses.
                    ToArray().
                    OrderBy(x => (x.PressName)).
                    Select(x => new PressVM(x)).
                    ToList();

            // returns List of imagines 
            return View(pressesList);
        }

        // POST: Admin/Presses/AddPress
        [HttpGet]
        public ActionResult AddPress() => View();

        // POST: Admin/Presses/AddPress
        [HttpPost]
        public ActionResult AddPress(PressVM model)
        {
            //Check model validity
            if (!ModelState.IsValid)
                return View(model);

            using (DB db = new DB())
            {
                //Create variable for PressName
                string nameTmp = string.Empty;


                //Innit PressMDL class
                PressMDL press = new PressMDL();

                //Check Name on existing in Model if NO take it 
                if (string.IsNullOrWhiteSpace(model.PressName))
                    ModelState.AddModelError("", "Field is empty!!!");
                else
                    nameTmp = model.PressName;

                //Check What Name is unique
                if (db.Presses.Any(x => x.PressName == model.PressName))
                {
                    ModelState.AddModelError("", "This press already exists!");
                    return View(model);
                }

                // Wrote left var's in DTO
                press.PressName = nameTmp;

                // Save model in DB
                db.Presses.Add(press);
                db.SaveChanges();
            }

            // Push message through TempData
            TempData["SM"] = "Press was added successfully";

            // Redirecting user on INDEX method
            return RedirectToAction("Index");
        }

        // GET: Admin/Presses/EditPress/Id
        [HttpGet]
        public ActionResult EditPress(int id)
        {
            //Create model(PageVM)
            PressVM model;
            //Take that 
            using (DB db = new DB())
            {
                PressMDL press = db.Presses.Find(id);

                //Check if the page is available 
                if (press == null)
                    return Content("The page does not exists!");

                //Innit model by data 
                model = new PressVM(press);
            }

            //return view with model for client
            return View(model);
        }

        // POST: Admin/Presses/EditPress
        [HttpPost]
        public ActionResult EditPress(PressVM model)
        {
            //Check model validity
            if (!ModelState.IsValid)
                return View(model);

            string nameTmp = string.Empty;

            using (DB db = new DB())
            {

                // Take press by ID
                PressMDL press = db.Presses.Find(model.Id);

                // Take Name from MODEL to DTO
                press.PressName = model.PressName;

                // Check Name and take that if needed 

                if (string.IsNullOrWhiteSpace(model.PressName))
                    ModelState.AddModelError("", "Field is empty!!!");
                else
                    nameTmp = model.PressName;

                // Check Name on unique 
                if (db.Presses.Where(x => x.Id != model.Id).Any(x => x.PressName == model.PressName))
                {
                    ModelState.AddModelError("", "That Author already exists!");
                    return View(model);
                }
                // Wrote left var's in DTO
                press.PressName = nameTmp;

                // Save changes
                db.SaveChanges();
            }

            //Push message through TempData
            TempData["SM"] = "You have edited this press";

            //Redirecting on EditPress
            return RedirectToAction("EditPress");
        }

        // GET: Admin/Presses/PressDetails/Id
        [HttpGet]
        public ActionResult PressDetails(int id)
        {
            //Create model(PressVM)
            PressVM model;
            using (DB db = new DB())
            {
                //Take press from DB

                PressMDL press = db.Presses.Find(id);

                //Check access to author
                if (press == null)
                    return Content("This page is not available!");

                //Take model from DB
                model = new PressVM(press);
            }

            //Return model into View
            return View(model);
        }

        // GET: Admin/Presses/DeletePress/Id
        [HttpGet]
        public ActionResult DeletePress(int id)
        {
            using (DB db = new DB())
            {
                //Take author
                PressMDL press = db.Presses.Find(id);

                //Remove author 
                db.Presses.Remove(press);

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