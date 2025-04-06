using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.SeedData
{
    /// <summary>
    /// Seeds initial <see cref="Duedate"/> for the database
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class SeedDuedates
    {
        /// <summary>
        /// Creates seven different <see cref="Duedate"/> according to customer requirement
        /// and adds them to the database
        /// </summary>
        /// <author>Thomas Dworschak</author>
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if (db.Duedates.Any())
                {
                    return;
                }
                DuedateContainer duedates = serviceProvider.GetRequiredService<DuedateContainer>();

                string jsonString = "[\n" +
                    "  {\n" +
                    "    \"ID\": 1,\n" +
                    "    \"Name\": \"ASAP\",\n" +
                    "    \"Days\": 0\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 2,\n" +
                    "    \"Name\": \"2 Monate vor Antritt\",\n" +
                    "    \"Days\": -60\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 3,\n" +
                    "    \"Name\": \"2 Wochen vor Start\",\n" +
                    "    \"Days\": -14\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 4,\n" +
                    "    \"Name\": \"Am ersten Arbeitstag\",\n" +
                    "    \"Days\": 0\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 5,\n" +
                    "    \"Name\": \"3 Wochen nach Arbeitsbeginn\",\n" +
                    "    \"Days\": 21\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 6,\n" +
                    "    \"Name\": \"3 Monate nach Arbeitsbeginn\",\n" +
                    "    \"Days\": 90\n" +
                    "  },\n" +
                    "  {\n" +
                    "    \"ID\": 7,\n" +
                    "    \"Name\": \"6 Monate nach Arbeitsbeginn\",\n" +
                    "    \"Days\": 180\n" +
                    "  }\n" +
                    "]";
                
                duedates.Import(jsonString);
            }
        }
    }
}