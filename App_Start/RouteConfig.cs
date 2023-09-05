using System.Web.Mvc;
using System.Web.Routing;

namespace WebShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
          

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                new[] { "WebShop.Controllers" });

            routes.MapRoute("SidebarPart", "Pages/SidebarPart", new { controller = "Pages", action = "SidebarPart" },
               new[] { "WebShop.Controllers" });

            routes.MapRoute("Shop", "Shop/{action}/{name}", new { controller = "Shop", action = "Index", name = UrlParameter.Optional },
                new[] { "WebShop.Controllers" });

            routes.MapRoute("PagesMenuPart", "Pages/PagesMenuPart", new { controller = "Pages", action = "PagesMenuPart" },
                new[] { "WebShop.Controllers" });
           
            routes.MapRoute("Pages", "{page}", new { controller = "Pages", action = "Index" },
                new[] { "WebShop.Controllers" });

            routes.MapRoute("Default", "", new { controller = "Pages", action = "Index" },
                new[] { "WebShop.Controllers" });

            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional },
              new[] { "WebShop.Controllers" });



        }
    }
}
