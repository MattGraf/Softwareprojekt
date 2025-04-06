using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Replay.Models.MTM;

namespace Replay.Container
{

    /// <summary>
    /// Management of the many-to-many-table between <see cref="Models.Procedure"/> and <see cref="Models.Department"/>
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class ProcedureDepartmentContainer
    {
        private MakandraContext MakandraContext;

        /// <summary>
        /// Creates new Container
        /// </summary>
        /// <param name="MakandraContext">Context which is responsible for the database</param>
        /// <author>Matthias Grafberger, Florian Fendt</author>
        public ProcedureDepartmentContainer(MakandraContext MakandraContext) {
            this.MakandraContext = MakandraContext;
        }

        /// <summary>
        /// Adds a connection between a <see cref="Procedure"/> and a <see cref="Department"/> in the database
        /// </summary>
        /// <param name="ProcedureDepartment">Contains the connection to be added</param>
        /// <author>Matthias Grafberger</author>
        public async Task AddProcedureDepartment(ProcedureDepartment ProcedureDepartment) {
            var ProcedureDepartmentTypeWhenExists = await MakandraContext.ProcedureDepartments
                .FirstOrDefaultAsync<ProcedureDepartment>(s => s.Equals(ProcedureDepartment));
            
            if (ProcedureDepartmentTypeWhenExists is not null) return;

            MakandraContext.Add(ProcedureDepartment);
            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a connection between a <see cref="Procedure"/> and a <see cref="Department"/> in the database
        /// </summary>
        /// <param name="ProcedureDepartment">Contains the connection to be deleted</param>
        /// <author>Matthias Grafberger</author>
        public async Task DeleteProcedureDepartment(ProcedureDepartment ProcedureDepartment) {
            var ProcedureDepartmentToDelete = await MakandraContext.ProcedureDepartments

                .FirstOrDefaultAsync<ProcedureDepartment>(s => s.Equals(ProcedureDepartment));

            if (ProcedureDepartmentToDelete is null) return;
            
            MakandraContext.Remove(ProcedureDepartmentToDelete);

            await MakandraContext.SaveChangesAsync();
        }
        /// <summary>
        /// Gets all ProcedureDepartments based on the provided ProcedureId
        /// </summary>
        /// <param name="id">Id of the Procedure</param>
        /// <returns>returns a list of ProcedureDepartments</returns>
        /// <author>Florian Fendt</author>
        public async Task<List<ProcedureDepartment>> GetProcedureDepartmentfromProcedureId(int id){
            var result = await MakandraContext.ProcedureDepartments
                .Where(x => x.ProcedureID == id)
                .ToListAsync();
            return result;
        }
        /// <summary>
        /// Imports the ProcedureDepartments from the JsonString
        /// </summary>
        /// <param name="procedureContainer">Container needed for IsValid function</param>
        /// <param name="departmentContainer">Container needed for IsValid function</param>
        /// <param name="json">Json String that includes ProcedureDepartments</param>
        /// <author>Florian Fendt</author>
        public async void Import(ProcedureContainer procedureContainer, DepartmentContainer departmentContainer, string json){
            if(json == null){
                return;
            }
            List<ProcedureDepartment> proceduredepartmentList = new List<ProcedureDepartment>();
            try {
                proceduredepartmentList = JsonSerializer.Deserialize<List<ProcedureDepartment>>(json);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return;
            }
            foreach(ProcedureDepartment t in proceduredepartmentList){
                Console.WriteLine("................:" + t);
                bool result = t.IsValid(procedureContainer, departmentContainer);
                if(result == false){
                    break;
                    return;
                } else {
                    await AddProcedureDepartment(t);
                }
            }
        }
    }
}