using Microsoft.EntityFrameworkCore;
using Replay.Models.Account;
using Replay.Models.Account.MTM;
using System.Linq;
using System.Security.Claims;

namespace Replay.Container.Account.MTM
{
    /// <summary>
    /// Managing the connection to the database for the <see cref="UserRole"/> class.<br />
    /// This is also registered as a service in <see cref="Program"/>.
    /// </summary>
    /// <author>Felix Nebel</author>
    public class UserRolesContainer
    {
        private MakandraContext makandraContext;

        /// <summary>
        /// Creates a new Container with the passed database context
        /// </summary>
        /// <param name="makandraContext"></param>
        /// <author>Felix Nebel</author>
        public UserRolesContainer(MakandraContext makandraContext)
        {
            this.makandraContext = makandraContext;
        }
        /// <summary>
        /// Adds the <paramref name="userRole"/> relation to the database via the <see cref="makandraContext"/>
        /// </summary>
        /// <param name="userRole"></param>
        /// <author>Felix Nebel</author>
        public async void AddUserRole(UserRole userRole)
        {
            await makandraContext.UserRoles.AddAsync(userRole);
            await makandraContext.SaveChangesAsync();
        }
        /// <summary>
        /// Updates the <paramref name="userRole"/> relation in the database, that has the same Ids as the passed <see cref="UserRole"/> using the <see cref="makandraContext"/>.
        /// <br />If no <see cref="UserRole"/> relation with corresponding Ids exists, the passed <paramref name="userRole"/> relation gets inserted into the database using <see cref="AddUserRole(UserRole)"/>.
        /// </summary>
        /// <param name="userRole"></param>
        /// <author>Felix Nebel</author>
        public async void UpdateUserRole(UserRole userRole)
        {
            var UserRoleToUpdate = makandraContext.UserRoles.FirstOrDefault(
                _userRole => _userRole.RoleId == userRole.RoleId && _userRole.UserId == userRole.UserId
            );
            if (UserRoleToUpdate == null)
            {
                AddUserRole(userRole);
                return;
            }
            UserRoleToUpdate = userRole;

            await makandraContext.SaveChangesAsync();
        }
        /// <summary>
        /// Removes <paramref name="userRole"/> from the database, that has the same Ids as the passed <see cref="UserRole"/> using the <see cref="makandraContext"/>.
        /// <br />If no <see cref="UserRole"/> relation with corresponding Ids exists, nothing happens.
        /// </summary>
        /// <param name="userRole"></param>
        /// <author>Felix Nebel</author>
        public async void DeleteUserRole(UserRole userRole)
        {

            var UserRoleToDelete = makandraContext.UserRoles.FirstOrDefault(
                _userRole => _userRole.RoleId == userRole.RoleId && _userRole.UserId == userRole.UserId
            );
            
            makandraContext.UserRoles.Remove(UserRoleToDelete);

            await makandraContext.SaveChangesAsync();
        }


        /// <summary>
        /// Returns all the <see cref="Role"/>s assigned to the <paramref name="user" />.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Enumerator over the <see cref="Role"/>s.</returns>
        /// <author>Felix Nebel</author>

        public virtual async Task<IEnumerable<Role>> GetRolesFromUser(User user)
        {
            var Roles = await makandraContext.UserRoles
                .Where(_userRole => _userRole.UserId == user.Id)
                .Select(_userRole => _userRole.Role).ToListAsync();
            return Roles;
        }


        public async Task<IEnumerable<UserRole>> GetUserRolesFromUser(User user)
        {
            var UserRoles = await makandraContext.UserRoles
                .Where(_userRole => _userRole.UserId == user.Id)
                .ToListAsync();
            return UserRoles;
        }


        /// <summary>
        /// Returns all the <see cref="User"/>s assigned to the <paramref name="role"/>.
        /// </summary>
        /// <param name="role"></param>
        /// <returns>Enumerator over the <see cref="User"/>s.</returns>
        /// <author>Felix Nebel</author>
        public async Task<IEnumerable<User>> GetUsersFromRole(Role role)
        {
            var Users = await makandraContext.UserRoles
                .Where(_userRole => _userRole.RoleId == role.Id)
                .Select(_userRole => _userRole.User).ToListAsync();
            return Users;
        }




        /// <summary>
        /// Returns all the <see cref="User"/>s assigned to any of the <paramref name="roles"/>.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>Enumerator over the <see cref="User"/>s.</returns>
        /// <author>Felix Nebel</author>
        public async Task<IEnumerable<User>> GetUsersFromRoles(IEnumerable<Role> roles)
        {
            var RoleIds = roles.Select(_role => _role.Id).ToList();
            var Users = await makandraContext.UserRoles
                .Where(_userRole => RoleIds.Contains(_userRole.RoleId))
                .Select(_userRole => _userRole.User).ToListAsync();
            return Users;
        }


        /// <summary>
        /// Returns all the <see cref="User"/>s assigned to all the <paramref name="roles"/>.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns>Enumerator over the <see cref="User"/>s.</returns>
        /// <author>Felix Nebel</author>
        public IEnumerable<User> GetUsersFromExactRoles(IEnumerable<Role> roles)
        {
            IEnumerable<User> users = Enumerable.Empty<User>();
            foreach (Role role in roles)
            {
                var _users = GetUsersFromRole(role).Result.ToList();
                if (_users.Count == 0)
                    return Enumerable.Empty<User>();
                users ??= _users;
                users = users.Where(user => _users.Contains(user)).ToList();
            }
            users ??= Enumerable.Empty<User>();
            return users;
        }

    }
}
