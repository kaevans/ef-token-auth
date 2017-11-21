# Connect to Azure SQL Database with token authentication using Entity Framework
This repository contains samples that demonstrate how to use Entity Framework to connect to an Azure SQL Database using token authentication. 

While these projects focus on demonstrating EF with token authentication to Azure SQL Database, there are multiple opportunities for improvement such as using dependency injection for the token cache implementation and separating the ADAL components from the ASP.NET MVC sample project. To see a complete sample to create a multi-tenant application using ADAL, see:

https://github.com/mspnp/multitenant-saas-guidance 

One of the challenges with using the ADAL library with the OpenId Connect flow is that the behavior of redirecting a user to re-authenticate
is best left to a controller method. However, the behavior is intrinsic to the connection, suggesting the implementation belongs
as a DbConfiguration method. 

## Configure Azure SQL Database with token authentication

To configure Azure SQL Database to use token authentication, see the following article:

https://docs.microsoft.com/en-us/azure/sql-database/sql-database-aad-authentication-configure

## Token cache
Each project includes two token cache implementations. `Cache/SessionTokenCache.cs` will cache tokens in the ASP.NET session cache. This cache is only intended to be used in a single server implementation such as dev/test. Implementations that span multiple servers should use an implementation that enables multiple servers to use the same cache. `Cache/RedisTokenCache.cs` demonstrates how to use a Redis cache (such as the Redis service provided in Azure) 

The sample is configured to use the `RedisTokenCache` implementation and requires a connection to a Redis cache.

## Running the samples
These samples use a configuration file that is not included in source control, intentionally keeping secrets out of the source code repository. Add a file at the root of the web project named Web.Keys.config with the following contents, and provide values for each:

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

Finally, update the `web.config` file to use the connection string to your Azure SQL Database as shown in each project.