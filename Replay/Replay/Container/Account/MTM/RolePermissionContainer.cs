using Replay.Models.Account.MTM;
using Replay.Models.Account;
using System.Linq;
using System.Diagnostics;

namespace Replay.Container.Account.MTM
{
    /// <summary>
    /// Managing the connection to the database for the <see cref="RolePermission"/> class.<br />
    /// This is also registered as a service in <see cref="Program"/>.
    /// </summary>
    /// <author>Felix Nebel</author>
    public class RolePermissionContainer
    {
        private MakandraContext makandraContext;
        /// <summary>
        /// Creates a new Container with the passed database context
        /// </summary>
        /// <param name="makandraContext"></param>
        /// <author>Felix Nebel</author>
        public RolePermissionContainer(MakandraContext makandraContext)
        {
            this.makandraContext = makandraContext;
        }
        /// <summary>
        /// Adds the <paramref name="rolePermission"/> relation to the database via the <see cref="makandraContext"/>
        /// </summary>
        /// <param name="rolePermission"></param>
        /// <author>Felix Nebel</author>
        public async void AddRolePermission(RolePermission rolePermission)
        {
            await makandraContext.RolePermissions.AddAsync(rolePermission);
            await makandraContext.SaveChangesAsync();
        }
        /// <summary>
        /// Updates the <paramref name="rolePermission"/> relation in the database, that has the same Ids as the passed <see cref="RolePermission"/> using the <see cref="makandraContext"/>.
        /// <br />If no <see cref="RolePermission"/> relation with corresponding Ids exists, the passed <paramref name="rolePermission"/> relation gets inserted into the database using <see cref="AddRolePermission(RolePermission)"/>.
        /// </summary>
        /// <param name="rolePermission"></param>
        /// <author>Felix Nebel</author>
        public async void UpdateRolePermission(RolePermission rolePermission)
        {
            var RolePermissionToUpdate = makandraContext.RolePermissions.FirstOrDefault(
                _rolePermission => _rolePermission.RoleId == rolePermission.RoleId && _rolePermission.PermissionId == rolePermission.PermissionId
            );
            if (RolePermissionToUpdate == null)
            {
                AddRolePermission(rolePermission);
                return;
            }
            RolePermissionToUpdate = rolePermission;

            await makandraContext.SaveChangesAsync();
        }


        /// <summary>
        /// Removes <paramref name="rolePermission"/> from the database, that has the same Ids as the passed <see cref="RolePermission"/> using the <see cref="makandraContext"/>.
        /// <br />If no <see cref="RolePermission"/> relation with corresponding Ids exists, nothing happens.
        /// </summary>
        /// <param name="rolePermission"></param>
        /// <author>Felix Nebel</author>
        public async void DeleteRolePermission(RolePermission rolePermission)
        {
            Debug.WriteLine("Removing");
            var RolePermissionToDelete = makandraContext.RolePermissions.FirstOrDefault(
                _rolePermission => _rolePermission.RoleId == rolePermission.RoleId && _rolePermission.PermissionId == rolePermission.PermissionId
            );
            if (RolePermissionToDelete == null)
            {
                return;
            }

            makandraContext.RolePermissions.Remove(RolePermissionToDelete);

            await makandraContext.SaveChangesAsync();
        }


        /// <summary>
        /// Returns all the <see cref="Role"/>s assigned to the <paramref name="permission" />.
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>Enumerator over the <see cref="Role"/>s.</returns>
        /// <author>Felix Nebel</author>

        public async Task<IEnumerable<Role>> GetRolesFromPermission(Permission permission)
        {
            var Roles = makandraContext.RolePermissions
                .Where(_rolePermission => _rolePermission.PermissionId == permission.Id)
                .Select(_rolePermission => _rolePermission.Role).ToList<Role>();
            return Roles;
        }
    
        public async Task<IEnumerable<RolePermission>> GetRolePermissionsFromRole(Role role)
        {
            var RolePermissions = makandraContext.RolePermissions.Where(rolepermission => rolepermission.RoleId == role.Id);
            return RolePermissions;

        }

        public async Task<IEnumerable<RolePermission>> GetRolePermissionsFromPermission(Permission permission)
        {
            var RolePermissions = makandraContext.RolePermissions.Where(rolepermission => rolepermission.RoleId == permission.Id);
            return RolePermissions;

        }

        /// <summary>
        /// Returns all the <see cref="Permission"/>s assigned to the <paramref name="role"/>.
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Enumerator over the <see cref="Permission"/>s.</returns>
        /// <author>Felix Nebel</author>
        public IEnumerable<Permission> GetPermissionsFromRole(Role role)
        {
            var Users = makandraContext.RolePermissions
                .Where(_rolePermission => _rolePermission.RoleId == role.Id)
                .Select(_rolePermission => _rolePermission.Permission).ToList<Permission>();
            Debug.WriteLine("Role: " + role.Id);
            return Users;
        }

        /// <summary>
        /// Returns all the <see cref="Permission"/>s assigned to any of the <paramref name="roles"/>.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>Enumerator over the <see cref="Permission"/>s.</returns>
        /// <author>Felix Nebel</author>
        public async Task<IEnumerable<Permission>> GetPermissionsFromRoles(IEnumerable<Role> roles)
        {
            var RoleIds = roles.Select(_role => _role.Id).ToList();
            var Users = makandraContext.RolePermissions
                .Where(_rolePermission => RoleIds.Contains(_rolePermission.RoleId))
                .Select(_rolePermission => _rolePermission.Permission).ToList();
            return Users;
        }


        /// <summary>
        /// Returns all the <see cref="Permission"/>s assigned to all the <paramref name="roles"/>.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>Enumerator over the <see cref="Permission"/>s.</returns>
        /// <author>Felix Nebel</author>
        public IEnumerable<Permission> GetPermissionsFromExactRoles(IEnumerable<Role> roles)
        {
            IEnumerable<Permission> permissions = Enumerable.Empty<Permission>();
            foreach (Role role in roles)
            {
                var _permissions = GetPermissionsFromRole(role).ToList();
                if (_permissions.Count == 0)
                    return Enumerable.Empty<Permission>();
                permissions ??= _permissions;
                permissions = permissions.Where(user => _permissions.Contains(user)).ToList();
            }
            permissions ??= Enumerable.Empty<Permission>();
            return permissions;
        }
    }
}
