using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace KIOT.Server.Data.Persistence.Identity
{
    internal class ApplicationUser : IdentityUser
    {
        public IList<Claim> Claims { get; internal set; }
    }
}
