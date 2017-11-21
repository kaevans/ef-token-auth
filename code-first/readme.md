# Connect to Azure SQL Database with token authentication using Entity Framework
This sample demonstrates how to use Entity Framework code-first to connect to an Azure SQL Database using token authentication. 

This project focuses on demonstrating EF with token authentication to Azure SQL Database. While the sample encapsulates best practices, there are multiple opportunities for improvement such as using dependency injection for the token cache implementation and separating the ADAL components from the ASP.NET MVC sample project. Unit tests are not provided. To see a more complete sample using ADAL (bot not EF and ADAL combined), see:

https://github.com/mspnp/multitenant-saas-guidance 

## Configure Azure SQL Database with token authentication

To configure Azure SQL Database to use token authentication, see the following article:

https://docs.microsoft.com/en-us/azure/sql-database/sql-database-aad-authentication-configure

It is important not to skip the step for requesting permissions for your application to the resource `https://database.windows.net/`. The application must request and be granted permissions to this downstream API.

## Project structure
- `App_Start/DataConfig.cs` - Registers the database initializer.
- `Infrastructure/SqlTokenAuthenticationConfiguration.cs` - Configures the EF context class to add a connection interceptor.
- `Infrastructure/SqlTokenConnectionInterceptor.cs` - Intercepts the database connection opening to add the access token. This removes the need to set the token in every calling method.
- `Models/ITokenContext.cs` - Interface for DbContext implementations that use token authentication. Used by the SqlTokenConnectionInterceptor to identify which context it should apply a token to.
- `Models/EFADALContext.cs` - The DbContext implementation. The InitializeTokenAsync method attempts to read the cache to obtain a token for the user for the given resource, and if one is found, applies it to the model. If one is not found, it is a cache miss and an AdalSilentTokenAcquisitionException exception is re-thrown.

## Token cache
The project includes two token cache implementations. `Cache/SessionTokenCache.cs` will cache tokens in the ASP.NET session cache. This cache is only intended to be used in a single server implementation such as dev/test. Implementations that span multiple servers should use an implementation that enables multiple servers to use the same cache. `Cache/RedisTokenCache.cs` demonstrates how to use a Redis cache (such as the Redis service provided in Azure) 

The sample is configured to use the `RedisTokenCache` implementation and requires a connection to a Redis cache. This is recommended as a best practice, especially for implementations that have multiple nodes behind a load balancer.

## Running the sample
This sample uses a configuration file that is not included in source control, intentionally keeping secrets out of the source code repository. Add a file at the root of the web project named `Web.Keys.config` with the following contents, and provide values for each:

````xml
<appSettings>
  <!-- Update with the logout redirect URL -->
  <add key="ida:PostLogoutRedirectUri" value="https://localhost:44315/" />
  <!--Update with your Application ID-->
  <add key="ida:ClientId" value="" />
  <!--Update with your tenant name-->
  <add key="ida:Domain" value="" />
  <!--Update with your tenant ID-->
  <add key="ida:TenantId" value="" />
  <!--Generate a Key-->
  <add key="ida:AppKey" value="" />
  <!--Redis connection string-->
    <add key="ida:CacheConnection" value=""/>
</appSettings>
````

Finally, update the `web.config` file to use the connection string to your Azure SQL Database:

````xml
  <connectionStrings>
    <!-- Update with the connection string to your Azure SQL Database -->
    <add name="EFADALContext"
        providerName="System.Data.SqlClient"
        connectionString="Server=tcp:sqlpassadtest.database.windows.net,1433;Initial Catalog=sqlpassadtest;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;"/>
  </connectionStrings>
````