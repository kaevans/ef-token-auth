using EFCodeFirstTokenAuthSample.Cache;
using EFCodeFirstTokenAuthSample.Infrastructure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EFCodeFirstTokenAuthSample.Models
{
    /// <summary>
    /// The DbContext implementation. 
    /// </summary>
    public class ADALContext : DbContext, ITokenContext
    {
        private string _accessToken;

        public ADALContext()
            : base("ADALContext")
        {
            
        }
        

        public DbSet<Todo> Todoes { get; set; }

        public string AccessToken { get { return _accessToken; } }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

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
            var userIdentifier = new UserIdentifier(userObjectID, UserIdentifierType.UniqueId);
            AuthenticationResult result = null;

            try
            {
                result = await authContext.AcquireTokenSilentAsync(Startup.targetResource, credential, userIdentifier);
                _accessToken = result.AccessToken;
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (AdalSilentTokenAcquisitionException ee)
            {
                throw ee;
            }
        }

    }
}