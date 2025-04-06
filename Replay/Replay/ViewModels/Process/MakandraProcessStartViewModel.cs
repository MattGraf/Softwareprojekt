using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models;
using Replay.Models.Account;

namespace Replay.ViewModels.Process
{
    public class MakandraProcessStartViewModel
    {
        /// <summary>
        /// The Id of the MakandraProcess that should be started
        /// </summary>
        public int ProcessId { get; set; }
        /// <summary>
        /// The Name of the Procedure. Defaults to the underlying MakandraProcess' name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The Deadline for the new Procedure
        /// </summary>
        [Required(ErrorMessage = "Das Zieldatum muss gesetzt sein")]
        [DisplayFormat(DataFormatString = "{0: dd.MM.yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Deadline { get; set; }
        /// <summary>
        /// The Id of the ContractType to be established with the new Procedure
        /// </summary>
        public int EstablishingContractType { get; set; }
        /// <summary>
        /// A List of Departments that the ReferencePerson is supposed to be part of when the new Procedure is finished
        /// </summary>
        public List<Department>? TargetDepartment { get; set; }
        /// <summary>
        /// The Id of the User referenced by the new Procedure 
        /// </summary>
        public int ReferencePerson { get; set; }
        /// <summary>
        /// The Id of the User responsible for the new Procedure
        /// </summary>
        public int ResponsiblePerson { get; set; }
        /// <summary>
        /// A List of MakandraTasks to be created for the new Procedure
        /// </summary>
        public List<MakandraTask>? MakandraTasks { get; set; }
        /// <summary>
        /// A DepartmentContainer
        /// </summary>
        public DepartmentContainer AllDepartments { get; set; } = new DepartmentContainer(new MakandraContext());
        /// <summary>
        /// A ContractTypesContainer
        /// </summary>
        public ContractTypesContainer AllContractTypes { get; set; } = new ContractTypesContainer(new MakandraContext());
        /// <summary>
        /// A UserContainer
        /// </summary>
        public UserContainer AllUsers { get; set; } = new UserContainer(new MakandraContext());
        /// <summary>
        /// A TaskTemplateContainer
        /// </summary>
        public TaskTemplateContainer AllTaskTemplates { get; set; } = new TaskTemplateContainer(new MakandraContext());
        /// <summary>
        /// The default User set as responsible for the new Procedure
        /// </summary>
        public User? DefaultResponsiblePerson { get; set; }
        /// <summary>
        /// An Array of DepartmentSelectionViewModels responsible for handling checkboxes in the corresponding Start View
        /// </summary>
        public DepartmentSelectionViewModel[] Departments { get; set; } = new DepartmentSelectionViewModel[new DepartmentContainer(new MakandraContext()).GetDepartments().Result.Count()];

        /// <summary>
        /// Creates a new MakandraProcessStartViewModel with a new Array of DepartmentSelectionViewModels, with all of the IsSelected boolean values initialized with false
        /// </summary>
        /// <author>Arian Scheremet</author>
        public void SetMakandraProcessStartViewModel()
        {
            int i = 0;
            foreach (var d in AllDepartments.GetDepartments().Result)
            {
                Departments[i] = new DepartmentSelectionViewModel
                {
                    Name = d.Name,
                    Id = d.Id,
                    IsSelected = false
                };
                i++;
            }
        }
    }
}