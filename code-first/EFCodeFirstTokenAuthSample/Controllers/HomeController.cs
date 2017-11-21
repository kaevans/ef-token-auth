using EFCodeFirstTokenAuthSample.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EFCodeFirstTokenAuthSample.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> AAD()
        {
            int numbers = -1;

            using (var ctx = new ADALContext())
            {
                try
                {
                    //Initialize the access token for the context
                    await ctx.InitializeTokenAsync();
                    numbers = ctx.Todoes.Count();
                    ViewBag.Message = string.Format("Username: {0} ; Records in DB: {1}", User.Identity.Name, numbers);
                }                
                catch(AdalSilentTokenAcquisitionException oops)
                {
                    //If we fail to acquire a token silently, this means the user's token for 
                    //the requested resource was not found in the cache. We have to go through 
                    //the OpenId Connect flow again to obtain a fresh token and store it in the cache.                    
                    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/Home/AAD" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
            };
                    

            return View();
        }

    }
}