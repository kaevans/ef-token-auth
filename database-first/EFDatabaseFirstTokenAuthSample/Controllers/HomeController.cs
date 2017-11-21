using EFDatabaseFirstTokenAuthSample.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EFDatabaseFirstTokenAuthSample.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            int numbers = -1;

            using (var ctx = new sqlpassadtestEntities())
            {
                try
                {
                    //Initialize the access token for the context
                    await ctx.InitializeTokenAsync();
                    numbers = ctx.Customers.Count();
                    ViewBag.Message = string.Format("Username: {0} ; Records in DB: {1}", User.Identity.Name, numbers);
                }
                catch (AdalSilentTokenAcquisitionException oops)
                {
                    //If we fail to acquire a token silently, this means the user's token for 
                    //the requested resource was not found in the cache. We have to go through 
                    //the OpenId Connect flow again to obtain a fresh token and store it in the cache.                    
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/Home/Index" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
            };
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}