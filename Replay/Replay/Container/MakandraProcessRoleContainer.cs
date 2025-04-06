using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models;
using Replay.Models.Account;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using Replay.Models.MTM;
using Replay.Models;
using Replay.Models.Account;

namespace Replay.Container
{

    /// <summary>
    /// Management of the many-to-many-table between <see cref="Models.MakandraProcess"/> and <see cref="Models.Account.Role"/>
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class MakandraProcessRoleContainer
    {
        private MakandraContext MakandraContext;

        /// <summary>
        /// Creates new Container
        /// </summary>
        /// <param name="MakandraContext">Context which is responsible for the database</param>
        /// <author>Matthias Grafberger</author>
        public MakandraProcessRoleContainer(MakandraContext MakandraContext)
        {
            this.MakandraContext = MakandraContext;
        }

        /// <summary>
        /// Adds a new connection between a <see cref="Models.MakandraProcess"/> and a <see cref="Models.Account.Role"/> in the database when it doesn't exist yet
        /// </summary>
        /// <param name="MakandraProcessRole">Contain the connection which should be added</param>
        /// <author>Matthias Grafberger</author>
        public async void AddMakandraProcessRole(MakandraProcessRole MakandraProcessRole)
        {
            var MakandraProcessRoleWhenExists = await MakandraContext.MakandraProcessRoles
                .FirstOrDefaultAsync<MakandraProcessRole>(s => s.Equals(MakandraProcessRole));

            if (MakandraProcessRoleWhenExists is not null) return;

            MakandraContext.Add(MakandraProcessRole);
            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a connection between a <see cref="Models.MakandraProcess"/> and a <see cref="Models.Account.Role"/> in the database
        /// </summary>
        /// <param name="MakandraProcessRole">Contains the connection to be deleted</param>
        /// <author>Matthias Grafberger</author>
        public async void DeleteMakandraProcessRole(MakandraProcessRole MakandraProcessRole)
        {
            var MakandraProcessRoleToDelete = await MakandraContext.MakandraProcessRoles

                .FirstOrDefaultAsync<MakandraProcessRole>(s => s.Equals(MakandraProcessRole));

            if (MakandraProcessRoleToDelete is null) return;

            MakandraContext.Remove(MakandraProcessRole);

            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes all old connections between a <see cref="MakandraProcess"/> and a <see cref="Role"/> in the database
        /// </summary>
        /// <param name="makandraProcess"><see cref="MakandraProcess"/> of which the connections are deleted</param>
        /// <author>Matthias Grafberger, Arian Scheremet</author>
        public void DeleteMakandraProcessRoleWithMakandraProcess(MakandraProcess makandraProcess)
        {
            List<MakandraProcessRole> MakandraProcessRolesToDelete = MakandraContext.MakandraProcessRoles
                .Where(s => s.MakandraProcessId == makandraProcess.Id)
                .ToList();

            if (MakandraProcessRolesToDelete is null) return;

            foreach (MakandraProcessRole makandraProcessRole in MakandraProcessRolesToDelete)
            {
                MakandraContext.Remove(makandraProcessRole);
            }

            MakandraContext.SaveChanges();
        }

        /// <summary>
        /// Returns the Ids of all Roles associated with a given MakandraProcess
        /// </summary>
        /// <param name="MakandraProcess">MakandraProcess from which the connections are needed</param>
        /// <returns>An Array of Ids of the associated Roles</returns>
        /// <author>Matthias Grafberger, Arian Scheremet</author>
        public async Task<int[]> GetAssociatedRoleIDsFromMakandraProcess(MakandraProcess MakandraProcess)
        {
            List<MakandraProcessRole> MakandraProcessRoles = await MakandraContext.MakandraProcessRoles
                .Where(s => s.MakandraProcessId == MakandraProcess.Id)
                .OrderBy(s => s.RoleID)
                .ToListAsync();

            int[] Ids = new int[MakandraProcessRoles.Count()];
            int i = 0;
            foreach (var v in MakandraProcessRoles)
            {
                Ids[i] = v.RoleID;
                i++;
            }

            return Ids;
        }

        /// <summary>
        /// Returns the Roles that have Access to the Process
        /// </summary>
        /// <param name="Process">the process of which the roles are required</param>
        /// <returns>List of Roles with access to the Process</returns>
        /// <author>Florian Fendt</author>
        public virtual async Task<List<Role>> GetRolesFromProcess(MakandraProcess Process)
        {
            var MakandraProcessRoles = await MakandraContext.MakandraProcessRoles
                .Where(x => x.MakandraProcessId == Process.Id)
                .Select(x => x.Role)
                .ToListAsync();
            return MakandraProcessRoles;
        }

        /// <summary>
        /// Returns the Processes that the provided role has access to
        /// </summary>
        /// <param name="role">the roel of which the proceses are required</param>
        /// <returns>List of Processes</returns>
        /// <author>Florian Fendt</author>
        public async Task<List<MakandraProcess>> GetProcessFromRoles(Role role)
        {
            var MakandraProcesses = await MakandraContext.MakandraProcessRoles
                .Where(x => x.RoleID == role.Id)
                .Select(x => x.MakandraProcess)
                .ToListAsync();
            return MakandraProcesses;
        }

        public async Task<int[]> GetAssociatedMakandraProcessIdsFromRoleId(int roleId)
        {
            List<MakandraProcessRole> MakandraProcessRoles = await MakandraContext.MakandraProcessRoles
                .Where(s => s.RoleID == roleId)
                .OrderBy(s => s.MakandraProcessId)
                .ToListAsync();

            int[] Ids = new int[MakandraProcessRoles.Count()];
            int i = 0;
            foreach (var v in MakandraProcessRoles)
            {
                Ids[i] = v.MakandraProcessId;
                i++;
            }

            return Ids;
        }

        /// <summary>
        /// Imports a Json-string with <see cref="MakandraProcessRole"/>s in the database
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(MakandraProcessContainer makandraProcessContainer, RoleContainer roleContainer, string jsonFile) {

            if (jsonFile is null) return;

                
            List<MakandraProcessRole> makandraProcessRoles = new List<MakandraProcessRole>();

            try {
                makandraProcessRoles = JsonSerializer.Deserialize<List<MakandraProcessRole>>(jsonFile);
            } catch (InvalidOperationException e) {
                return;
            }

            makandraProcessRoles.ForEach(e => {

                int h = e.IsValid(makandraProcessContainer, roleContainer);
                if (h == 0) {
                    AddMakandraProcessRole(e);
                } else {
                    Console.WriteLine(h);
                    Console.WriteLine("MakandraProcessRole couldn't added to database, because of a invalid state");
                }
            });
        }
    }
}