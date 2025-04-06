using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.SeedData
{
    /// <summary>
    /// Seeds initial <see cref="MakandraTaskState"/> for the database
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public static class SeedStates
    {
        /// <summary>
        /// Creates three different <see cref="MakandraTaskState"/> according to customer requierment
        /// and adds them to the database
        /// </summary>
        /// <author>Thomas Dworschak</author>
        public static async void InitializeStates(IServiceProvider serviceProvider)
        {
            using (var db = new MakandraContext(serviceProvider.GetRequiredService<DbContextOptions<MakandraContext>>()))
            {
                if (db.States.Any())
                { 
                    return;
                }

                MakandraTaskStateContainer states = new MakandraTaskStateContainer(db);

                await states.AddMakandraTaskState(new MakandraTaskState
                {
                    Name = "Offen"
                });

                await states.AddMakandraTaskState(new MakandraTaskState
                {
                    Name = "In Bearbeitung"
                });

                await states.AddMakandraTaskState(new MakandraTaskState
                {
                    Name = "Erledigt"
                });
            }            
        }
    }
}