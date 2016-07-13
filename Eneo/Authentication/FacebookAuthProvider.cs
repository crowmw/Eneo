using Microsoft.Owin.Security.Facebook;
using System.Threading.Tasks;

namespace Eneo.Authentication
{
    public class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            context.Identity.AddClaim(new System.Security.Claims.Claim("ExternalAccessToken", context.AccessToken));
            return Task.FromResult<object>(null);
        }
    }
}