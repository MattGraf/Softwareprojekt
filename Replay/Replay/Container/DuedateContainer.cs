using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Replay.Data;
using Replay.Models;

using System.Text.Json;

namespace Replay.Container
{
    /// <summary>
    /// Managing the connection to the database of the <see cref="Duedate"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class DuedateContainer
    {

        private MakandraContext MakandraContext;

        /// <summary>
        /// Create new Container
        /// </summary>
        /// <param name="makandraContext">Context which is responsible for the database</param>
        /// <author>Matthias Grafberger</author>
        public DuedateContainer(MakandraContext makandraContext) {
            this.MakandraContext = makandraContext;
        }

        /// <summary>
        /// Add a Duedate in the database
        /// </summary>
        /// <param name="duedate">Duedate to be added</param>
        /// <author>Matthias Grafberger</author>
        public async void AddDuedate(Duedate duedate) {
            var DuedateWhenExists = await MakandraContext.Duedates
                .FirstOrDefaultAsync<Duedate>(s => s.Name == duedate.Name);
            
            if (DuedateWhenExists is not null) return;

            MakandraContext.Duedates.Add(duedate);
            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Returns the saved Duedates
        /// </summary>
        /// <returns>Saved Duedates</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<List<Duedate>> GetDuedates() {
            List<Duedate> Duedates = await MakandraContext.Duedates
                .OrderBy(s => s.ID)
                .ToListAsync();
            return Duedates;
        }

        /// <summary>
        /// Update a specific Duedate in the database
        /// </summary>
        /// <param name="duedate">Duedate to be changed</param>
        /// <author>Matthias Grafberger</author>
        public async void UpdateDuedate(Duedate duedate) {
            var duedateToUpdate = await MakandraContext.Duedates
                .FirstOrDefaultAsync<Duedate>(s => s.ID == duedate.ID);

            if (duedateToUpdate is null) AddDuedate(duedate);
            
            duedateToUpdate.Name = duedate.Name;
            duedateToUpdate.Days = duedate.Days;

            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Delete a specific Duedate in the database
        /// </summary>
        /// <param name="duedate">Duedate to be deleted</param>
        /// <author>Matthias Grafberger</author>
        public async void DeleteDuedate(Duedate duedate) {

            var duedateToDelete = await MakandraContext.Duedates
                .FirstOrDefaultAsync<Duedate>(s => s.ID == duedate.ID);
            
            MakandraContext.Remove(duedateToDelete);

            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Get a specific Duedate with a specific ID
        /// </summary>
        /// <param name="ID">ID of the wished Duedate</param>
        /// <returns>Duedate with wanted ID</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<Duedate> GetDuedateFromId(int ID) {

            var duedate = await MakandraContext.Duedates
                .FirstOrDefaultAsync<Duedate>(s => s.ID == ID);

            return duedate;
        }

        /// <summary>
        /// Returns if a duedate with the given name already exists in the database
        /// </summary>
        /// <param name="name">Wished name of the new Duedate</param>
        /// <returns>If a duedate with the given name already exists in the database</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<bool> DuedateNameExists(string name) {
            var Duedate = await MakandraContext.Duedates
                .FirstOrDefaultAsync<Duedate>(s => s.Name.Equals(name));

            return Duedate is not null;
        }

        /// <summary>
        /// Returns if a duedate exists with a specific name and another id
        /// </summary>
        /// <param name="name">Name of the new duedate which is to be added</param>
        /// <param name="ID">Id of duedate to exclude that not changing the name is not allowed</param>
        /// <returns>If a duedate exists with a specific name and another id</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<bool> DuedateNameExistsWithoutID(string name, int ID) {
            var Duedate = await MakandraContext.Duedates
                .FirstOrDefaultAsync<Duedate>(s => s.Name.Equals(name) && s.ID != ID);

            return Duedate is not null;
        }

        /// <summary>
        /// Imports a Json-string with <see cref="Duedate"/>s in the database
        /// </summary>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(string jsonFile) {

            if (jsonFile is null) return;

                
            List<Duedate> duedates = new List<Duedate>();

            try {
                duedates = JsonSerializer.Deserialize<List<Duedate>>(jsonFile);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            duedates.ForEach(e => {
                int h = e.IsValid();
                if (h == 0)
                {
                    AddDuedate(e);
                } else {
                    Console.WriteLine("Duedate couldn't added to database, because of a invalid state");
                }
            });
        }
    }
}