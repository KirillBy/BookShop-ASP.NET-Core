using Logic;
using MVC5_Online_shop.Models.Data;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MVC5_Online_shop
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //dependency injection

            var kernel = new StandardKernel(new NinjectSettings { LoadExtensions = true });

            kernel.Load(new LogicDIModule());
        }

        protected void Application_AuthentificateRequest()
        {
            //check if user is authorized
            if(User == null)
            {
                return;
            }

            //get name of user
            string userName = Context.User.Identity.Name;

            //declare roles array
            string[] roles = null;

            using(Db db = new Db())
            {
                //fill roles
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);
                if (dto == null)
                    return;

                roles = db.UserRoles.Where(x => x.UserId == dto.Id).Select(x => x.Role.Name).ToArray();
            }

            //make a variable of IPrinciple
            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            //declare and init with data Context.User
            Context.User = newUserObj;
        }
    }
}
