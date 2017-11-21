using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;
using EFDatabaseFirstTokenAuthSample.Models;
using EFDatabaseFirstTokenAuthSample.Infrastructure;
using EFDatabaseFirstTokenAuthSample.Cache;

namespace EFDatabaseFirstTokenAuthSample
{
    public partial class Startup
    {
        public static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        public static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        public static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        public static string appKey = ConfigurationManager.AppSettings["ida:AppKey"];
        public static string targetResource = ConfigurationManager.AppSettings["ida:TargetResource"];
        public static string authority = aadInstance + tenantId;



        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            //If your application is in an infinite loop, see
            // http://katanaproject.codeplex.com/wikipage?title=System.Web%20response%20cookie%20integration%20issues&referringTitle=Documentation
            //This should not be necessary to implement for this solution.
            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    CookieManager = new SystemWebCookieManager()
            //});

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        SecurityTokenReceived = OnSecurityTokenReceived,
                        AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                        AuthenticationFailed = OnAuthenticationFailed

                    }
                });
        }

        private Task OnSecurityTokenReceived(SecurityTokenReceivedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            var token = context.ProtocolMessage.IdToken;
            return Task.FromResult(0);
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/Home/Error?message=" + context.Exception.Message);

            return Task.FromResult(0);
        }



        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification context)
        {
            var code = context.Code;

            ClientCredential credential = new ClientCredential(clientId, appKey);
            string userObjectID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            AuthenticationContext authContext = new AuthenticationContext(authority, new RedisTokenCache(userObjectID));


            // If you create the redirectUri this way, it will contain a trailing slash. 
            // Make sure you've registered the same exact Uri in the Azure Portal (including the slash).
            Uri uri = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));

            AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(code, uri, credential, targetResource);

        }
    }
}
