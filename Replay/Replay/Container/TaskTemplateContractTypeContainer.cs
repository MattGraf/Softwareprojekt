using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Replay.Models;
using Replay.Models.MTM;

using System.Text.Json;

namespace Replay.Container
{
    /// <summary>
    /// Management of the many-to-many-table between <see cref="TaskTemplate"/> and <see cref="ContractType"/>
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateContractTypeContainer
    {
        private MakandraContext MakandraContext;

        /// <summary>
        /// Creates new Container
        /// </summary>
        /// <param name="MakandraContext">Context which is responsible for the database</param>
        /// <author>Matthias Grafberger</author>
        public TaskTemplateContractTypeContainer(MakandraContext makandraContext) {
            this.MakandraContext = makandraContext;
        }

        /// <summary>
        /// Adds a connection between a <see cref="TaskTemplate"/> and a <see cref="ContractType"/> in the database
        /// </summary>
        /// <param name="taskTemplateContractType">Contains the connection to be added</param>
        /// <author>Matthias Grafberger</author>
        public async void AddTaskTemplateContractType(TaskTemplateContractType taskTemplateContractType) {
            var taskTemplateContractTypeWhenExists = await MakandraContext.TaskTemplateContractTypes
                .FirstOrDefaultAsync<TaskTemplateContractType>(s => s.TaskTemplateID == taskTemplateContractType.TaskTemplateID && s.ContractTypeID == taskTemplateContractType.ContractTypeID);
            
            if (taskTemplateContractTypeWhenExists is not null) return;

            MakandraContext.TaskTemplateContractTypes.Add(taskTemplateContractType);
            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a connection between a <see cref="TaskTemplate"/> and a <see cref="ContractType"/> in the database
        /// </summary>
        /// <param name="taskTemplateContractType">Contains the connection to be deleted</param>
        /// <author>Matthias Grafberger</author>
        public async void DeleteTaskTemplateContractType(TaskTemplateContractType taskTemplateContractType) {
            var taskTemplateContractTypeToDelete = await MakandraContext.TaskTemplateContractTypes

                .FirstOrDefaultAsync<TaskTemplateContractType>(s => s.Equals(taskTemplateContractType));

            if (taskTemplateContractTypeToDelete is null) return;
            
            MakandraContext.Remove(taskTemplateContractType);

            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes all old connection between a <see cref="TaskTemplate"/> and a <see cref="ContractType"/> in the database
        /// </summary>
        /// <param name="taskTemplate"><see cref="TaskTemplate"/> of which the connections are deleted</param>
        /// <author>Matthias Grafberger</author>
        public void DeleteTaskTemplateContractTypeWithTaskTemplate(TaskTemplate taskTemplate) {
            List<TaskTemplateContractType> taskTemplateContractTypesToDelete = MakandraContext.TaskTemplateContractTypes
                .Where(s => s.TaskTemplateID == taskTemplate.ID)
                .ToList();

            if (taskTemplateContractTypesToDelete is null) return;

            foreach (TaskTemplateContractType taskTemplateContractType in taskTemplateContractTypesToDelete) {
                MakandraContext.Remove(taskTemplateContractType);
            }

            MakandraContext.SaveChanges();
        }

        /// <summary>
        /// Returns all connections between a specific <see cref="TaskTemplate"/> and possible <see cref="ContractType"/>s
        /// </summary>
        /// <param name="taskTemplate">TaskTemplate from which the connections are needed</param>
        /// <returns>All connections between a specific TaskTemplate and possible ContractTypes</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<List<TaskTemplateContractType>> GetTaskTemplateContractTypeFromTaskTemplate(TaskTemplate taskTemplate) {
            List<TaskTemplateContractType> TaskTemplateContractTypes = await MakandraContext.TaskTemplateContractTypes
                .Where(s => s.TaskTemplateID == taskTemplate.ID)
                .OrderBy(s => s.ContractTypeID)
                .ToListAsync();
            
            return TaskTemplateContractTypes;
        }

        /// <summary>
        /// Imports a Json-string with <see cref="TaskTemplateContractType"/>s in the database
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="contractTypesContainer">Container for the connection to the database</param>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(TaskTemplateContainer taskTemplateContainer, ContractTypesContainer contractTypesContainer, string jsonFile) {

            if (jsonFile is null) return;

                
            List<TaskTemplateContractType> taskTemplateContractTypes = new List<TaskTemplateContractType>();

            try {
                taskTemplateContractTypes = JsonSerializer.Deserialize<List<TaskTemplateContractType>>(jsonFile);
            } catch (InvalidOperationException e) {
                return;
            }

            taskTemplateContractTypes.ForEach(e => {

                int h = e.IsValid(taskTemplateContainer, contractTypesContainer);
                if (h == 0) {
                    AddTaskTemplateContractType(e);
                } else {
                    Console.WriteLine(h);
                    Console.WriteLine("TaskTemplateContractType couldn't added to database, because of a invalid state");
                }
            });
        }
    }
}