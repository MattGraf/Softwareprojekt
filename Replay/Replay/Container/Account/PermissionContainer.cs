using Microsoft.EntityFrameworkCore;
using Replay.Container.Account.MTM;
using Replay.Models.Account;
using Replay.Models.Account.MTM;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace Replay.Container.Account
{
    /// <summary>
    /// Managing the connection to the database for the <see cref="Permission"/> class
    /// </summary>
    /// <author>Felix Nebel</author>
    public class PermissionContainer
    {
        
        
        private MakandraContext makandraContext;

        /// <summary>
        /// Creates a new Container with the passed database context.<br/>
        /// This is also registered as a service in <see cref="Program"/>.
        /// </summary>
        /// <param name="makandraContext"></param>
        /// <author>Felix Nebel</author>
        public PermissionContainer(MakandraContext makandraContext)
        {
            this.makandraContext = makandraContext;
        }

        /// <summary>
        /// Adds a new permission to the database via the <see cref="makandraContext"/>
        /// </summary>
        /// <param name="permission"></param>
        /// <author>Felix Nebel</author>
        public async void AddPermission(Permission permission)
        {
            await makandraContext.Permissions.AddAsync(permission);
            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the permission in the database, that has the same Id as the passed <see cref="Permission"/> using the <see cref="makandraContext"/>.
        /// <br />If no permission with this Id exists, the passed permission gets inserted into the database using <see cref="AddPermission(Permission)"/>.
        /// </summary>
        /// <param name="permission"></param>
        /// <author>Felix Nebel</author>
        public async void UpdatePermission(Permission permission)
        {
            var PermissionToUpdate = makandraContext.Permissions.FirstOrDefault<Permission>(_role => _role.Id == permission.Id);
            if (PermissionToUpdate == null)
            {
                AddPermission(permission);
                return;
            }

            PermissionToUpdate.Name = permission.Name;

            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the permission in the database, that has the same Id as the passed <see cref="Permission"/> using the <see cref="makandraContext"/>.
        /// <br />If no permission with this Id exists, nothing happens.
        /// </summary>
        /// <param name="permission"></param>
        /// <author>Felix Nebel</author>
        public async void DeletePermission(Permission permission)
        {

            var PermissionToDelete = makandraContext.Permissions.FirstOrDefault<Permission>(_user => _user.Id == permission.Id);
            if (PermissionToDelete == null)
            {
                return;
            }

            makandraContext.Permissions.Remove(PermissionToDelete);

            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the permission from the database that corresponds to the passed id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The corresponding Permission to the Id or null if no Permission with such Id exists.</returns>
        /// <author>Felix Nebel</author>
        public async Task<Permission?> GetPermissionFromId(int id)
        {
            Permission? permission = await makandraContext.Permissions.FirstOrDefaultAsync<Permission>(_permission => _permission.Id == id);
            return permission;
        }

        /// <summary>
        /// Gets a permission by the passed name, only finds permissions with matching capitalisation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The corresponding Permission to the name or null if no permission with such name exists.</returns>
        /// <author>Felix Nebel</author>
        public async Task<Permission?> GetPermissionFromName(string name)
        {
            Permission? Permission = await makandraContext.Permissions.FirstOrDefaultAsync<Permission>(_permission => _permission.Name == name);
            return Permission;
        }

        /// <summary>
        /// Gets all permissions as an Enumerable.
        /// </summary>
        /// <returns>All permissions saved in the database.</returns>
        /// <author>Felix Nebel</author>
        public IEnumerable<Permission> GetPermissions()
        {
            var Permissions = makandraContext.Permissions.AsEnumerable();
            Permissions ??= Enumerable.Empty<Permission>();
            return Permissions;
        }


        /// <summary>
        /// Imports a Json-string with <see cref="Permission"/>s in the database
        /// </summary>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(string jsonFile)
        {

            if (jsonFile is null) return;



            List<Permission> roles = new List<Permission>();

            try
            {
                roles = JsonSerializer.Deserialize<List<Permission>>(jsonFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            roles.ForEach(e => {
                AddPermission(e);
            });
        }
    }
}
