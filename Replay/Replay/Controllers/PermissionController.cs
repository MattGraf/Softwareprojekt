using Replay.Container.Account.MTM;
using Replay.Container.Account;
using Replay.Models.Account;
using Replay.ViewModels.Account;
using System.Diagnostics;
using System.Text.Json;
using Replay.Authorization;
namespace Replay.Controllers
{
    /// <summary>
    /// Controller to manage the views for the <see cref="Permission"/>s
    /// </summary>
    /// <author>Felix Nebel</author>
    public class PermissionController : Controller
    {
        private RoleContainer _roleContainer;
        private UserContainer _userContainer;
        private UserRolesContainer _userRoleContainer;
        private PermissionContainer _permissionContainer;
        private RolePermissionContainer _rolePermissionContainer;
        private readonly ILogger<AccountController> _logger;
        /// <summary>
        /// Creates a PermissionController with the needed Containers
        /// </summary>
        /// <param name="logger">Logger for logging events</param>
        /// <param name="userContainer">Container for the connection to the database</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="userRolesContainer">Container for the connection to the database</param>
        /// <param name="rolePermissionContainer">Container for the connection to the database</param>
        /// <param name="permissionContainer">Container for the connection to the database</param>
        /// <author>Felix Nebel</author>
        public PermissionController(ILogger<AccountController> logger,
            UserContainer userContainer, RoleContainer roleContainer,
            UserRolesContainer userRolesContainer, RolePermissionContainer rolePermissionContainer, PermissionContainer permissionContainer)
        {
            _logger = logger;
            _userContainer = userContainer;
            _roleContainer = roleContainer;
            _userRoleContainer = userRolesContainer;
            _rolePermissionContainer = rolePermissionContainer;
            _permissionContainer = permissionContainer;
        }
        /// <summary>
        /// Change the View to overview of all the <see cref="Permission"/>s
        /// </summary>
        /// <returns>View of all the <see cref="Permission"/>s</returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public IActionResult Index()
        {
            var permissions = _permissionContainer.GetPermissions();
            return View(permissions);
        }
        /// <summary>
        /// Change the View to the Create Page for the Permission
        /// </summary>
        /// <returns>View of the Create Page</returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public IActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Change the View to the Edit Page for the selected <see cref="Permission"/> with the passed <paramref name="id"/>
        /// </summary>
        /// <param name="id">ID of the permission to edit</param>
        /// <returns>View of the Edit Page for the Permission with <paramref name="id"/></returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public IActionResult Edit(int id)
        {
            Permission? permission = _permissionContainer.GetPermissionFromId(id).Result;
            return View(permission);
        }
        /// <summary>
        /// Edits the passed permission based on the permission's ID
        /// </summary>
        /// <param name="permissionViewModel">ViewModel containing permission data</param>
        /// <returns>Returns the Index View of permissions</returns>
        /// <author>Felix Nebel</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("Administrator")]
        public IActionResult Edit(Permission permissionViewModel)
        {
            Permission? previousPermission = _permissionContainer.GetPermissionFromId(permissionViewModel.Id).Result;
            Permission newPermission = new Permission()
            {
                Id = permissionViewModel.Id,
                Name = permissionViewModel.Name
            };
            _permissionContainer.UpdatePermission(newPermission);
            return RedirectToAction("Index", "Permission");
        }
        /// <summary>
        /// Creates a new permission in the application
        /// </summary>
        /// <param name="permission">The permission to create</param>
        /// <returns>Redirect to the Index View if successful, otherwise returns to the Create View</returns>
        /// <author>Felix Nebel</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Permission permission)
        {
            if (_roleContainer.GetRoleFromName(permission.Name).Result != null)
                ModelState.AddModelError(nameof(permission.Name), "Eine Berechtigung mit diesem Namen existiert bereits.");
            if (ModelState.IsValid)
            {
                _permissionContainer.AddPermission(permission);
                return RedirectToAction("Index");
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Debug.WriteLine(error.ErrorMessage);
            }

            return View();

        }

        /// <summary>
        /// Deletes the selected permission with the passed <paramref name="Id"/>
        /// </summary>
        /// <param name="Id">ID of the permission to delete</param>
        /// <returns>Redirect to the Index View</returns>
        /// <author>Felix Nebel</author>
        [PermissionChecker("Administrator")]
        public IActionResult Delete(string Id)
        {
            Permission? permissionToDelete = _permissionContainer.GetPermissionFromId(Int32.Parse(Id)).Result;
            if (permissionToDelete == null)
            {
                return View("Error");
            }
            _permissionContainer.DeletePermission(permissionToDelete);
            
            return RedirectToAction("Index");

        }

    }
}
