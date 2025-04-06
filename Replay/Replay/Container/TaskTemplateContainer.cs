using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Replay.Data;
using Replay.Models;
using Replay.Models.MTM;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Replay.Container
{
    /// <summary>
    /// Managing the connection to the database of the <see cref="TaskTemplate"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateContainer
    {
        
        private MakandraContext MakandraContext;

        /// <summary>
        /// Create new Container
        /// </summary>
        /// <param name="makandraContext">Context which is responsible for the database</param>
        /// <author>Matthias Grafberger</author>
        public TaskTemplateContainer(MakandraContext makandraContext) {
            this.MakandraContext = makandraContext;
        }
        

        /// <summary>
        /// Add a TaskTemplate in the database
        /// </summary>
        /// <param name="taskTemplate">TaskTemplate to be added</param>
        /// <author>Matthias Grafberger</author>
        public int AddTaskTemplate(TaskTemplate taskTemplate) {
            MakandraContext.TaskTemplates.Add(taskTemplate);
            MakandraContext.SaveChanges();

            return taskTemplate.ID;
        }

        /// <summary>
        /// Returns the saved TaskTemplates
        /// </summary>
        /// <returns>Saved TaskTemplates</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<List<TaskTemplate>> GetAllTaskTemplates() {
            List<TaskTemplate> TaskTemplates = await MakandraContext.TaskTemplates

                .OrderBy(s => s.ID)
                .ToListAsync();
            return TaskTemplates;
        }

        /// <summary>
        /// Update a specific TaskTemplates in the database
        /// </summary>
        /// <param name="taskTemplate">TaskTemplate to be changed</param>
        /// <author>Matthias Grafberger</author>
        public async Task<TaskTemplate> UpdateTaskTemplate(TaskTemplate taskTemplate) {
            var taskTemplateToUpdate = await MakandraContext.TaskTemplates

                .FirstOrDefaultAsync<TaskTemplate>(s => s.ID == taskTemplate.ID);


            if (taskTemplateToUpdate is null) AddTaskTemplate(taskTemplate);
            
            taskTemplateToUpdate.Name = taskTemplate.Name;
            taskTemplateToUpdate.Instruction = taskTemplate.Instruction;
            taskTemplateToUpdate.DuedateID = taskTemplate.DuedateID;
            taskTemplateToUpdate.Duedate = taskTemplate.Duedate;
            taskTemplateToUpdate.DefaultResponsible = taskTemplate.DefaultResponsible;
            taskTemplateToUpdate.Archived = taskTemplate.Archived;
            taskTemplateToUpdate.MakandraProcessId = taskTemplate.MakandraProcessId;

            await MakandraContext.SaveChangesAsync();
            
            return taskTemplateToUpdate;
        }

        /// <summary>
        /// Get a specific TaskTemplate with a specific ID and with its references
        /// </summary>
        /// <param name="ID">ID of the wished TaskTemplate</param>
        /// <returns>TaskTemplate with wanted ID</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<TaskTemplate> GetTaskTemplateFromId(int ID) {

            var TaskTemplate = await MakandraContext.TaskTemplates
                .Where(s => s.ID == ID)
                .FirstOrDefaultAsync<TaskTemplate>();

            if (TaskTemplate is null) return null;

            DuedateContainer duedateContainer = new DuedateContainer(MakandraContext);
            TaskTemplateContractTypeContainer taskTemplateContractTypeContainer = new TaskTemplateContractTypeContainer(MakandraContext);
            ContractTypesContainer contractTypesContainer = new ContractTypesContainer(MakandraContext);

            TaskTemplateDepartmentContainer taskTemplateDepartmentContainer = new TaskTemplateDepartmentContainer(MakandraContext);
            DepartmentContainer departmentContainer = new DepartmentContainer(MakandraContext);

            TaskTemplate.Duedate = duedateContainer.GetDuedateFromId(TaskTemplate.DuedateID).Result;
            TaskTemplate.TaskTemplateContractTypes = taskTemplateContractTypeContainer.GetTaskTemplateContractTypeFromTaskTemplate(TaskTemplate).Result;
            TaskTemplate.TaskTemplateDepartments = taskTemplateDepartmentContainer.GetTaskTemplateDepartmentFromTaskTemplate(TaskTemplate).Result;

            foreach(TaskTemplateContractType taskTemplateContractType in TaskTemplate.TaskTemplateContractTypes) {
                taskTemplateContractType.ContractType = contractTypesContainer.GetContractTypeFromID(taskTemplateContractType.ContractTypeID).Result;
            }

            foreach(TaskTemplateDepartment taskTemplateDepartment in TaskTemplate.TaskTemplateDepartments) {
                taskTemplateDepartment.Department = departmentContainer.GetDepartmentFromId(taskTemplateDepartment.DepartmentID).Result;
            }

            return TaskTemplate;
        }

        /// <summary>
        /// Returns all <see cref="TaskTemplate"/>s which aren't archived
        /// </summary>
        /// <returns>List of all <see cref="TaskTemplate"/>s which aren't archived</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<List<TaskTemplate>> GetTaskTemplatesNotArchived() {
            List<TaskTemplate> TaskTemplates = await MakandraContext.TaskTemplates
                .Where(s => !s.Archived)
                .OrderBy(s => s.ID)
                .ToListAsync();
            return TaskTemplates;
        }

        /// <summary>
        /// Returns all <see cref="TaskTemplate"/>s which aren't archived and are from a specific <see cref="MakandraProcess"/>
        /// </summary>
        /// <param name="processId">Id of the <see cref="MakandraProcess"/></param>
        /// <returns>All <see cref="TaskTemplate"/>s which aren't archived and are from a specific <see cref="MakandraProcess"/></returns>
        /// <author>Matthias Grafberger</author>
         public async Task<List<TaskTemplate>> GetTaskTemplatesNotArchivedWithProcessId(int processId) {
            List<TaskTemplate> TaskTemplates = await MakandraContext.TaskTemplates
                .Where(s => !s.Archived)
                .Where(s => s.MakandraProcessId == processId)
                .OrderBy(s => s.ID)
                .ToListAsync();
            return TaskTemplates;
         }

        /// <summary>
        /// Returns all <see cref="TaskTemplate"/>s which are archived
        /// </summary>
        /// <returns>List of all <see cref="TaskTemplate"/>s which are archived</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<List<TaskTemplate>> GetTaskTemplatesArchived() {
            List<TaskTemplate> TaskTemplates = await MakandraContext.TaskTemplates
                .Where(s => s.Archived)
                .OrderBy(s => s.ID)
                .ToListAsync();
            return TaskTemplates;
        }

        /// <summary>
        /// Returns all <see cref="TaskTemplate"/>s which are archived and from a specific <see cref="MakandraProcess"/>
        /// </summary>
        /// <param name="processId">Id of the <see cref="MakandraProcess"/></param>
        /// <returns>All <see cref="TaskTemplate"/>s which are archived and from a specific <see cref="MakandraProcess"/></returns>
        /// <author>Matthias Grafberger</author>

        public async Task<List<TaskTemplate>> GetTaskTemplatesArchivedWithProcessId(int processId) {
            List<TaskTemplate> TaskTemplates = await MakandraContext.TaskTemplates
                .Where(s => s.Archived)
                .Where(s => s.MakandraProcessId == processId)
                .OrderBy(s => s.ID)
                .ToListAsync();
            return TaskTemplates;
         }

        /// <summary>
        /// Method to archive a <see cref="TaskTemplate"/>
        /// </summary>
        /// <param name="ID">ID of the <see cref="TaskTemplate"/> which is wanted to archive</param>
        /// <returns>If a TaskTemplate with the given ID is found</returns>
        /// <author>Matthias Grafberger</author>
        public  bool TaskTemplateArchive(int ID) {
            var TaskTemplate = MakandraContext.TaskTemplates
                .FirstOrDefault<TaskTemplate>(s => s.ID == ID);

            if (TaskTemplate is null) return false;

            TaskTemplate.Archived = true;

            MakandraContext.SaveChanges();

            return true;;
        }

        /// <summary>
        /// Method to restore a <see cref="TaskTemplate"/>
        /// </summary>
        /// <param name="ID">ID of the <see cref="TaskTemplate"/> which is wanted to restore</param>
        /// <returns>If a TaskTemplate with the given ID is found</returns>
        /// <author>Matthias Grafberger</author>
        public  bool TaskTemplateRestore(int ID) {
            var TaskTemplate = MakandraContext.TaskTemplates
                .FirstOrDefault<TaskTemplate>(s => s.ID == ID);

            if (TaskTemplate is null) return false;

            TaskTemplate.Archived = false;

            MakandraContext.SaveChanges();

            return true;;
        }

        /// <summary>
        /// Imports a Json-string with <see cref="TaskTemplate"/>s in the database
        /// </summary>
        /// <param name="duedateContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(DuedateContainer duedateContainer, MakandraProcessContainer makandraProcessContainer, string jsonFile) {

            if (jsonFile is null) return;

                
            List<TaskTemplate> taskTemplates = new List<TaskTemplate>();

            try {
                taskTemplates = JsonSerializer.Deserialize<List<TaskTemplate>>(jsonFile);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            taskTemplates.ForEach(e => {
                int h = e.IsValid(duedateContainer, makandraProcessContainer);
                if (h == 0)
                {
                    AddTaskTemplate(e);
                } else {
                    switch(h)
                    {
                        case 1: Console.WriteLine("Name Error"); break;
                        case 2: Console.WriteLine("Duedate Error"); break;
                        case 3: Console.WriteLine("DefaultResponsible Error"); break;
                        case 4: Console.WriteLine("Process Error"); break;
                    }

                    Console.WriteLine("TaskTemplate couldn't added to database, because of a invalid state");
                }
            });
        }
    }
}
