using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Proje.API.Services
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user, IList<string> roles);
    }
}