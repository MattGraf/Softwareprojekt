using Microsoft.EntityFrameworkCore;
using Replay.Container.Account;

namespace Replay.SeedData
{
    public static class SeedPermissions
    {
        public static void InitializeRoles(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if (db.Roles.Any())
                {
                    return;
                }
                PermissionContainer permissionContainer = new PermissionContainer(db);

                string jsonString = "[\r\n    {\r\n        \"Id\": 1,\r\n        \"Name\": \"Administrator\"\r\n    }]";

                permissionContainer.Import(jsonString);
            }
        }
    }
}
