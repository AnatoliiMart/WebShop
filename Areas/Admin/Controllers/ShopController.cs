using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebShop.Areas.Admin.Models.ViewModels.Shop;
using WebShop.Models.Data;
using WebShop.Models.ViewModels.Shop;

namespace WebShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        [HttpGet]
        public ActionResult Categories()
        {
            //Cteate model with List type
            List<CategoryVM> categories;
            using (DB db = new DB())
            {
                //Innit List by data 
                categories = db.Categories.ToArray()
                             .OrderBy(x => x.Sorting)
                             .Select(x => new CategoryVM(x))
                             .ToList();
            }
            //Return List in View
            return View(categories);
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Create STRING variable Id
            string ID;
            using (DB db = new DB())
            {
                //Check category on unique 
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";
                else
                {
                    //Innit model from DB
                    CategoryMDL model = new CategoryMDL();

                    //Full model by data from model
                    model.Name = catName;
                    model.Slug = catName.Replace(" ", "-").ToLower();
                    model.Sorting = 100;

                    //Save changes
                    db.Categories.Add(model);
                    db.SaveChanges();

                    //Take ID and return it
                    ID = model.Id.ToString();
                }
            }
            return ID;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (DB dB = new DB())
            {
                //Create counter 
                int count = 1;
                // Innit model(CategoryMDL)
                CategoryMDL model;

                // Set sorting for each page
                foreach (var item in id)
                {
                    model = dB.Categories.Find(item);
                    model.Sorting = count;
                    dB.SaveChanges();
                    count++;
                }
            }
        }

        // GET: Admin/Shop/DeleteCategory/Id
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (DB db = new DB())
            {
                //Take category
                CategoryMDL model = db.Categories.Find(id);
                //Remove category
                db.Categories.Remove(model);
                //Save changes
                db.SaveChanges();
            }
            //Push message through TempData
            TempData["SM"] = "Category was succesfully deleted";
            //Redirecting to INDEX
            return RedirectToAction("Categories");
        }

        // POST: Admin/Shop/RenameCategory/Id
        [HttpPost]
        public string RenameCategory(string newCatName, int Id)
        {
            using (DB db = new DB())
            {
                //Check NAME on unique
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";
                else
                {
                    //Innit model(CategoryMDL)
                    CategoryMDL model = db.Categories.Find(Id);

                    //Rename model
                    model.Name = newCatName;
                    model.Slug = newCatName.Replace(" ", "-").ToLower();

                    //Save changes
                    db.SaveChanges();
                }
            }
            //Return something because we need ///titletaken/// on response in JS func ;)))
            return "Fuck JS";
        }

        // GET: Admin/Shop/AddProduct/Id
        [HttpGet]
        public ActionResult AddProduct()
        {
            //Create data model
            ProductVM model = new ProductVM();

            //Innit List of category from DB to model
            using (DB db = new DB())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                model.Books = new SelectList(db.Books.ToList(), "Id", "Title");
            }

            //Returns model 
            return View(model);
        }

        // POST: Admin/Shop/AddProduct/Id
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Check Name on unique
            // Innit var ProductId
            int id;
            using (DB db = new DB())
            {
                if (db.Products.Any(x => x.BookId == model.BookId))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    model.Books = new SelectList(db.Books.ToList(), "Id", "Title");
                    ModelState.AddModelError("", "That product is taken");
                    return View(model);
                }

                //Save model based on ProductMDL
                ProductMDL product = new ProductMDL();
                CategoryMDL category = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                if (category != null)
                    product.CategoryName = category.Name;
                BookMDL book = db.Books.FirstOrDefault(x => x.Id == model.BookId);
                if (book != null)
                {
                    product.Title = book.Title + " " + $"publicated({book.PublicationYear})";
                    product.Author = book.Author.FirstName + " " + book.Author.LastName;
                    product.Press = book.Press.PressName;
                    product.Genre = book.Genre.GenreName;
                }
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.BookId = model.BookId;
                db.Products.Add(product);
                db.SaveChanges();
                id = product.Id;
            }
            //Push message through TempData
            TempData["SM"] = "Product was added successfully";

            /////////////////////////////////////////////////////////////////////
            //Create directory links for images
            DirectoryInfo originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");
            string rootPath = Path.Combine(originalDirectory.ToString(), "Products");
            string eachProductPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            string productThumbsPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            string productGalleryPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            string productGalleryThumbsPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            //Check what created directories is existing(if no - create)
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            if (!Directory.Exists(eachProductPath))
                Directory.CreateDirectory(eachProductPath);

            if (!Directory.Exists(productThumbsPath))
                Directory.CreateDirectory(productThumbsPath);

            if (!Directory.Exists(productGalleryPath))
                Directory.CreateDirectory(productGalleryPath);

            if (!Directory.Exists(productGalleryThumbsPath))
                Directory.CreateDirectory(productGalleryThumbsPath);

            //Check file on IsUpload

            if (file != null && file.ContentLength > 0)
            {
                //Check upload file extension
                string ext = file.ContentType.ToLower();
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/bmp" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (DB db = new DB())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Image wasn't uploaded - wrong extension");
                        return View(model);
                    }
                }

                //Innit  var with name of image 
                string imageName = file.FileName;

                //Save name of image in MDl
                using (DB db = new DB())
                {
                    ProductMDL mdl = db.Products.Find(id);
                    mdl.ImageName = imageName;
                    db.SaveChanges();
                }

                //Give pathes for real and small image 
                string pathOriginal = $"{eachProductPath}\\{imageName}";
                string pathSmall = $"{productThumbsPath}\\{imageName}";

                //Save original image 
                file.SaveAs(pathOriginal);

                //Create and save small image
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1,1);
                img.Save(pathSmall);
            }
            //////////////////////////////////////////////////////////////////////
            //Redirect to Index
            return RedirectToAction("AddProduct");
        }

        // GET: Admin/Shop/ProductList
        [HttpGet]
        public ActionResult ProductList(int? page, int? catId)
        {
            //Create list of ProductVM
            List<ProductVM> prodList = null;

            //Set page number
            int pageNumb = page ?? 1;
            using (DB db = new DB())
            {
                //Innit list and take data 
                prodList = db.Products.ToArray()
                           .Where(x => x.CategoryId == catId || catId == null || catId == 0)
                           .Select(x => new ProductVM(x)).ToList();

                //Full categories by Data
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                //Set selected category
                ViewBag.SelectedCategory = catId.ToString();
            }

            //Set pagination
            var paginate = prodList.ToPagedList(pageNumb, 10);
            ViewBag.paged = paginate;

            //Return view with data
            return View(prodList);
        }

        // GET: Admin/Shop/EditProduct/Id
        [HttpGet]
        public ActionResult EditProduct(int Id)
        {
            //Innit model ProductVM
            ProductVM model;
            using (DB db = new DB())
            {
                //Take Product from DB
                ProductMDL product = db.Products.Find(Id);

                //Check what the product is aviliable
                if (product == null)
                    return Content("That product not exists");

                //Innit model by data
                model = new ProductVM(product);

                //Innit list of categories 
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                model.Books = new SelectList(db.Books.ToList(), "Id", "Title");

                //Take images from gallery
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath($"~/Images/Uploads/Products/{Id}/Gallery/Thumbs/"))
                                       .Select(fileName => Path.GetFileName(fileName));
            }
            //Return model in View
            return View(model);
        }

        // POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            //Get product Id
            int Id = model.Id;

            using (DB db = new DB())
            {
                //Full lists castegories, images and books
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                model.Books = new SelectList(db.Books.ToList(), "Id", "Title");
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath($"~/Images/Uploads/Products/{Id}/Gallery/Thumbs/"))
                                      .Select(fileName => Path.GetFileName(fileName));

                //Check Title on unique
                if (db.Products.Where(x => x.Id != Id).Any(x => x.BookId == model.BookId))
                {
                    ModelState.AddModelError("", "That product is taken");
                    return View(model);
                }
                //Refresh data on db
                ProductMDL product = db.Products.Find(Id);
                CategoryMDL category = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                if (category != null)
                    product.CategoryName = category.Name;
                BookMDL book = db.Books.FirstOrDefault(x => x.Id == model.BookId);
                if (book != null)
                {
                    product.Title = book.Title + " " + $"publicated({book.PublicationYear})";
                    product.Author = book.Author.FirstName + " " + book.Author.LastName;
                    product.Press = book.Press.PressName;
                    product.Genre = book.Genre.GenreName;
                }
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.BookId = model.BookId;
                product.ImageName = model.ImageName;
                db.SaveChanges();
            }

            //Send message through TempData
            TempData["SM"] = "Product was successfully edited";

            //Upload images logic 
            /////////////////////////////////////////////////////////////////////
            //Check what image was uploaded 
            if (file != null && file.ContentLength > 0)
            {
                //Get the file extention
                string ext = file.ContentType.ToLower();

                //Check the file extention
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/bmp" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    ModelState.AddModelError("", "Image wasn't uploaded - wrong extension");
                    return View(model);
                }

                //Set pathes for upload
                DirectoryInfo originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");
                string eachProductPath = Path.Combine(originalDirectory.ToString(), "Products\\" + Id.ToString());
                string productThumbsPath = Path.Combine(originalDirectory.ToString(), "Products\\" + Id.ToString() + "\\Thumbs");

                //Check on FileExists if YES Delete them fnd their folders
                DirectoryInfo directory1 = new DirectoryInfo(eachProductPath);
                DirectoryInfo directory2 = new DirectoryInfo(productThumbsPath);

                foreach (var item in directory1.GetFiles())
                    item.Delete();

                foreach (var item in directory2.GetFiles())
                    item.Delete();

                //Save image 
                string imageName = file.FileName;

                using (DB db = new DB())
                {
                    ProductMDL mdl = db.Products.Find(Id);
                    mdl.ImageName = imageName;
                    db.SaveChanges();
                }

                //Original
                string pathOriginal = $"{eachProductPath}\\{imageName}";
                file.SaveAs(pathOriginal);


                //Preview
                string pathSmall = $"{productThumbsPath}\\{imageName}";
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(pathSmall);
            }
            /////////////////////////////////////////////////////////////////////

            //Redirect to EditProduct
            return RedirectToAction("EditProduct");
        }

        // POST: Admin/Shop/DeleteProduct
        [HttpPost]
        public ActionResult DeleteProduct(int Id)
        {
            //Delete product from DB
            using (DB db = new DB())
            {
                ProductMDL product = db.Products.Find(Id);
                db.Products.Remove(product);
                db.SaveChanges();
            }

            //Delite images with ditectories
            DirectoryInfo originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");
            string eachProductPath = Path.Combine(originalDirectory.ToString(), "Products\\" + Id.ToString());

            if (Directory.Exists(eachProductPath))
                Directory.Delete(eachProductPath, true);

            //Redirect to ProductList
            return RedirectToAction("ProductList");
        }

        //POST: Admin/Shop/GalleryImagesSaving/Id
        [HttpPost]
        public void GalleryImagesSaving(int id)
        {
            //Check all recieved files from VM
            foreach (string fileName in Request.Files)
            {
                //Innit this files
                HttpPostedFileBase file = Request.Files[fileName];

                //Check what that files are not null
                if (file != null && file.ContentLength > 0)
                {
                    //Set pathes for images
                    DirectoryInfo originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");
                    string eachProductPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string productThumbsPath = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    //Set pathes of images
                    string imagePathOriginal = $"{eachProductPath}\\{file.FileName}";
                    string imagePathPrewiew = $"{productThumbsPath}\\{file.FileName}";

                    //Save images
                    //Original
                    file.SaveAs(imagePathOriginal);

                    //Preview
                    WebImage image = new WebImage(file.InputStream);
                    image.Resize(200, 200).Crop(1, 1);
                    image.Save(imagePathPrewiew);
                }
            }
        }

        //POST: Admin/Shop/DeleteImage/id, imageName
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPathOriginal = Request.MapPath($"~/Images/Uploads/Products/{id}/Gallery/{imageName}");
            string fullPathPreview = Request.MapPath($"~/Images/Uploads/Products/{id}/Gallery/Thumbs/{imageName}");

            if (System.IO.File.Exists(fullPathOriginal))
                System.IO.File.Delete(fullPathOriginal);

            if (System.IO.File.Exists(fullPathPreview))
                System.IO.File.Delete(fullPathPreview);
        }


        //GET: Admin/Shop/Orders
        [HttpGet]
        public ActionResult Orders() 
        {
            //Innit OrdersForAdminVM
            List<OrdersForAdminVM> ordersForAdmin = new List<OrdersForAdminVM>();

            using (DB db = new DB())
            {
                //Innit model OrderVM
                List<OrderVM> orders = db.Orders
                                         .ToArray()
                                         .Select(x => new OrderVM(x))
                                         .ToList();
                //Check OrderVM data 
                foreach (var order in orders)
                {
                    //Innit Dict 
                    Dictionary<string, int> prodAndQty = new Dictionary<string, int>();

                    //Create var for total
                    decimal total = 0m;

                    //Innit orderDetailsMDL list
                    List<OrderDetailsMDL> ordersDetails = db.OrdersDetails
                                                            .Where(x => x.OrderId == order.OrderId)
                                                            .ToList();

                    //Take UserName
                    UserMDL user = db.Users.FirstOrDefault(x => x.Id == order.UserId);
                    string userName = user.Login;

                    //Check list orderDetailsMDL 
                    foreach (var orderDetails in ordersDetails)
                    {
                        //Take product
                        ProductMDL product = db.Products.FirstOrDefault(x => x.Id == orderDetails.ProductId);

                        //Take product price
                        decimal price = (decimal)product.Price;

                        //Take product name 
                        string prodName = product.Title;

                        //Add product to dictionary
                        prodAndQty.Add(prodName, orderDetails.Quantity);

                        //Take total cost of all products of this user
                        total += orderDetails.Quantity * price;
                    }

                    //Add data into OrderForAdminVM
                    ordersForAdmin.Add(new OrdersForAdminVM()
                    {
                        OrderNumber = order.OrderId,
                        UserName = userName,
                        Total = total,
                        ProductsAndQty = prodAndQty,
                        OrderDate = order.OrderDate
                    });
                }
            }

            //Return View with model
            return View(ordersForAdmin);
        }
    }
}