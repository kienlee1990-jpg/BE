using UserManagementAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UserManagementAPI.Interfaces
{
    public interface IPermissionService
    {
        Task<List<string>> GetPermissionsAsync(ApplicationUser user);
    }
}