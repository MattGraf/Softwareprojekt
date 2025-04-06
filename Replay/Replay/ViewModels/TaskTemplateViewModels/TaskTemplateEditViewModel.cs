using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Replay.Models;
using Replay.Models.Account;

using Replay.Container.Account.MTM;

namespace Replay.ViewModels.TaskTemplateViewModels
{
    /// <summary>
    /// ViewModel for the editing of a <see cref="TaskTemplate"/>
    /// Contains only the attributes which are necessary
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateEditViewModel
    {
        public int ID {get; set;}
        [Required(ErrorMessage = "Ein Name muss angegeben werden")]
        public string Name {set; get;}
        public string? Instruction {get; set;}

        public ContractTypeViewModel[] ContractTypes {get; set;} = new ContractTypeViewModel[4];

        public List<DepartmentViewModel> Departments {get; set;} = new List<DepartmentViewModel>();

        public int Duedate {set; get;}

        public string DefaultResponsible {set; get;}

        public bool Archived {set; get;}

         [Range(1, int.MaxValue, ErrorMessage = "Es muss ein zugehöriger Prozess ausgewählt werden")]
        public int MakandraProcessId {get; set;}

        public List<Duedate> Duedates {get; set;} = new List<Duedate>();
        

        public bool Index {get; set;}

        public List<string> DefaultResponsibles {get; set;} = new List<string>();
        public List<TaskTemplateMakandraProcessViewModel> Processes {get; set;} = new List<TaskTemplateMakandraProcessViewModel>();

        /// <summary>
        /// Initialize the <see cref="ContractType"/>- and the <see cref="Department"/>-List
        /// </summary>
        /// <param name="departmentContainer">Container for the connection to the database</param>
        /// <author>Matthias Grafberger</author>
        public void Initialize(DepartmentContainer departmentContainer)
        {
            Departments = new List<DepartmentViewModel>();
            List<Department> departments = departmentContainer.GetDepartments().Result;

            ContractTypes[0] = new ContractTypeViewModel { Name = "Festanstellung" };
            ContractTypes[1] = new ContractTypeViewModel { Name = "Werkstudent" };
            ContractTypes[2] = new ContractTypeViewModel { Name = "Praktikum" };
            ContractTypes[3] = new ContractTypeViewModel { Name = "Trainee" };

            foreach (Department department in departments) {
                Departments.Add(new DepartmentViewModel {
                    Name = department.Name,
                });
            } 

        }

        /// <summary>
        /// Initialize the Process- and DefaultResponsible-List"/>
        /// </summary>
        /// <param name="user">Logged User</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="userRolesContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessRoleContainer"><Container for the connection to the database/param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <author>Matthias Grafberger</author>
        public void InitializeProcess(User user, RoleContainer roleContainer, UserRolesContainer userRolesContainer, MakandraProcessRoleContainer makandraProcessRoleContainer, MakandraProcessContainer makandraProcessContainer)
        {
            List<Role> roles = userRolesContainer.GetRolesFromUser(user).Result.ToList();

            List<int> makandraProcessIds = new List<int>();

            foreach (Role role in roles) {
                foreach(int id in makandraProcessRoleContainer.GetAssociatedMakandraProcessIdsFromRoleId(role.Id).Result) {
                    if (!makandraProcessIds.Contains(id)) makandraProcessIds.Add(id);
                }
            }

            List<MakandraProcess> makandraProcesses = new List<MakandraProcess>();

            foreach (int id in makandraProcessIds) {
                MakandraProcess makandraProcess = makandraProcessContainer.GetProcessFromId(id).Result;
                if (!makandraProcesses.Contains(makandraProcess)) makandraProcesses.Add(makandraProcess);
            }

            foreach (MakandraProcess makandraProcess in makandraProcesses) {
                Processes.Add(new TaskTemplateMakandraProcessViewModel{
                    ID = makandraProcess.Id,
                    Name = makandraProcess.Name
                });
            }

            Processes.OrderBy(e => e.Name);

            List<Role> rolesForDefaultResponsible = roleContainer.GetRoles().ToList();


            DefaultResponsibles.Add("Vorgangsverantwortlicher");
            DefaultResponsibles.Add("Bezugsperson");


            foreach (Role role in rolesForDefaultResponsible) {
                DefaultResponsibles.Add(role.Name);
            }

        }

        /// <summary>
        /// Checks for each <see cref="ContractType"/> if it is wanted for the <see cref="TaskTemplate"/>
        /// </summary>
        /// <returns>List of the wanted <see cref="ContractType"/>s for the <see cref="TaskTemplate"/></returns>
        /// <author>MatthiasGrafberger</author>
        public List<ContractType> GetContractTypes() {
            List<ContractType> contractTypes = new List<ContractType>();

            for (int i = 0; i < ContractTypes.Length; i++) {
                if (ContractTypes[i].IsSelected) {
                    foreach (ContractType contractType in ContractTypesContainer.ContractTypesList) {
                        if (ContractTypes[i].Name.Equals(contractType.Name)) contractTypes.Add(contractType);
                    }
                }
            }

            return contractTypes;
        }

        /// <summary>
        /// Checks for each <see cref="Department"/> if it is wanted for the <see cref="TaskTemplate"/>
        /// </summary>
        /// <param name="departmentContainer">Container for the connection to the database</param>
        /// <returns>List of the wanted <see cref="Department"/>s for the <see cref="TaskTemplate"/></returns>
        /// <author>MatthiasGrafberger</author>
        public List<Department> GetDepartments(DepartmentContainer departmentContainer) {
            List<Department> departmentList = new List<Department>();

            List<Department> departments = departmentContainer.GetDepartments().Result;

            foreach (DepartmentViewModel departmentViewModel in Departments) {
                if (departmentViewModel.IsSelected) {
                    foreach (Department department in departments) {
                        if (department.Name.Equals(departmentViewModel.Name)) departmentList.Add(department);
                    }
                }
            }

            return departmentList;
        }
    }
}