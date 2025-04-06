using Replay.Container.Account.MTM;
using Replay.Container;
using Microsoft.AspNetCore.Identity;
using Replay.Models.Account;
using System.Diagnostics;
using Replay.Container.Account;
using Microsoft.EntityFrameworkCore;
using Replay.ViewModels.Account;
using Replay.Authorization;
using System.Security;


namespace Replay.Controllers
{
    /// <summary>
    /// Controller to manage the views for the <see cref="Role"/>s
    /// </summary>
    /// <author>Felix Nebel</author>
    public class RoleController : Controller
    {
        private RoleContainer _roleContainer;
        private PermissionContainer _permissionContainer;
        private RolePermissionContainer _rolePermissionContainer;
        /// <summary>
        /// Creates a RoleController with the needed Containers
        /// </summary>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="rolePermissionContainer">Container for the connection to the database</param>
        /// <param name="permissionContainer">Container for the connection to the database</param>
        /// <author>Felix Nebel</author>
        public RoleController(RoleContainer roleContainer, RolePermissionContainer rolePermissionContainer,
            PermissionContainer permissionContainer)
        {
            _roleContainer = roleContainer;
            _rolePermissionContainer = rolePermissionContainer;
            _permissionContainer = permissionContainer;
        }

        /// <summary>
        /// Change the View to overview of all the <see cref="Role"/>s
        /// </summary>
        /// <returns>View of all the <see cref="Role"/>s</returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public IActionResult Index()
        {
            var roles = _roleContainer.GetRoles();
            Debug.WriteLine(roles.Count());
            return View(roles);
        }

        /// <summary>
        /// Change the View to the Edit Page for the selected <see cref="Role"/> with the passed <paramref name="id"/>
        /// </summary>
        /// <param name="id">ID of the role to edit</param>
        /// <returns>View of the Edit Page for the Role with <paramref name="id"/></returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public IActionResult Edit(int id)
        {
            var role = _roleContainer.GetRoleFromId(id).Result;
            var perms = _rolePermissionContainer.GetPermissionsFromRole(role);
            var allperms = _permissionContainer.GetPermissions();
            return View(role.CreateRolePermissionViewModel(allperms, perms));
        }


        /// <summary>
        /// Edits the passed role based on the role's ID
        /// </summary>
        /// <param name="role">ViewModel containing role and permissions data</param>
        /// <returns>Returns the Index View of roles</returns>
        /// <author>Felix Nebel</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("Administrator")]
        public IActionResult Edit(RolePermissionViewModel role)
        {
            Role? previousRole = _roleContainer.GetRoleFromId(role.Id).Result;
            _rolePermissionContainer.GetRolePermissionsFromRole(previousRole).Result.ToList().ForEach(rolepermission => _rolePermissionContainer.DeleteRolePermission(rolepermission));
            Role newRole = new Role()
            {
                Id = role.Id,
                Name = role.Name
            };
            _roleContainer.UpdateRole(newRole);
            if (role.PermissionViews == null)
                return RedirectToAction("Index");
            var viewList = role.PermissionViews.Where(rolepermission => rolepermission.isAllowed == true).ToList();
            viewList.ForEach(async rolepermission => _rolePermissionContainer.AddRolePermission(new Models.Account.MTM.RolePermission()
            {
                PermissionId = rolepermission.Id,
                Permission = await _permissionContainer.GetPermissionFromId(rolepermission.Id),
                RoleId = newRole.Id,
                Role = await _roleContainer.GetRoleFromId(newRole.Id)
            }));
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Change the View to the Create Page for the Role
        /// </summary>
        /// <returns>View of the Create Page</returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates a new role in the application
        /// </summary>
        /// <param name="role">The role to create</param>
        /// <returns>Redirect to the Index View if successful, otherwise returns to the Create View</returns>
        /// <author>Felix Nebel</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Role role)
        {
            if (_roleContainer.GetRoleFromName(role.Name).Result != null)
                ModelState.AddModelError(nameof(role.Name), "Eine Rolle mit diesem Namen existiert bereits.");
            if (ModelState.IsValid)
            {
                _roleContainer.AddRole(role);
                return RedirectToAction("Index");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Debug.WriteLine(error.ErrorMessage);
            }

            Debug.WriteLine(role.Id);
            return View();

        }

        /// <summary>
        /// Deletes the selected role with the passed <paramref name="Id"/>
        /// </summary>
        /// <param name="Id">ID of the role to delete</param>
        /// <returns>Redirect to the Index View</returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> DeleteAsync(string Id)
        {
            Role? roleToDelete = await _roleContainer.GetRoleFromId(Int32.Parse(Id));
            if (roleToDelete == null)
            {
                return View("Error");
            }
            _roleContainer.DeleteRole(roleToDelete);

            return RedirectToAction("Index");

        }

        

    }
}
