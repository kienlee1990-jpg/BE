using Microsoft.AspNetCore.Authorization;

namespace UserManagementAPI.Authorization
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        public PermissionAttribute(string permission)
        {
            Policy = permission;
        }
    }
}