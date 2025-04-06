using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.SeedData
{
    /// <summary>
    /// Seeds initial <see cref="ContractType"/> for the database
    /// </summary>
    public static class SeedContractTypes
    {
        /// <summary>
        /// Creates four different <see cref="ContractType"/> based on customre requirements
        /// and adds them to the database
        /// </summary>
        /// <author>Thomas Dworschak</author>
        public static void InitializeContractTypes(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if (db.ContractTypes.Any())
                {
                    return;
                }

                ContractTypesContainer contractTypes = serviceProvider.GetRequiredService<ContractTypesContainer>();

                string jsonString = "[\n" +
                    "  {\n" +
                    "    \"ID\": 1,\n" +
                    "    \"Name\": \"Festanstellung\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 2,\n" +
                    "    \"Name\": \"Werkstudent\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 3,\n" +
                    "    \"Name\": \"Praktikum\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 4,\n" +
                    "    \"Name\": \"Trainee\"\n" +
                    "  }\n" +
                    "]";

                contractTypes.Import(jsonString);
            }
        }
    }
}