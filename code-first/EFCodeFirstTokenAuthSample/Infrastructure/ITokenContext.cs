using System.Threading.Tasks;

namespace EFCodeFirstTokenAuthSample.Infrastructure
{
    /// <summary>
    /// Interface for DbContext implementations that use token authentication. 
    /// Used by the SqlTokenConnectionInterceptor to identify which context it 
    /// should apply a token to.
    /// </summary>
    interface ITokenContext
    {
        Task InitializeTokenAsync();

        string AccessToken { get; }
    }
}
