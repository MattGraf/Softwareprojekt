using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.SeedData
{
    /// <summary>
    /// Seeds initial MakandraProcesses for the database
    /// </summary>
    public static class SeedMakandraProcesses
    {
        private static RoleContainer _roleContainer;
        private static MakandraProcessContainer _makandraProcessContainer;
        private static MakandraProcessRoleContainer _makandraProcessRoleContainer;

        public static async void InitializeMakandraProcesses(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if (db.Processes.Any())
                {
                    return;
                }

                _roleContainer = serviceProvider.GetRequiredService<RoleContainer>();
                _makandraProcessContainer = serviceProvider.GetRequiredService<MakandraProcessContainer>();
                _makandraProcessRoleContainer = serviceProvider.GetRequiredService<MakandraProcessRoleContainer>();
                
                string jsonString = "[{\n    \"Id\": 1,\n    \"Name\": \"Onboarding\",\n    \"Tasks\": [],\n    \"MakandraProcessRoles\": []\n}]";

                _makandraProcessContainer.Import(jsonString);
                InitializeMakandraProcessRoles();
            }
        }

        public static async void InitializeMakandraProcessRoles()
        {
            int personal = _roleContainer.GetRoleFromName("Personal").Result.Id;
            int businessLead = _roleContainer.GetRoleFromName("Geschäftsleitung").Result.Id;
            int admin = _roleContainer.GetRoleFromName("Administrator").Result.Id;

            string jsonString = "[\n" +
                "  {\n" +
                "    \"MakandraProcessId\": 1,\n" +
                "    \"RoleID\": " + personal + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"MakandraProcessId\": 1,\n" +
                "    \"RoleID\": " + businessLead + "\n" +
                "  },\n" +
                "  {\n" +
                "    \"MakandraProcessId\": 1,\n" +
                "    \"RoleID\": " + admin + "\n" +
                "  }\n" +
                "]";

            _makandraProcessRoleContainer.Import(_makandraProcessContainer, _roleContainer, jsonString);
        }
    }
}