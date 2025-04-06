using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.SeedData
{
    /// <summary>
    /// Seeds initial <see cref="Department"/> for the dataabase
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public static class SeedDepartments
    {
        /// <summary>
        /// Creates six different <see cref="Depdartment"/> according to customer requirement
        /// and adds them to the database
        /// </summary>
        /// <author>Thomas Dworschak</author>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if (db.Departments.Any())
                {
                    return;
                }
                DepartmentContainer container = serviceProvider.GetRequiredService<DepartmentContainer>();

                string jsonString = "[\n" +
                    "  {\n" +
                    "    \"Id\": 1,\n" +
                    "    \"Name\": \"Entwicklung\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"Id\": 2,\n" +
                    "    \"Name\": \"Operations\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"Id\": 3,\n" +
                    "    \"Name\": \"UI/UX\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"Id\": 4,\n" +
                    "    \"Name\": \"Projektmanagement\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"Id\": 5,\n" +
                    "    \"Name\": \"Backoffice\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"Id\": 6,\n" +
                    "    \"Name\": \"People & Culture\"\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"Id\": 7,\n" +
                    "    \"Name\": \"Sales\"\n" +
                    "  }\n" +
                    "]";


                container.Import(jsonString);

            }
        }
    }
}
