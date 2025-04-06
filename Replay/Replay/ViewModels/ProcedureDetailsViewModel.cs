using Replay.Models;
using Replay.Models.Account;
using Replay.Container;
using Replay.Models.MTM;

namespace Replay.ViewModels
{
    /// <summary>
    /// ViewModel for the DetailsView
    /// </summary>
    /// <author>Florian Fendt</author>
    public class ProcedureDetailsViewModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime Deadline { get; set; }

        public string name { get; set; }

        public MakandraProcess basedProcess { get; set; }

        public ContractType? EstablishingContractType { get; set; }

        public List<Department> TargetDepartment { get; set; } = new List<Department>();

        public User ReferencePerson { get; set; }

        public User ResponsiblePerson { get; set; }

        public List<MakandraTask> makandraTasks { get; set; } = new List<MakandraTask>();
        public int openTasks { get; set;}
        public int completedTasks { get; set;}
        public int inprogressTasks { get; set;} 

        public double progressbar { get; set;}

    }
}