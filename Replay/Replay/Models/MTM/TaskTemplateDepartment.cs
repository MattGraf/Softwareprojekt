using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Replay.Models.MTM
{
    /// <summary>
    /// Model for connection between the <see cref="Models.TaskTemplate"/>s and <see cref="Models.Department"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateDepartment
    {
        [Range(0, int.MaxValue)]
        public int TaskTemplateID {get; set;}
        public TaskTemplate TaskTemplate {get; set;}

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
            var item = obj as TaskTemplateDepartment;

            if (item == null) return false;

            return this.DepartmentID == item.DepartmentID && this.TaskTemplateID == item.TaskTemplateID;
        }

         /// <summary>
        /// Checks if a TaskTemplate is valid to save it in the database
        /// </summary>
        /// <param name="duedateContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <returns>0 if it's valid, a error value else</returns>
        /// <author>Matthias Grafberger</author>
        public int IsValid(TaskTemplateContainer taskTemplateContainer, DepartmentContainer departmentContainer) {
            TaskTemplate taskTemplate = taskTemplateContainer.GetTaskTemplateFromId(TaskTemplateID).Result;
            if (taskTemplate is null) {
                return 1;
            } else {
                TaskTemplate = taskTemplate;
            }
            Department department = departmentContainer.GetDepartmentFromId(DepartmentID).Result;
            if (department is null) {
                return 2;
            } else {
                Department = department;
            }

            return 0;
        }
    }
}