using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Replay.Models
{
    /// <summary>Container class for <c>MakandraProcess</c> objects. Provides methods for database operations</summary>
    /// <author>Arian Scheremet</author>
    public class MakandraProcessContainer
    {
        /// <summary>
        /// The MakandraContext used in this container
        /// </summary>
        private MakandraContext db;

        /// <summary>
        /// Creates a new MakandraProcessContainer using a given MakandraContext
        /// </summary>
        /// <param name="db">A MakandraContext</param>
        /// <author>Arian Scheremet</author>
        public MakandraProcessContainer(MakandraContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Adds a MakandraProcess to the database
        /// </summary>
        /// <param name="makandraProcess">A MakandraProcess to be added to the database</param>
        /// <author>Arian Scheremet</author>
        public virtual async Task<int> AddProcess(MakandraProcess makandraProcess)
        {
            db.Processes.Add(makandraProcess);
            await db.SaveChangesAsync();

            return makandraProcess.Id;
        }

        /// <summary>
        /// Returns all MakandraProcesses that exist in the database
        /// </summary>
        /// <returns>A List of all found MakandraProcesses</returns>
        /// <author>Arian Scheremet</author>
        public async Task<List<MakandraProcess>> GetProcesses()
        {
            List<MakandraProcess> Processes = await db.Processes
                .OrderBy(s => s.Id)
                .ToListAsync();
            return Processes;
        }

        /// <summary>
        /// Updates the MakandraProcess in the database with a matching Id of the given MakandraProcess using all values of the given MakandraProcess
        /// </summary>
        /// <param name="makandraProcess">The MakandraProcess whose values should be used to update the MakandraProcess in the database</param>
        /// <author>Arian Scheremet</author>
        public async Task<MakandraProcess> UpdateProcess(MakandraProcess makandraProcess)
        {
            MakandraProcess ProcessToUpdate = GetProcessFromId(makandraProcess.Id).Result;

            ProcessToUpdate.Name = makandraProcess.Name;
            ProcessToUpdate.Tasks = makandraProcess.Tasks;
            ProcessToUpdate.MakandraProcessRoles = makandraProcess.MakandraProcessRoles;

            await db.SaveChangesAsync();

            return ProcessToUpdate;
        }

        /// <summary>
        /// Removes the MakandraProcess with a matching Id of the given MakandraProcess from the database
        /// </summary>
        /// <param name="makandraProcess">The MakandraProcess to be deleted from the database</param>
        /// <author>Arian Scheremet</author>
        public async void DeleteProcess(MakandraProcess makandraProcess)
        {
            var ProcessToDelete = await db.Processes
                .Where(s => s.Id == makandraProcess.Id)
                .FirstOrDefaultAsync<MakandraProcess>();

            db.Remove(ProcessToDelete);

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Returns the MakandraProcess found in the database with a given Id
        /// </summary>
        /// <param name="Id">An Id of the MakandraProcess to be retrieved from the database</param>
        /// <returns>The MakandraProcess with the given Id</returns>
        /// <author>Arian Scheremet</author>
        public async Task<MakandraProcess> GetProcessFromId(int Id)
        {
            var makandraProcess = await db.Processes
                .Where(s => s.Id == Id)
                .FirstOrDefaultAsync<MakandraProcess>();

            return makandraProcess;
        }

        /// <summary>
        /// Imports a Json-string with <see cref="MakandraProcess"/>s in the database
        /// </summary>
        /// <param name="jsonFile">Json-string to be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(string jsonFile) {

            if (jsonFile is null) return;
                
            List<MakandraProcess> makandraProcesses = new List<MakandraProcess>();

            try {
                makandraProcesses = JsonSerializer.Deserialize<List<MakandraProcess>>(jsonFile);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            makandraProcesses.ForEach(e => {
                int h = e.IsValid();
                if (h == 0)
                {
                    AddProcess(e);
                } else {
                    Console.WriteLine("MakandraProcess couldn't added to database, because of a invalid state");
                }
            });
        }
    }
}
