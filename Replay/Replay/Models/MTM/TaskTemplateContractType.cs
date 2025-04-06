using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Replay.Models;

namespace Replay.Models.MTM
{
    /// <summary>
    /// Model for connection between the <see cref="Models.TaskTemplate"/>s and <see cref="Models.ContractType"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateContractType
    {
        [Range(0, int.MaxValue)]
        public int TaskTemplateID {get; set;}
        public TaskTemplate TaskTemplate {get; set;}
        [Range(0, 3)]
        public int ContractTypeID {get; set;}
        public ContractType ContractType {get; set;}

        /// <summary>
        /// Overwrites the equals method
        /// Equals when the IDs are the same
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>If the Object equals this one</returns>
        /// <author>Matthias Grafberger</author>
        public override bool Equals(object obj)
        {
            var item = obj as TaskTemplateContractType;

            if (item == null) return false;

            return this.TaskTemplateID == item.TaskTemplateID && this.ContractTypeID == item.ContractTypeID;
        }

        /// <summary>
        /// Checks if a TaskTemplateContractType is valid to save it in the database
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="contractTypesContainer">Container for the connection to the database</param>
        /// <returns>0 if it's valid, a error value else</returns>
        /// <author>Matthias Grafberger</author>
        public int IsValid(TaskTemplateContainer taskTemplateContainer, ContractTypesContainer contractTypesContainer) {
            TaskTemplate taskTemplate = taskTemplateContainer.GetTaskTemplateFromId(TaskTemplateID).Result;
            if (taskTemplate is null) {
                return 1;
            } else {
                TaskTemplate = taskTemplate;
            }
            ContractType contractType = contractTypesContainer.GetContractTypeFromID(ContractTypeID).Result;
            if (contractType is null) {
                return 2;
            } else {
                ContractType = contractType;
            }

            return 0;
        }

    }
}