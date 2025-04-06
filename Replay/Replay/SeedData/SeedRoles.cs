using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Replay.Container.Account;
using Replay.Container.Account.MTM;
using Replay.Models.Account;
using Replay.Models.Account.MTM;

namespace Replay.SeedData
{
    /// <summary>
    /// Seed initial <see cref="Role"/> for the database
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public static class SeedRoles
    {
        /// <summary>
        /// Creates five different <see cref="Role"/> according to customer requirement
        /// and adds them to the database
        /// </summary>
        /// <author>Thomas Dworschak</author>
        public static async void InitializeRoles(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if(db.Roles.Any())
                {
                    return;
                }

                RoleContainer roles = new RoleContainer(db);
                RolePermissionContainer rolePermissions = new RolePermissionContainer(db);
                PermissionContainer permissionContainer = new PermissionContainer(db);


                string jsonString = "[\r\n    {\r\n        \"Id\": 1,\r\n        \"Name\": \"Administrator\"\r\n    },\r\n    {\r\n        \"Id\": 2,\r\n        \"Name\": \"IT\"\r\n    },\r\n    {\r\n        \"Id\": 3,\r\n        \"Name\": \"Backoffice\"\r\n    },\r\n    {\r\n        \"Id\": 4,\r\n        \"Name\": \"Geschäftsleitung\"\r\n    },\r\n    {\r\n        \"Id\": 5,\r\n        \"Name\": \"Personal\"\r\n    }\r\n]";

                roles.Import(jsonString);

                rolePermissions.AddRolePermission(new RolePermission()
                {
                    PermissionId = permissionContainer.GetPermissionFromName("Administrator").Id,
                    Permission = await permissionContainer.GetPermissionFromName("Administrator"),
                    RoleId = roles.GetRoleFromName("Administrator").Id,
                    Role = await roles.GetRoleFromName("Administrator")
                });

            }
        }
    }
}