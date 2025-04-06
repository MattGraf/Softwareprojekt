using Microsoft.EntityFrameworkCore;
using Replay.Models.Account;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Replay.Container
{
    /// <summary>
    /// Managing the connection to the database for the <see cref="Role"/> class
    /// </summary>
    /// <author>Felix Nebel</author>
    public class RoleContainer
    {
        private MakandraContext makandraContext;

        /// <summary>
        /// Creates a new Container with the passed database context.<br />
        /// This is also registered as a service in <see cref="Program"/>.
        /// </summary>
        /// <param name="makandraContext"></param>
        /// <author>Felix Nebel</author>
        public RoleContainer(MakandraContext makandraContext)
        {
            this.makandraContext = makandraContext;
        }

        /// <summary>
        /// Adds a new role to the database via the <see cref="makandraContext"/>
        /// </summary>
        /// <param name="role"></param>
        /// <author>Felix Nebel</author>
        public virtual async void AddRole(Role role)
        {
            await makandraContext.Roles.AddAsync(role);
            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the role in the database, that has the same Id as the passed <see cref="Role"/> using the <see cref="makandraContext"/>.
        /// <br />If no role with this Id exists, the passed role gets inserted into the database using <see cref="AddRole(Role)"/>.
        /// </summary>
        /// <param name="role"></param>
        /// <author>Felix Nebel</author>
        public async void UpdateRole(Role role)
        {
            var RoleToUpdate = makandraContext.Roles.FirstOrDefault<Role>(_role => _role.Id == role.Id);
            if (RoleToUpdate == null)
            {
                AddRole(role);
                return;
            }

            RoleToUpdate.Name = role.Name;

            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the role in the database, that has the same Id as the passed <see cref="Role"/> using the <see cref="makandraContext"/>.
        /// <br />If no role with this Id exists, nothing happens.
        /// </summary>
        /// <param name="role"></param>
        /// <author>Felix Nebel</author>
        public async void DeleteRole(Role role)
        {

            var RoleToDelete = makandraContext.Roles.FirstOrDefault<Role>(_role => _role.Id == role.Id);
            if (RoleToDelete == null)
            {
                return;
            }

            makandraContext.Roles.Remove(RoleToDelete);

            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the role from the database that corresponds to the passed id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The corresponding Role to the Id or null if no Role with such Id exists.</returns>
        /// <author>Felix Nebel</author>
        public async Task<Role?> GetRoleFromId(int id)
        {
            Role? Role = await makandraContext.Roles.FirstOrDefaultAsync<Role>(_role => _role.Id == id);
            return Role;
        }

        /// <summary>
        /// Gets a role by the passed name, only finds roles with matching capitalisation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The corresponding Role to the name or null if no role with such name exists.</returns>
        /// <author>Felix Nebel</author>
        public async Task<Role?> GetRoleFromName(string name)
        {
            Role? Role = await makandraContext.Roles.FirstOrDefaultAsync<Role>(_role => _role.Name.Equals(name));
            return Role;
        }

        /// <summary>
        /// Gets all roles as an Enumerable.
        /// </summary>
        /// <returns>All roles saved in the database.</returns>
        /// <author>Felix Nebel</author>
        public IEnumerable<Role> GetRoles()
        {
            var Roles = makandraContext.Roles.AsEnumerable();
            Roles ??= Enumerable.Empty<Role>();
            return Roles;
        }

        /// <summary>
        /// Imports a Json-string with <see cref="Role"/>s in the database
        /// </summary>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(string jsonFile)
        {

            if (jsonFile is null) return;



            List<Role> roles = new List<Role>();

            try
            {
                roles = JsonSerializer.Deserialize<List<Role>>(jsonFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            roles.ForEach(e => {
                AddRole(e);
            });
        }

    }
}
