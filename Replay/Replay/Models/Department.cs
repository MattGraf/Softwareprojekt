using Replay.Models.MTM;

namespace Replay.Models
{
    public class Department
    {
        [Key]
        public int Id {get; set;}
        public string Name {set; get;}

        public virtual List<ProcedureDepartment> ProcedureDepartments {get; set;} = new List<ProcedureDepartment>();
        public virtual List<TaskTemplateDepartment> TaskTemplateDepartments {get; set;} = new List<TaskTemplateDepartment>();
        /// <summary>
        /// Checks if a Department is valid to save it in the database
        /// </summary>
        /// <returns>0 if it's valid, a error value else</returns>
        /// <author>Matthias Grafberger</author>
        public int IsValid() {
            if (Name is null) return 1;
            return 0;
        }
    }
}