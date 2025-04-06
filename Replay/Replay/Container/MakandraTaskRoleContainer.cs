using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models.MTM;
using Microsoft.EntityFrameworkCore;
using Replay.Models;

namespace Replay.Container
{
    /// <summary>
    /// Collection of methods to maange the database table that
    /// associates <see cref="MakandraTask"/> and <see cref="Role"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskRoleContainer
    {
        private readonly MakandraContext _db;

        public MakandraTaskRoleContainer(MakandraContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Adds a <see cref="MakandraTaskRole"/> to the database.
        /// If such a task role exists already, the method
        /// returns immediately without effect
        /// </summary>
        /// <param name="taskRole"><see cref="MakandraTaskRole"/> that is supposed to be added</param>
        /// <author>Thomas Dworschak</author>
        public async void AddTaskRole(MakandraTaskRole taskRole)
        {
            var taskRoleExists = await _db.TaskRoles
                .FirstOrDefaultAsync<MakandraTaskRole>(s => s.Equals(taskRole));

            Console.WriteLine("TEST------------------------------------" + taskRoleExists is null);

            if (taskRoleExists is not null)
            {
                return;
            }

            _db.Add(taskRole);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a <see cref="MakandraTaskRole"/> form the database.
        /// If no such task role is in the database, the method
        /// returns immediately without effect
        /// </summary>
        /// <param name="taskRole"><see cref="MakandraTaskRole"/> that is supposed to be removed</param>
        /// <author>Thomas Dworschak</author>
        public async void DeleteTaskRole(MakandraTaskRole taskRole)
        {
            var taskRoleToDelete = await _db.TaskRoles
                .FirstOrDefaultAsync<MakandraTaskRole>(s => s.Equals(taskRole));

            if (taskRoleToDelete is null)
            {
                return;
            }

            _db.Remove(taskRole);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a list of <see cref="Role"/> roles from the database
        /// that have their Id matched in the <c>EditAccess</c> attribute
        /// of the input <see cref="MakandraTask"/>
        /// </summary>
        /// <param name="task"><see cref="MakandraTask"/> whose associated roles that have edit access are to be retrieved</param>
        /// <returns>List of roles that have edit access to the input task</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTaskRole>> GetRolesFromTask(MakandraTask task)
        {
            List<MakandraTaskRole> TaskRoles = await _db.TaskRoles
                .Where(s => s.TaskId == task.Id)
                .OrderBy(s => s.RoleId)
                .ToListAsync();

            return TaskRoles;            
        }
    }
}