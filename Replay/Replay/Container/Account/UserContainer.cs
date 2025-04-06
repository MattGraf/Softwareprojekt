using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Replay.Data;
using Replay.Models.Account;
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Replay.Container
{
    /// <summary>
    /// Managing the connection to the database for the <see cref="User"/> class.<br />
    /// This is also registered as a service in <see cref="Program"/>.
    /// </summary>
    /// <author>Felix Nebel</author>
    public class UserContainer
    {
        private MakandraContext makandraContext;

        /// <summary>
        /// Creates a new Container with the passed database context
        /// </summary>
        /// <param name="makandraContext"></param>
        /// <author>Felix Nebel</author>
        public UserContainer(MakandraContext makandraContext)
        {
            this.makandraContext = makandraContext;
        }

        /// <summary>
        /// Adds the <paramref name="user"/> to the database via the <see cref="makandraContext"/>
        /// </summary>
        /// <param name="user"></param>
        /// <author>Felix Nebel</author>
        public virtual async void AddUser(User user)
        {
            await makandraContext.Users.AddAsync(user);
            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the user in the database, that has the same Id as the passed <see cref="User"/> using the <see cref="makandraContext"/>.
        /// <br />If no user with this Id exists, the passed user gets inserted into the database using <see cref="AddUser(User)"/>.
        /// </summary>
        /// <param name="user"></param>
        /// <author>Felix Nebel</author>
        public async void UpdateUser(User user)
        {
            var UserToUpdate = makandraContext.Users.FirstOrDefault<User>(_user =>  _user.Id == user.Id);
            if (UserToUpdate == null)
            {
                AddUser(user);
                return;
            }
            UserToUpdate.FullName = user.FullName;
            UserToUpdate.Email = user.Email;
            UserToUpdate.Active = user.Active;

            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes <paramref name="user"/> from the database, that has the same Id as the passed <paramref name="user"/> using the <see cref="makandraContext"/>.
        /// <br />If no <see cref="User"/>with this Id exists, nothing happens.
        /// </summary>
        /// <param name="user"></param>
        /// <author>Felix Nebel</author>
        public async void DeleteUser(User user)
        {

            var UserToDelete = makandraContext.Users.FirstOrDefault<User>(_user => _user.Id == user.Id);
            if (UserToDelete == null)
            {
                return;
            }

            makandraContext.Users.Remove(UserToDelete);

            await makandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the user from the database that corresponds to the passed id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The corresponding user to the Id or null if no user with such Id exists.</returns>
        /// <author>Felix Nebel</author>
        public async Task<User?> GetUserFromId(int id)
        {
            var User = await makandraContext.Users.FirstOrDefaultAsync<User>(_user => _user.Id == id);
            return User;
        }

        /// <summary>
        /// Gets the user from the database that corresponds to the passed email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The corresponding user to the email or null if no user with such email exists.</returns>
        /// <author>Felix Nebel</author>
        public async Task<User?> GetUserFromEmail(string email)
        {
            User? User = await makandraContext.Users.FirstOrDefaultAsync<User>(_user => _user.Email == email);
            return User;
        }

        /// <summary>
        /// Returns the currently logged in user, requires the HttpContext of the controller calling this method to get the claims.
        /// </summary>
        /// <param name="context">The HttpContext of the controller calling this method. Needed to acquire the claims.</param>
        /// <returns>Logged in user or a null Task if not logged in.</returns>
        public virtual Task<User?> GetLoggedInUser(HttpContext context)
        {
            Claim? userEmail = context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            if (userEmail == null)
                return Task.FromResult<User?>(null);
            return GetUserFromEmail(userEmail.Value);
        }
        /// <summary>
        /// Gets all users as an Enumerable.
        /// </summary>
        /// <returns>All users saved in the database.</returns>
        /// <author>Felix Nebel</author>
        public IEnumerable<User> GetUsers()
        {
            var Users = makandraContext.Users.AsEnumerable();
            Users ??= Enumerable.Empty<User>();
            return Users;
        }



        /// <summary>
        /// Imports a Json-string with <see cref="User"/>s in the database after Hashing the passed Password
        /// </summary>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger, Felix Nebel</author>
        public void Import(string jsonFile)
        {

            if (jsonFile is null) return;



            List<User> users = new List<User>();

            try
            {
                users = JsonSerializer.Deserialize<List<User>>(jsonFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }


            PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

            users.ForEach(e => {
                string pattern = @"\..*";
                string result = Regex.Replace(e.Email, pattern, "");
                User dummyUser = new User
                {
                    Email = result
                };
                e.Password = _passwordHasher.HashPassword(dummyUser, e.Password);
                AddUser(e);
            });
        }

    }
}
