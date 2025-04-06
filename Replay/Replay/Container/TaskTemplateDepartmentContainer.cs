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
    public class TaskTemplateDepartmentContainer
    {
        private MakandraContext MakandraContext;

        /// <summary>
        /// Creates new Container
        /// </summary>
        /// <param name="makandraContext">Context which is responsible for the database</param>
        /// <author>Matthias Grafberger</author>
        public TaskTemplateDepartmentContainer(MakandraContext makandraContext) {
            this.MakandraContext = makandraContext;
        }

        /// <summary>
        /// Adds a connection between a <see cref="Models.TaskTemplate"/> and a <see cref="Models.Department"/> in the database
        /// </summary>
        /// <param name="taskTemplateDepartment">Contains the connection to be added</param>
        /// <author>Matthias Grafberger</author>
        public async void AddTaskTemplateDepartment(TaskTemplateDepartment taskTemplateDepartment) {
            var taskTemplateDepartmentTypeWhenExists = await MakandraContext.TaskTemplateDepartments
                .FirstOrDefaultAsync<TaskTemplateDepartment>(s => s.Equals(taskTemplateDepartment));
            
            if (taskTemplateDepartmentTypeWhenExists is not null) return;

            MakandraContext.Add(taskTemplateDepartment);
            await MakandraContext.SaveChangesAsync();
        }

         /// <summary>
        /// Deletes a connection between a <see cref="Models.TaskTemplate"/> and a <see cref="Models.Department"/> in the database
        /// </summary>
        /// <param name="taskTemplateDepartment">Contains the connection to be deleted</param>
        /// <author>Matthias Grafberger</author>
        public async void DeleteTaskTemplateDepartment(TaskTemplateDepartment taskTemplateDepartment) {
            var taskTemplateDepartmentToDelete = await MakandraContext.TaskTemplateDepartments

                .FirstOrDefaultAsync<TaskTemplateDepartment>(s => s.Equals(taskTemplateDepartment));

            if (taskTemplateDepartmentToDelete is null) return;
            
            MakandraContext.Remove(taskTemplateDepartment);

            await MakandraContext.SaveChangesAsync();
        }

        /// <summary>
        /// Returns all connections between a specific <see cref="TaskTemplate"/> and possible <see cref="Department"/>s
        /// </summary>
        /// <param name="taskTemplate">TaskTemplate from which the connections are needed</param>
        /// <returns>All connections between a specific TaskTemplate and possible ContractTypes</returns>
        /// <author>Matthias Grafberger</author>
        public async Task<List<TaskTemplateDepartment>> GetTaskTemplateDepartmentFromTaskTemplate(TaskTemplate taskTemplate) {
            List<TaskTemplateDepartment> taskTemplateDepartments = await MakandraContext.TaskTemplateDepartments
                .Where(s => s.TaskTemplateID == taskTemplate.ID)
                .OrderBy(s => s.DepartmentID)
                .ToListAsync();
            
            return taskTemplateDepartments;
        }

        /// <summary>
        /// Deletes all old connection between a <see cref="TaskTemplate"/> and a <see cref="Department"/> in the database
        /// </summary>
        /// <param name="taskTemplate"><see cref="TaskTemplate"/> of which the connections are deleted</param>
        /// <author>Matthias Grafberger</author>
        public void DeleteTaskTemplateDepartmentWithTaskTemplate(TaskTemplate taskTemplate) {
            List<TaskTemplateDepartment> taskTemplateDepartmentsToDelete = MakandraContext.TaskTemplateDepartments
                .Where(s => s.TaskTemplateID == taskTemplate.ID)
                .ToList();

            if (taskTemplateDepartmentsToDelete is null) return;

            foreach (TaskTemplateDepartment taskTemplateDepartment in taskTemplateDepartmentsToDelete) {
                MakandraContext.Remove(taskTemplateDepartment);
            }

            MakandraContext.SaveChanges();
        }

        /// <summary>
        /// Imports a Json-string with <see cref="TaskTemplateDepartment"/>s in the database
        /// </summary>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="departmentContainer">Container for the connection to the database</param>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(TaskTemplateContainer taskTemplateContainer, DepartmentContainer departmentContainer, string jsonFile) {

            if (jsonFile is null) return;

                
            List<TaskTemplateDepartment> taskTemplateDepartments = new List<TaskTemplateDepartment>();

            try {
                taskTemplateDepartments = JsonSerializer.Deserialize<List<TaskTemplateDepartment>>(jsonFile);
            } catch (InvalidOperationException e) {
                return;
            }

            taskTemplateDepartments.ForEach(e => {

                int h = e.IsValid(taskTemplateContainer, departmentContainer);
                if (h == 0) {
                    AddTaskTemplateDepartment(e);
                } else {
                    Console.WriteLine(h);
                    Console.WriteLine("TaskTemplateDepartment couldn't added to database, because of a invalid state");
                }
            });
        }

    }
}