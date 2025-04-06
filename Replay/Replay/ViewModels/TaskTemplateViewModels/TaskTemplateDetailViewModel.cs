using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models;
using Replay.Models.MTM;

namespace Replay.ViewModels.TaskTemplateViewModels
{
    /// <summary>
    /// ViewModel for the detail-view of <see cref="TaskTemplate"/>
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateDetailViewModel
    {
        public string Name {set; get;}
        public string Instruction {get; set;}
        public List<string> ContractTypes {get; set;} = new List<string>();
        public List<string> Departments {get; set;} = new List<string>();
        public string DuedateName {get; set;}
        public bool Archived {get; set;}
        public string DefaultResponsible {get; set;}
        public int ReturnPage {get; set;}

        public string Process {get; set;}

        public int ProcessId {get; set;}

        /// <summary>
        /// Initialize all needed information for the DetailView
        /// </summary>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <param name="taskTemplate"><see cref="TaskTemplate"/> of which the view is wanted</param>
        /// <param name="returnPage">Page from which this view is wanted</param>
        /// <param name="processId">Id of the <see cref="MakandraProcess"/> to go simple to the detail view from which this view is wanted</param>
        /// <author>Matthias Grafberger</author>
        public TaskTemplateDetailViewModel(MakandraProcessContainer makandraProcessContainer, TaskTemplate taskTemplate, int returnPage, int processId) {
            MakandraProcess makandraProcess = makandraProcessContainer.GetProcessFromId(taskTemplate.MakandraProcessId).Result;

            this.Name = taskTemplate.Name;

            this.Instruction = taskTemplate.Instruction;
            
            foreach (TaskTemplateContractType taskTemplateContractType in taskTemplate.TaskTemplateContractTypes) {
                ContractTypes.Add(taskTemplateContractType.ContractType.Name);
            }

            foreach (TaskTemplateDepartment taskTemplateDepartment in taskTemplate.TaskTemplateDepartments) {
                Departments.Add(taskTemplateDepartment.Department.Name);
            }

            this.DuedateName = taskTemplate.Duedate.Name;

            this.DefaultResponsible = taskTemplate.DefaultResponsible;

            this.Archived = taskTemplate.Archived;
        
            this.ReturnPage = returnPage;

            this.Process = makandraProcess.Name;

            this.ProcessId = processId;

        }
    }
}