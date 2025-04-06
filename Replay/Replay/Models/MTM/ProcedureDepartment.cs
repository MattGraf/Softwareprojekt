using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.Models.MTM
{
    /// <summary>
    /// Model for connection between the <see cref="Models.Procedure"/>s and <see cref="Models.Department"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class ProcedureDepartment
    {
        [Range(0, int.MaxValue)]
        public int ProcedureID {get; set;}
        public Procedure Procedure {get; set;}

        [Range(0, int.MaxValue)]
        public int DepartmentID {get; set;}
        public Department Department {get; set;}


        /// <summary>
        /// Overwrites the equals method
        /// Equals when the IDs are the same
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>If the Object equals this one</returns>
        /// <author>Matthias Grafberger</author>
        public override bool Equals(object obj)
        {
            var item = obj as ProcedureDepartment;

            if (item == null) return false;

            return this.DepartmentID == item.DepartmentID && this.ProcedureID == item.ProcedureID;
        }
        /// <summary>
        /// Checks if the ProcedureDepartment is allowed to be added
        /// </summary>
        /// <param name="procedureContainer">for ckecking if Procedure exists</param>
        /// <param name="departmentContainer">for ckecking if Department exists</param>
        /// <returns></returns>

        public bool IsValid(ProcedureContainer procedureContainer, DepartmentContainer departmentContainer){
            if(procedureContainer.getProcedureFromId(ProcedureID).Result is null || departmentContainer.GetDepartmentFromId(DepartmentID).Result is null){
                return false;
            }
            Procedure = procedureContainer.getProcedureFromId(ProcedureID).Result;
            Department = departmentContainer.GetDepartmentFromId(DepartmentID).Result;
            return true;
        }
    }
}