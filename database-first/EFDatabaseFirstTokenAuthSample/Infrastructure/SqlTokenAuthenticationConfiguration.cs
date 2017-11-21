using System.Data.Entity;

namespace EFDatabaseFirstTokenAuthSample.Infrastructure
{
    /// <summary>
    /// Configures the EF context class to add a connection interceptor
    /// that injects an access token upon connection.
    /// </summary>
    public class SqlTokenAuthenticationConfiguration : DbConfiguration
    {

        public SqlTokenAuthenticationConfiguration() : base()
        {
            //Add the connection interceptor so we can inject the access token
            //while the connection is opening.
            this.AddInterceptor(new SqlTokenConnectionInterceptor());            
        }

    }
}