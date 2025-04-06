using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Storage;
using OpenQA.Selenium.DevTools.V123.Debugger;
using Replay.Models;
using Replay.Models.Account;
using Replay.Models.MTM;

namespace Replay.Models
{
    /// <summary>
    /// Procedure that inherits from Process
    /// </summary>
    /// <author>Florian Fendt</author>
    public class Procedure
    {
        /// <summary>
        /// Primary key for a Procedure, Auto-Increment by Database
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Date on which the Procedure has to be completed
        /// </summary>
        [Required(ErrorMessage = "Es muss ein Datum festgelegt sein")]
        public DateTime Deadline { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Der Name darf keine leere Zeichenkette sein")]
        /// <summary>
        /// Name of the Procedure
        /// </summary>
        public string name { get; set; } 

        [Required(ErrorMessage = "Es muss ein Vertragstyp angegeben werden")]
        /// <summary>
        /// ContractType of the Procedure
        /// </summary>
        public ContractType EstablishingContractType { get; set; }

        /// <summary>
        /// Id of the ContractType of the Procedure
        /// </summary>
        public int EstablishingContractTypeId { get; set; }

        /// <summary>
        /// Departments which the ReferencePerson should be part of        
        /// /// </summary>
        
        [Required(ErrorMessage = "Es muss eine Zielabteilung angegeben werden")]
        public List<Department> TargetDepartment { get; set; } = new List<Department>();

        /// <summary>
        /// Person that is the target of the Procedure
        /// </summary>
        [Required(ErrorMessage = "Es muss eine Zielperson für den Vorgang ausgewählt werden")]
        public int ReferencePersonId { get; set; }

        /// <summary>
        /// Person that is responsible for the Procedure
        /// </summary>
        [Required(ErrorMessage = "Es muss ein Verantwortlicher für den Vorgang bestimmt werden")]
        public int ResponsiblePersonId { get; set; }

        /// <summary>
        /// Process which the Procedure is based on
        /// </summary>
        public int basedProcessId { get; set; }

        /// <summary>
        /// List of Tasks that are part of the Procedure and need to be finished in order to complete the Procedure
        /// </summary>
        [Required(ErrorMessage = "Ein vorgang muss mindestens eine Aufgabe enthalten")] 
        public List<MakandraTask> makandraTasks { get; set; } = new List<MakandraTask>();

        /// <summary>
        /// needed for the Many to Many Relation bewtween Procedure and Departments
        /// </summary>
        public virtual List<ProcedureDepartment> ProcedureDepartments {get; set;} = new List<ProcedureDepartment>();

        /// <summary>
        /// Determines if the Procedure is archived
        /// </summary>
        public bool Archived {get; set;} = false;
        /// <summary>
        /// Number of completed Tasks
        /// </summary>
        public int completedTasks {get; set;}
        /// <summary>
        /// Number of open Tasks
        /// </summary>
        public int openTasks {get; set;}
        /// <summary>
        /// Number of Tasks that are in progress
        /// </summary>
        public int inprogressTasks {get; set;}
        /// <summary>
        /// Used to show visually how far the Procedure is completed
        /// </summary>
        public double progressbar {get; set;}

        public Procedure(){}
        /// <summary>
        /// Creates a Procedure
        /// </summary>
        /// <param name="process">The Process the procedure is created from</param>
        /// <param name="Deadline">The Deadline of the Procedure</param>
        /// <param name="ReferencePerson">The Person that is the target of the Procedure</param>
        /// <param name="ResponsiblePerson">The Person responsible for the Procedure</param>
        /// <param name="name">The name of the Procedure</param>
        /// <param name="EstablishingContractType">The ContractType of the Procedure</param>
        /// <param name="TargetDepartments">The Departments the ReferencePerson should be part of</param>
        /// <param name="taskContainer">The Container for managing the database for the tasks to be created</param>
        /// <param name="taskStateContainer">The Container for managing the database for the states of the  tasks to be created</param>
        /// <param name="taskRoles">The Container for managing the database for roles for the tasks</param>
        /// <param name="duedates">The Container for managing the database for the dates for the tasks</param>
        /// <param name="procedures">The Container for managing the database for procedures</param>
        /// <param name="roleContainer">The Container for managing the database with roles</param>
        /// <param name="userContainer">The Container for managing the database with users</param>
        /// <param name="taskTemplateDepartmentContainer">The Container for managing the tasktemplates Department Many to many relation</param>
        /// <param name="taskTemplateContractTypeContainer">The Container for managing the tasktemplates ContractTypes Many to many relation</param>
        /// <returns>a Procedure object</returns>
        /// <exception cref="ArgumentNullException"><Exception thronw if provieded parameter is null/exception>
        /// <author>Florian Fendt</author>

        public async static Task<Procedure> CreateProcedure(MakandraProcess process, DateTime Deadline, User ReferencePerson, User ResponsiblePerson, string name, ContractType EstablishingContractType, List<Department> TargetDepartments, MakandraTaskContainer taskContainer, MakandraTaskStateContainer taskStateContainer,
        MakandraTaskRoleContainer taskRoles, DuedateContainer duedates, ProcedureContainer procedures, RoleContainer roleContainer, UserContainer userContainer, TaskTemplateDepartmentContainer taskTemplateDepartmentContainer, TaskTemplateContractTypeContainer taskTemplateContractTypeContainer, ProcedureDepartmentContainer procedureDepartmentContainer, DepartmentContainer departmentContainer)
        {
            Procedure procedure = new Procedure();

            if(process == null)
            {
                throw new ArgumentNullException("Procedure - process is null - Constructor");
            }
            if(Deadline == null){
                throw new ArgumentNullException("Procedure - Deadline is null - Constructor");
            }
            procedure.EstablishingContractType = EstablishingContractType;
            procedure.EstablishingContractTypeId = EstablishingContractType.ID;
            procedure.TargetDepartment = TargetDepartments;
            procedure.name = name;
            procedure.Deadline = Deadline;
            procedure.basedProcessId = process.Id;
            procedure.ReferencePersonId = ReferencePerson.Id;
            procedure.ResponsiblePersonId = ResponsiblePerson.Id;
             List<int> taskids = new List<int>();
            procedure.completedTasks = 0;
            procedure.openTasks = procedure.makandraTasks.Count; 
            procedure.inprogressTasks = 0;
            procedure.progressbar = 0.0;
            procedure.Id = await procedures.addProcedure(procedure);

            foreach (Department d in TargetDepartments) {
                procedureDepartmentContainer.AddProcedureDepartment(new ProcedureDepartment {
                    ProcedureID = procedure.Id,
                    Procedure = procedures.getProcedureFromId(procedure.Id).Result,
                    DepartmentID = d.Id,
                    Department = departmentContainer.GetDepartmentFromId(d.Id).Result,
                });
            }

            procedure.makandraTasks = Procedure.createTasks(process.Tasks.ToList(), procedure, taskContainer, taskStateContainer, taskRoles, duedates, procedures, roleContainer, userContainer, taskTemplateDepartmentContainer, taskTemplateContractTypeContainer).Result;
           
           return procedure;
        }

        /// <summary>
        /// starts a Process by creating a Procedure
        /// </summary>
        /// <param name="process">The Process the procedure is created from</param>
        /// <param name="Deadline">The Deadline of the Procedure</param>
        /// <param name="ReferencePerson">The Person that is the target of the Procedure</param>
        /// <param name="ResponsiblePerson">The Person responsible for the Procedure</param>
        /// <param name="name">The name of the Procedure</param>
        /// <param name="EstablishingContractType">The ContractType of the Procedure</param>
        /// <param name="TargetDepartments">The Departments the ReferencePerson should be part of</param>
        /// <param name="taskContainer">The Container for managing the database for the tasks to be created</param>
        /// <param name="taskStateContainer">The Container for managing the database for the states of the  tasks to be created</param>
        /// <param name="taskRoleContainer"></param>
        /// <param name="duedates">The Container for managing the database for the dates for the tasks</param>
        /// <param name="procedures">The Container for managing the database for procedures</param>
        /// <param name="roleContainer">The Container for managing the database with roles</param>
        /// <param name="userContainer">The Container for managing the database with users</param>
        /// <param name="taskTemplateDepartmentContainer">The Container for managing the tasktemplates Department Many to many relation</param>
        /// <param name="taskTemplateContractTypeContainer">The Container for managing the tasktemplates ContractTypes Many to many relation</param>
        /// <returns>a Procedure</returns>
        /// <author>Florian Fendt</author>


        public async Task<Procedure> startProcess(MakandraProcess process, User ReferencePerson, User ResponsiblePerson, DateTime Deadline, string name,  ContractType EstablishingContractType, List<Department> TargetDepartments, MakandraTaskContainer taskContainer, MakandraTaskStateContainer taskStateContainer,
        MakandraTaskRoleContainer taskRoleContainer, DuedateContainer duedates, ProcedureContainer procedures, RoleContainer roleContainer, UserContainer userContainer, TaskTemplateContainer taskTemplateContainer, TaskTemplateDepartmentContainer taskTemplateDepartmentContainer, TaskTemplateContractTypeContainer taskTemplateContractTypeContainer, ProcedureDepartmentContainer procedureDepartmentContainer, DepartmentContainer departmentContainer)
        {
            foreach(Department d in TargetDepartment) Console.WriteLine(d.Name);

            var db = new MakandraContext();
            Procedure newProcedure =  Procedure.CreateProcedure(process, Deadline, ReferencePerson, ResponsiblePerson, name, EstablishingContractType, TargetDepartments, taskContainer, taskStateContainer, taskRoleContainer, duedates, procedures, roleContainer, userContainer, taskTemplateDepartmentContainer, taskTemplateContractTypeContainer, procedureDepartmentContainer, departmentContainer).Result;
            return newProcedure;
        }

        /// <summary>
        /// Creates tasks from a list of tasktemplates
        /// </summary>
        /// <param name="Templates">List of TaskTemplates that shall be converted into tasks</param>
        /// <param name="procedure">The procedutre the tasks shall be part of</param>
        /// <param name="taskContainer">The Container for managing the database for tasks</param>
        /// <param name="states">The Container for managing the database for states</param>
        /// <param name="taskRoleContainer">The Container for managing the database for tasks and the many to many to roles</param>
        /// <param name="duedateContainer">The Container for managing the database for duedates</param>
        /// <param name="procedureContainer">The Container for managing the database for procedures</param>
        /// <param name="roleContainer">The Container for managing the database for roles</param>
        /// <param name="userContainer">The Container for managing the database for users</param>
        /// <param name="templateDepartmentContainer">The Container for managing the database for tasktemplates</param>
        /// <param name="templateContractTypeContainer">The Container for managing the database for the many to many between tasktemplates and contracttypes</param>
        /// <returns>List of tasks</returns>
        /// <author>Florian Fendt</author>

        public static async Task<List<MakandraTask>> createTasks(
            List<TaskTemplate> Templates,
            Procedure procedure,
            MakandraTaskContainer taskContainer,
            MakandraTaskStateContainer states,
            MakandraTaskRoleContainer taskRoleContainer,
            DuedateContainer duedateContainer,
            ProcedureContainer procedureContainer,
            RoleContainer roleContainer,
            UserContainer userContainer,
            TaskTemplateDepartmentContainer templateDepartmentContainer,
            TaskTemplateContractTypeContainer templateContractTypeContainer)
        {
            List<MakandraTask> makandraTasks = new List<MakandraTask>();
            foreach (TaskTemplate element in Templates)
            {
                bool dpcorrect = false;
                bool ctcorrect = true;
                var templatedepartments = await templateDepartmentContainer.GetTaskTemplateDepartmentFromTaskTemplate(element);
                var templatecontracttypes = await templateContractTypeContainer.GetTaskTemplateContractTypeFromTaskTemplate(element);
                foreach(var dp in templatedepartments){
                    foreach (var tdp in procedure.TargetDepartment) {
                        if (dp.DepartmentID == tdp.Id) {
                            dpcorrect = true;
                            break;
                        }
                    }
                }
                var contracttypes = new List<int>();
                foreach(var ct in templatecontracttypes){
                    contracttypes.Add(ct.ContractTypeID);
                }
                if(!contracttypes.Contains(procedure.EstablishingContractTypeId)){
                    ctcorrect = false;
                }
                if(dpcorrect && ctcorrect && element.Archived is false){
                    var task = new MakandraTask();
                    await task.InitializeTask(procedure, element, taskContainer, states, taskRoleContainer, duedateContainer, procedureContainer, roleContainer, userContainer);
                    makandraTasks.Add(task);
                }
            }
            return makandraTasks;
        }
        /// <summary>
        /// Checks if Procedure to be added is allowed to be added
        /// </summary>
        /// <param name="contractTypesContainer">for ckecking if ContractType exists</param>
        /// <param name="makandraProcessContainer">for ckecking if Process exists</param>
        /// <param name="userContainer">for ckecking if Users exist</param>
        /// <returns>int that indicates the outcome of the valid check</returns>
        /// <author>Florian Fendt</author>
        public int IsValid(ContractTypesContainer contractTypesContainer, MakandraProcessContainer makandraProcessContainer, UserContainer userContainer){
            if (this.name is null) return 1;
            if (contractTypesContainer.GetContractTypeFromID(EstablishingContractTypeId).Result is null) return 2;
            if (makandraProcessContainer.GetProcessFromId(basedProcessId).Result is null) return 3;
            if (userContainer.GetUserFromId(ResponsiblePersonId).Result is null) return 4;
            if (userContainer.GetUserFromId(ReferencePersonId).Result is null) return 5;
            return 0;
        }
    }
}