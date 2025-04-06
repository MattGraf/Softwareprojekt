using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using Replay.Models.Account;
using Replay.Models.MTM;
using Replay.Container;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Reflection.Metadata.Ecma335;

namespace Replay.Models
{
    /// <summary>
    /// Template of a specific task
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplate
    {
        [Key]
        public int ID {get; set;}

        [Required]
        public string Name {set; get;}

        public string? Instruction {set; get;}

        public virtual List<TaskTemplateContractType> TaskTemplateContractTypes {get; set;} = new List<TaskTemplateContractType>();

        [Required]
        public int DuedateID {get; set;}
        
        public Duedate Duedate {set; get;}
        
        [Required]
        public string DefaultResponsible {set; get;}

        public virtual List<TaskTemplateRole> EditAccess {set; get;} = new List<TaskTemplateRole>();

        [Required]
        public bool Archived {set; get;}

        public virtual List<TaskTemplateDepartment> TaskTemplateDepartments {get; set;} = new List<TaskTemplateDepartment>();
        
        [Required]
        public int MakandraProcessId {get; set;}

        
        /// <summary>
        /// Creates a new TaskTemplate and saves it in the database
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateContractTypeContainer">Container for the connection to the database</param>
        /// <param name="contractTypesContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateDepartmentContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessRoleContainer">Container for the connection to the database</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateRoleContainer">Container for the connection to the database</param>
        /// <param name="name">Name of the TaskTemplate</param>
        /// <param name="instruction">Instruction of the <see cref="TaskTemplate"/></param>
        /// <param name="contractTypes"><see cref="ContractType"/>s of the <see cref="TaskTemplate"/></param>
        /// <param name="departments"><see cref="Department"/>s of the <see cref="TaskTemplate"/></param>
        /// <param name="duedate"><see cref="Models.Duedate"/> of the <see cref="TaskTemplate"/></param>
        /// <param name="defaultResponsible">Default Responsible of the <see cref="TaskTemplate"/></param>
        /// <param name="archived">If the <see cref="TaskTemplate"/> is archived</param>
        /// <param name="makandraProcessId">The Id of the <see cref="MakandraProcess"> of which the <see cref="TaskTemplate"/> is a part of</param>
        /// <author>Matthias Grafberger</author>
        public static void CreateTaskTemplate(TaskTemplateContainer taskTemplateContainer, TaskTemplateContractTypeContainer taskTemplateContractTypeContainer, ContractTypesContainer contractTypesContainer, TaskTemplateDepartmentContainer taskTemplateDepartmentContainer, MakandraProcessContainer makandraProcessContainer, MakandraProcessRoleContainer makandraProcessRoleContainer, RoleContainer roleContainer, TaskTemplateRoleContainer taskTemplateRoleContainer, string name, string instruction, List<ContractType> contractTypes, List<Department> departments, Duedate duedate, string defaultResponsible, bool archived, int makandraProcessId) {

            TaskTemplate taskTemplate = new TaskTemplate {
                Name = name,
                Instruction = instruction,
                DuedateID = duedate.ID,
                Duedate = duedate,
                DefaultResponsible = defaultResponsible,
                Archived = archived,
                MakandraProcessId = makandraProcessId
            };

            taskTemplate.ID = taskTemplateContainer.AddTaskTemplate(taskTemplate);
            
            foreach (ContractType contractType in contractTypes) {
                TaskTemplateContractType taskTemplateContractType = new TaskTemplateContractType {
                    TaskTemplateID = taskTemplate.ID,
                    TaskTemplate = taskTemplate,
                    ContractTypeID = contractType.ID,
                    ContractType = contractTypesContainer.GetContractTypeFromID(contractType.ID).Result
                };

                taskTemplateContractTypeContainer.AddTaskTemplateContractType(taskTemplateContractType);
            }

            foreach (Department department in departments) {
                TaskTemplateDepartment taskTemplateDepartment = new TaskTemplateDepartment {
                    TaskTemplateID = taskTemplate.ID,
                    TaskTemplate = taskTemplate,
                    DepartmentID = department.Id,
                    Department = department
                };

                taskTemplateDepartmentContainer.AddTaskTemplateDepartment(taskTemplateDepartment);
            }

            int[] roleIds = makandraProcessRoleContainer.GetAssociatedRoleIDsFromMakandraProcess(makandraProcessContainer.GetProcessFromId(makandraProcessId).Result).Result;

            foreach (int id in roleIds) {
                taskTemplateRoleContainer.AddTaskTemplateRole(new TaskTemplateRole {
                    TaskTemplateID = taskTemplate.ID,
                    TaskTemplate = taskTemplate,
                    RoleID = id,
                    Role = roleContainer.GetRoleFromId(id).Result
                });
            }
        }    



        /// <summary>
        /// Creates a TaskTemplate and saves its changes in the database
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateContractTypeContainer">Container for the connection to the database</param>
        /// <param name="contractTypesContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateDepartmentContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessRoleContainer">Container for the connection to the database</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateRoleContainer">Container for the connection to the database</param>
        /// <param name="id">Id of the edited <see cref="TaskTemplate"/></param>
        /// <param name="name">(new) Name of the edited <see cref="TaskTemplate"/></param>
        /// <param name="instruction">(new) Instruction of the edited <see cref="TaskTemplate"/></param>
        /// <param name="contractTypes">(new) <see cref="ContractType"/>s of the edited <see cref="TaskTemplate"/></param>
        /// <param name="departments">(new) <see cref="Department"/>s of the edited <see cref="TaskTemplate"/></param>
        /// <param name="duedate">(new) <see cref="Models.Duedate"/> of the edited <see cref="TaskTemplate"/></param>
        /// <param name="defaultResponsible">(new) Default Responsible of the edited <see cref="TaskTemplate"/></param>
        /// <param name="archived">(new) Archived-state of the edited <see cref="TaskTemplate"/></param>
        /// <param name="makandraProcessId">(new) Id of the Process of the edited <see cref="TaskTemplate"/></param>
        /// <author>Matthias Grafberger</author>
        public static void EditTaskTemplate(TaskTemplateContainer taskTemplateContainer, TaskTemplateContractTypeContainer taskTemplateContractTypeContainer, ContractTypesContainer contractTypesContainer, TaskTemplateDepartmentContainer taskTemplateDepartmentContainer, MakandraProcessContainer makandraProcessContainer, MakandraProcessRoleContainer makandraProcessRoleContainer, RoleContainer roleContainer, TaskTemplateRoleContainer taskTemplateRoleContainer, int id, string name, string instruction, List<ContractType> contractTypes, List<Department> departments, Duedate duedate, string defaultResponsible, bool archived, int makandraProcessId) {
            
            TaskTemplate taskTemplate = new TaskTemplate {
                ID = id,
                Name = name,
                Instruction = instruction,
                DuedateID = duedate.ID,
                Duedate = duedate,
                DefaultResponsible = defaultResponsible,
                Archived = archived,
                MakandraProcessId = makandraProcessId
            };

            taskTemplate = taskTemplateContainer.UpdateTaskTemplate(taskTemplate).Result;

            taskTemplateContractTypeContainer.DeleteTaskTemplateContractTypeWithTaskTemplate(taskTemplate);
            taskTemplateDepartmentContainer.DeleteTaskTemplateDepartmentWithTaskTemplate(taskTemplate);
            
            foreach (ContractType contractType in contractTypes) {
                TaskTemplateContractType taskTemplateContractType = new TaskTemplateContractType {
                    TaskTemplateID = taskTemplate.ID,
                    TaskTemplate = taskTemplate,
                    ContractTypeID = contractType.ID,
                    ContractType = contractTypesContainer.GetContractTypeFromID(contractType.ID).Result
                };


                taskTemplate.TaskTemplateContractTypes.Add(taskTemplateContractType);
                taskTemplateContractTypeContainer.AddTaskTemplateContractType(taskTemplateContractType);
            }

            foreach (Department department in departments) {
                TaskTemplateDepartment taskTemplateDepartment = new TaskTemplateDepartment {
                    TaskTemplateID = taskTemplate.ID,
                    TaskTemplate = taskTemplate,
                    DepartmentID = department.Id,
                    Department = department
                };

                taskTemplate.TaskTemplateDepartments.Add(taskTemplateDepartment);
                taskTemplateDepartmentContainer.AddTaskTemplateDepartment(taskTemplateDepartment);
            }

            int[] roleIds = makandraProcessRoleContainer.GetAssociatedRoleIDsFromMakandraProcess(makandraProcessContainer.GetProcessFromId(makandraProcessId).Result).Result;

            taskTemplateRoleContainer.DeleteTaskTemplateRoleWithTaskTemplate(taskTemplate);

            foreach (int roleId in roleIds) {
                taskTemplateRoleContainer.AddTaskTemplateRole(new TaskTemplateRole {
                    TaskTemplateID = taskTemplate.ID,
                    TaskTemplate = taskTemplate,
                    RoleID = roleId,
                    Role = roleContainer.GetRoleFromId(id).Result
                });
            }
        }

        /// <summary>
        /// Checks if a TaskTemplate is valid to save it in the database
        /// </summary>
        /// <param name="duedateContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <returns>0 if it's valid, a error value else</returns>
        /// <author>Matthias Grafberger</author>
        public int IsValid(DuedateContainer duedateContainer, MakandraProcessContainer makandraProcessContainer) {
            if (Name is null) return 1;
            Duedate duedate = duedateContainer.GetDuedateFromId(DuedateID).Result;
            if (duedate is null) {
                return 2;
            } else {
                Duedate = duedate;
            }
            if (DefaultResponsible is null) return 3;
            if (makandraProcessContainer.GetProcessFromId(MakandraProcessId).Result is null) return 4;
            return 0;
        }

    }
}