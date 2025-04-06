using Replay.Models;
using Replay.Models.Account;
using Replay.Container;
using Replay.Models.MTM;

namespace Replay.ViewModels
{
    /// <summary>
    /// ViewModel for the EditView
    /// </summary>
    /// <author>Florian Fendt</author>
    public class ProcedureEditViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required, DisplayName("Zieldatum")]
        public DateTime Deadline { get; set; }
        [Required, DisplayName("Name")]
        public string name { get; set; }
        public List<TaskTemplate> TaskTemplates { get; set; } = new List<TaskTemplate>();
        
        [DisplayName("Aufgaben")]
        public List<MakandraTask> makandraTasks { get; set; } = new List<MakandraTask>();
        public int openTasks { get; set;}
        public int completedTasks { get; set;}
        public int inprogressTasks { get; set;} 

        public double progressbar { get; set;}
        [DisplayName("Vertragsart")]
        public List<ContractType> ContractTypes { get; set; } = new List<ContractType>();
        [Required]
        public int EstablishingContractTypeId { get; set; }
        [Required, DisplayName("Zielabteilung/en")]
        public List<DepartmentViewModel> TargetDepartments { get; set; } = new List<DepartmentViewModel>();
        [Required, DisplayName("Zielperson")]
        public int ReferencePersonId { get; set; }
        [Required, DisplayName("Vorgangsverantwortlicher")]
        public int ResponsiblePersonId { get; set; }

        public bool Departmenterror {get; set; } = false; 
        public List<User> PossibleResponsiblePersons { get; set; } = new List<User>();
        public List<User> PossibleReferencePersons { get; set; } = new List<User>();

        public string? NewTasks { get; set; } = "[]";
        public string? RemovedTasks { get; set; } = "[]";

    /// <summary>
    /// Gets all Departments that were selected in the edit view
    /// </summary>
    /// <param name="departmentContainer">Container to use the database</param>
    /// <returns>List of Departments</returns>
    /// <author>Florian Fendt</author>
    public async Task<List<Department>> GetDepartments(DepartmentContainer departmentContainer){
            List<Department> departmentList = new List<Department>();
            List<Department> departments = await departmentContainer.GetDepartments();

            foreach (var dpvm in TargetDepartments) {
                if (dpvm.IsSelected) {
                    foreach (var dp in departments) {
                        if (dp.Name.Equals(dpvm.Name)){
                            departmentList.Add(dp);
                        }
                    }
                }
            }
            Console.WriteLine("editviewmodel");
            foreach(var element in departmentList){
                Console.WriteLine(element.Name);
            }

            return departmentList;
        }
    }

}