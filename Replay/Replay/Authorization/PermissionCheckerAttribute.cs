using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Diagnostics;
using Replay.Data;
using Replay.Models.Account;
using Replay.Container.Account;
using Replay.Container.Account.MTM;

namespace Replay.Authorization
{
    /// <summary>
    /// Manages checking the database on whether the user has the correct permissions to access an action
    /// </summary>
    /// <author>Felix Nebel</author>
    public class PermissionCheckerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _permissionName;
        public PermissionCheckerAttribute(string permission)
        {
            _permissionName = permission;
        }
        
        /// <summary>
        /// Gets called before the annotated methods call. Checks whether any of the users roles is allowed for the action,<br/>
        /// by looking for a permission
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var _dbContext = context.HttpContext.RequestServices.GetRequiredService<MakandraContext>();
            UserContainer _userContainer = new UserContainer(_dbContext);
            RoleContainer _roleContainer = new RoleContainer(_dbContext);
            UserRolesContainer _userRolesContainer = new UserRolesContainer(_dbContext);
            PermissionContainer _permissionContainer = new PermissionContainer(_dbContext);
            RolePermissionContainer _rolePermissionContainer = new RolePermissionContainer(_dbContext);
            var user = _userContainer.GetLoggedInUser(context.HttpContext).Result;
            if (user == null || !user.Active)
            {
                context.Result = new RedirectResult("/Account/Login");
                return;
            }
            Permission? permission = _permissionContainer.GetPermissionFromName(_permissionName).Result;

            if (permission == null)
            {
                Debug.WriteLine("No Authorization found for site.");
                return;
            }
            // Gets roles that have access to the _permission
            var allowedRoles = _rolePermissionContainer.GetRolesFromPermission(permission).Result;

            var hasPermission = _userRolesContainer.GetRolesFromUser(user).Result.Any(role => allowedRoles.Contains(role));


            if (!hasPermission)
            {
                Debug.WriteLine("No Permission!");
                context.Result = new RedirectResult("/Account/Login");
            }
            else
            {
                Debug.WriteLine("Permission!");
            }
        }
    }
}
