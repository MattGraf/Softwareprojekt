using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Replay.Container.Account.MTM;
using Replay.Models.Account;
using Replay.Models.Account.MTM;

namespace Replay.SeedData
{
    /// <summary>
    /// Seeds initial <see cref="User"/> for the database
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public static class SeedUsers
    {
        /// <summary>
        /// Creates six different <see cref="User"/> according to customer requirement
        /// and adds them to the database.
        /// For each user the many-to-many relaiton between <see cref="User"/> and <see cref="Role"/>
        /// is created
        /// </summary>
        /// <author>Thomas Dworschak</author>
        public static async void InitializeUser(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if (db.Users.Any())
                { 
                    return;
                }

                RoleContainer roles = new RoleContainer(db);
                UserContainer users = new UserContainer(db);
                UserRolesContainer usersRoles = new UserRolesContainer(db);



                string jsonString = "[\r\n    {\r\n        \"Id\": 1,\r\n        \"Email\": \"admin@replay.de\",\r\n        \"FullName\": \"Admin\",\r\n        \"Password\": \"Admin1!\",\r\n        \"Active\": true\r\n    },\r\n    {\r\n        \"Id\": 2,\r\n        \"Email\": \"koenig@replay.de\",\r\n        \"FullName\": \"König v. Augsburg\",\r\n        \"Password\": \"GHLutz13!\",\r\n        \"Active\": true\r\n    },\r\n    {\r\n        \"Id\": 3,\r\n        \"Email\": \"gerhard@replay.de\",\r\n        \"FullName\": \"Gerhard\",\r\n        \"Password\": \"Gerhard1!\",\r\n        \"Active\": true\r\n    },\r\n    {\r\n        \"Id\": 4,\r\n        \"Email\": \"bill@replay.de\",\r\n        \"FullName\": \"Bill Yard\",\r\n        \"Password\": \"B1llard!\",\r\n        \"Active\": true\r\n    },\r\n    {\r\n        \"Id\": 5,\r\n        \"Email\": \"karl@replay.de\",\r\n        \"FullName\": \"Karl Toffel\",\r\n        \"Password\": \"Erdapfel1!\",\r\n        \"Active\": true\r\n    },\r\n    {\r\n        \"Id\": 6,\r\n        \"Email\": \"wilma@replay.de\",\r\n        \"FullName\": \"Wilma Ruhe\",\r\n        \"Password\": \"Pssssst1!\",\r\n        \"Active\": true\r\n    }\r\n]";
                users.Import(jsonString);


                usersRoles.AddUserRole(new UserRole
                {
                    UserId = users.GetUserFromEmail("admin@replay.de").Id,
                    User = await users.GetUserFromEmail("admin@replay.de"),
                    RoleId = roles.GetRoleFromName("Administrator").Id,
                    Role = await roles.GetRoleFromName("Administrator")
                });

                usersRoles.AddUserRole(new UserRole
                {
                    UserId = users.GetUserFromEmail("koenig@replay.de").Id,
                    User = await users.GetUserFromEmail("koenig@replay.de"),
                    RoleId = roles.GetRoleFromName("Geschäftsleitung").Id,
                    Role = await roles.GetRoleFromName("Geschäftsleitung")
                });

                usersRoles.AddUserRole(new UserRole
                {
                    UserId = users.GetUserFromEmail("gerhard@replay.de").Id,
                    User = await users.GetUserFromEmail("gerhard@replay.de"),
                    RoleId = roles.GetRoleFromName("Backoffice").Id,
                    Role = await roles.GetRoleFromName("Backoffice")
                });

                usersRoles.AddUserRole(new UserRole
                {
                    UserId = users.GetUserFromEmail("bill@replay.de").Id,
                    User = await users.GetUserFromEmail("bill@replay.de"),
                    RoleId = roles.GetRoleFromName("Backoffice").Id,
                    Role = await roles.GetRoleFromName("Backoffice")
                });

                usersRoles.AddUserRole(new UserRole
                {
                    UserId = users.GetUserFromEmail("bill@replay.de").Id,
                    User = await users.GetUserFromEmail("bill@replay.de"),
                    RoleId = roles.GetRoleFromName("Personal").Id,
                    Role = await roles.GetRoleFromName("Personal")
                });

                usersRoles.AddUserRole(new UserRole
                {
                    UserId = users.GetUserFromEmail("karl@replay.de").Id,
                    User = await users.GetUserFromEmail("karl@replay.de"),
                    RoleId = roles.GetRoleFromName("IT").Id,
                    Role = await roles.GetRoleFromName("IT"),
                });
            }
        }
    }
}