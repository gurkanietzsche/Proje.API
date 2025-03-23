using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Proje.API.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
        Task<IdentityUser> GetUserByIdAsync(string userId);
        Task<IdentityUser> GetUserByEmailAsync(string email);
        Task<IEnumerable<string>> GetUserRolesAsync(IdentityUser user);
    }
}