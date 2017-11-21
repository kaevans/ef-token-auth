using EFDatabaseFirstTokenAuthSample.Cache;
using EFDatabaseFirstTokenAuthSample.Infrastructure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace EFDatabaseFirstTokenAuthSample.Models
{
    [DbConfigurationType(typeof(SqlTokenAuthenticationConfiguration))]
    public partial class sqlpassadtestEntities : DbContext
    {
        private string _accessToken;

        public string AccessToken { get { return _accessToken; } }

        /// <summary>
        /// Initialize the access token for the context. Will be read by the
        /// SqlTokenConnectionInterceptor and applied to the underlying 
        /// connection.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeTokenAsync()
        {
            var userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            var authContext = new AuthenticationContext(Startup.authority,
                        new RedisTokenCache(userObjectID));

            var credential = new ClientCredential(Startup.clientId, Startup.appKey);
            
            AuthenticationResult result = null;

            try
            {
                var userIdentifier = new UserIdentifier(userObjectID, UserIdentifierType.UniqueId);
                result = await authContext.AcquireTokenSilentAsync(Startup.targetResource, credential, userIdentifier);
                _accessToken = result.AccessToken;
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the 
            // required token. Send exception back to caller so they can handle how to do this. Client
            // applications can call AcquireTokenAsync, while web applications need to initiate a new 
            // challenge through the OWIN middleware.
            catch (AdalSilentTokenAcquisitionException ee)
            {
                throw ee;
            }
        }
    }
}