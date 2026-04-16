using KPITrackerAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KPITrackerAPI.Interfaces
{
    public interface IPermissionService
    {
        Task<List<string>> GetPermissionsAsync(ApplicationUser user);
    }
}
