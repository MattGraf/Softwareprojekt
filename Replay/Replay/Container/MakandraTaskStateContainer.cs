using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using Replay.Data;
using Replay.Models;

namespace Replay.Container
{
    /// <summary>
    /// Collection of methods to handle the association table between
    /// <see cref="MakandraTask"/> and <see cref="MakandraTaskState"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskStateContainer
    {
        private readonly MakandraContext _db;

        public MakandraTaskStateContainer(MakandraContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Adds a <see cref="MakandraTaskState"/> to the database.
        /// If the task state is already present in the database,
        /// the method returns immediately without effect
        /// </summary>
        /// <param name="state"><see cref="MakandraTaskState"/> that is supposed to be added</param>
        /// <author>Thomas Dworschak</author>
        public async Task AddMakandraTaskState(MakandraTaskState state)
        {
            var stateExists = await _db.States
                .FirstOrDefaultAsync<MakandraTaskState>(s => s.Id == state.Id);

            if (stateExists != null)
            {
                return;
            }

            _db.Add(state);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="MakandraTaskState"/> from the databse that
        /// matches the input Id.
        /// Throws an exception, if no task state in the database matches the id
        /// </summary>
        /// <param name="id">Id of the <see cref="MakandraTaskState"/> that is supposed to be retrieved</param>
        /// <returns><see cref="MakandraTaskState"/> that matches the id</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <author>Thomas Dworschak</author>
        public async Task<MakandraTaskState> GetMakandraTaskStateFromId(int id)
        {
            var state = await _db.States
                .FirstOrDefaultAsync<MakandraTaskState>(s => s.Id == id);

            if (state == null)
            {
                throw new KeyNotFoundException($"State with ID {id} not found.");
            }

            return state;
        }

        /// <summary>
        /// Retrieves a <see cref="MakandraTaskState"/> from the database that
        /// matches the input name.
        /// Throws an exception, if no task state is in the database that machtes the name
        /// </summary>
        /// <param name="name">Name of the <see cref="MakandraTaskState"/> that is supposed to be retrieved</param>
        /// <returns><see cref="MakandraTaskState"/> that machtes the name</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <author>Thomas Dworschak</author>
        public async Task<MakandraTaskState> GetMakandraTaskStateFromName(string name)
        {
            var state = await _db.States
                .FirstOrDefaultAsync<MakandraTaskState>(s => s.Name.Equals(name));

            if (state == null)
            {
                throw new KeyNotFoundException($"State with name {name} not found.");
            }

            return state;
        }

        /// <summary>
        /// Retrieves all <see cref="MakandraTaskState"/> task states that are currently
        /// saved in the database as a list
        /// </summary>
        /// <returns>List of <see cref="MakandraTaskState"/> states</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTaskState>> GetMakandraTaskStates()
        {
            List<MakandraTaskState> states = await _db.States
                .OrderBy(s => s.Id)
                .ToListAsync();

            return states;
        }
    }
}