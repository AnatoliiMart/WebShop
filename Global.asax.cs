using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebShop.Models.Data;

namespace WebShop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //Method for processing autentification requests (maybe has problems)
        protected void Application_AuthenticateRequest()
        {
            // check what user is utorized
            if (User == null)
                return;
            //Take username 
            string userName = Context.User.Identity.Name;

            //Innit roles arr
            string[] roles = null;

            //Full this arr
            using (DB db = new DB())
            {
                UserMDL user = db.Users.FirstOrDefault(x => x.Login == userName);

                if (user ==null)
                    return;

                roles = db.UserRoles
                          .Where(x => x.UserId == user.Id)
                          .Select(x => x.Role.Name)
                          .ToArray();
            }

            //Create IPrinciple obj - dont know how it's works but Stack Overflow says do it 
            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal userObj = new GenericPrincipal(userIdentity, roles);

            //Create and innit by data User context
            Context.User = userObj;
        }
    }
}
