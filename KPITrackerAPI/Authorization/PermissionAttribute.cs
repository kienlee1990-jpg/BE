using Microsoft.AspNetCore.Authorization;

namespace KPITrackerAPI.Authorization
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        public PermissionAttribute(string permission)
        {
            Policy = permission;
        }
    }
}
