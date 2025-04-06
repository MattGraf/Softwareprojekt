using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models.MTM;

namespace Replay.Models
{
    /// <summary>
    /// Possible ContractType
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class ContractType
    {
        public int ID {get; set;}
        public string Name {get; set;}
        [NotMapped]public virtual List<TaskTemplateContractType> TaskTemplateContractTypes {get; set;} = new List<TaskTemplateContractType>();

        /// <summary>
        /// Checks if a Duedate is valid to save it in the database
        /// </summary>
        /// <returns>0 if it's valid, a error value else</returns>
        /// <author>Matthias Grafberger</author>
        public int IsValid() {
            if (Name is null) return 1;
            return 0;
        }
    }
}