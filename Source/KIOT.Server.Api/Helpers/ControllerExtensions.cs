using System.Security.Claims;

namespace KIOT.Server.Api.Helpers
{
    public static class ControllerExtensions
    {
        public static ClaimsIdentity ToClaimsIdentity(this ClaimsPrincipal principal)
        {
            return new ClaimsIdentity(principal.Identity, principal.Claims);
        }
    }
}
