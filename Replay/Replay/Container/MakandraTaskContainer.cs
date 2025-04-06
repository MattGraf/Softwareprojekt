using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Replay.Models
{
    /// <summary>
    /// Collection of methods for database operations concering
    /// <see cref="MakandraTask"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskContainer
    {
        private readonly MakandraContext _db;

        public MakandraTaskContainer(MakandraContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Adds a task to the database with the ID as the primary key.
        /// If a task with the same Id exists already in the database, the method returns immediatley without any action perfmored.
        /// </summary>
        /// <param name="task">The task of type <see cref="MakandraTask"/> that is supposed to be saved in the database</param> 
        /// <author>Thomas Dworschak</author>
        public async Task AddMakandraTask(MakandraTask task)
        {
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Updates an existing task with identical Id in the database.
        /// If a task can't be found in the database, the method returns immediately without any action performed.
        /// If a task is in the database, all attributes are updated including possible changes.
        /// </summary>
        /// <param name="task">The task of type <see cref="MakandraTask"/> that is supposed to be updated in the database</param>
        /// <author>Thomas Dworschak</author>
        public async Task UpdateMakandraTask(MakandraTask task)
        {
            var makandraTaskToUpdate = await _db.Tasks
                .Include(r => r.EditAccess)
                .FirstOrDefaultAsync<MakandraTask>(t => t.Id == task.Id);

            if (makandraTaskToUpdate == null)
            {
                await AddMakandraTask(task);
            }
            else
            {
                _db.Entry(makandraTaskToUpdate).CurrentValues.SetValues(task);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an existing task from the database.
        /// If the task can't be found in the database, the method returns immediately without any action performed.
        /// </summary>
        /// <param name="task">The task of type <see cref="MakandraTask"/> that is supposed to be deleted from the database</param>
        /// <author>Thomas Dworschak</author>
        public async Task DeleteMakandraTask(MakandraTask task)
        {

            var makandraTaskToDelete = await _db.Tasks
                .Where(t => t.Id == task.Id)
                .FirstOrDefaultAsync<MakandraTask>();

            if (makandraTaskToDelete == null)
            {
                return;
            }

            _db.Remove(makandraTaskToDelete);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Returnes a task identified by Id from the database.
        /// </summary>
        /// <param name="id">Integer that should match an Id entry for <see cref="MakandraTask"/> in the database.</param>
        /// <returns>
        /// <c>MakandraTask</c> if the task was identified and found in the database.
        /// <c>null</c> if the task could not be identified and found in the database.
        /// </returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <author>Thomas Dworschak</author>
        public async Task<MakandraTask> GetMakandraTaskFromId(int id)
        {
            var task = await _db.Tasks
                .Where(t => t.Id == id)
                .Include(r => r.EditAccess)
                .ThenInclude(rt => rt.Role)
                .FirstOrDefaultAsync<MakandraTask>();

            if (task is null)
            {
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }

            return task;
        }

        /// <summary>
        /// Retrieves a list of <see cref="MakandraTask"/> tasks
        /// that are associated with an <see cref="User"/> assignee
        /// by checking the assignee id and the assignee id attribute
        /// of the task
        /// </summary>
        /// <param name="userId">Id of the <see cref="User"/> to find tasks associated with this user as assignee</param>
        /// <returns>List of tasks</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTask>> GetMakandraTasksForUser(int userId)
        {
            var tasks = await _db.Tasks
                .Include(t => t.State)
                .Where(t => t.AssigneeId == userId && t.Archived == false)
                .ToListAsync();
            return tasks;
        }

        /// <summary>
        /// Retrieves a list of archived <see cref="MakandraTask"/> tasks
        /// that are associated with an <see cref="User"/> assignee
        /// by checking the assignee id and the assignee id attribute
        /// of the task
        /// </summary>
        /// <param name="userId">Id of the <see cref="User"/> to find tasks associated with this user as assignee</param>
        /// <returns>List of archived tasks</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTask>> GetArchivedMakandraTasksForUser(int userId)
        {
            var tasks = await _db.Tasks
                .Include(t => t.State)
                .Where(t => t.AssigneeId == userId && t.Archived == true)
                .ToListAsync();
            return tasks;
        }

        /// <summary>
        /// Retrieves a list of <see cref="MakandraTask"/> tasks from the database according to the following association:
        /// A list of <see cref="Procedure"/> ids is given, where a <see cref="User"/> is associated as responsible
        /// for each of these procedures.
        /// Each task from the database is added, where the attribute <c>ProcedureId</c> has a match in the
        /// input list of Ids.
        /// </summary>
        /// <param name="responsibleProcedures">Ids of <see cref="Procedure"/> procedures a <see cref="User"/> is responsible for</param>
        /// <returns>List of tasks</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTask>> GetMakandraTasksForResponsiblePerson(List<int> responsibleProcedures)
        {
            var tasks = await _db.Tasks
                .Include(t => t.State)
                .Where(t => responsibleProcedures.Contains(t.ProcedureId) && t.Archived == false)
                .ToListAsync();

            return tasks;
        }

        /// <summary>
        /// Retrieves one or no <see cref="MakandraTask"/> from the database,
        /// that a <see cref="User"/> has acces to edit to.
        /// A user has access, if he is the assignee of a task or
        /// he is responsible for the <see cref="Procedure"/> the task is attached to.
        /// Additionally, a potential access is granted to a specific <see cref="Role"/>.
        /// Lastly: If the role ids contain the id <c>1</c> for <c>Administrator</c>,
        /// access is granted anyways.
        /// </summary>
        /// <param name="taskId"><see cref="MakandraTask"/> id of the task that is supposed to be accessed</param>
        /// <param name="userId"><see cref="User"/> id of the user who wants to access the task</param>
        /// <param name="procedureIds">Set of <see cref="Procedure"/> ids for which the user is responsible</param>
        /// <param name="roleIds">Set of <see cref="Role"/> ids of the user that allow access to the task</param>
        /// <returns>Task for which the user as edit access</returns>
        public async Task<MakandraTask?> GetAccessibleTask(int taskId, int userId, List<int> procedureIds, List<int> roleIds)
        {
            return await _db.Tasks
                .Where(t => (t.Id == taskId && t.AssigneeId == userId && t.Archived == false)
                    || (t.Id == taskId && procedureIds.Contains(t.Id))
                    || (t.Id == taskId && (roleIds.Contains((int) t.ResponsibleRoleId)))
                    || (t.Id == taskId && roleIds.Contains(1)))
                .FirstOrDefaultAsync<MakandraTask>(); 
        }

        /// <summary>
        /// Retrieves a list of archived <see cref="MakandraTask"/> tasks from the database according to the following association:
        /// A list of <see cref="Procedure"/> ids is given, where a <see cref="User"/> is associated as responsible
        /// for each of these procedures.
        /// Each task from the database is added, where the attribute <c>ProcedureId</c> has a match in the
        /// input list of Ids.
        /// </summary>
        /// <param name="responsibleProcedures">Ids of <see cref="Procedure"/> procedures a <see cref="User"/> is responsible for</param>
        /// <returns>List of archived tasks</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTask>> GetArchivedMakandraTasksForResponsiblePerson(List<int> responsibleProcedures)
        {
            var tasks = await _db.Tasks
                .Include(t => t.State)
                .Where(t => responsibleProcedures.Contains(t.ProcedureId) && t.Archived == true)
                .ToListAsync();

            return tasks;
        }

        /// <summary>
        /// Retrieves a list of <see cref="MakandraTask"/> tasks from the database where the
        /// attribute ResponsibleRoleId matches the input <see cref="Role"/> Id,
        /// but where the task has no <see cref="User"/> already assigned
        /// </summary>
        /// <param name="role">Id of the <see cref="Role"/> whose tasks are supposed to be retrieved</param>
        /// <returns>List of tasks</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTask>> GetMakandraTasksForRole(int role)
        {
            var tasks = await _db.Tasks
                .Include(t => t.State)
                .Where(t => t.ResponsibleRoleId == role && t.Archived == false)
                .ToListAsync();
            return tasks;
        }

        /// <summary>
        /// Retrieves a list of archived <see cref="MakandraTask"/> tasks from the database where the
        /// attribute ResponsibleRoleId matches the input <see cref="Role"/> Id,
        /// but where the task never had an <see cref="User"/> assigned
        /// </summary>
        /// <param name="role">Id of the <see cref="Role"/> whose archived tasks are supposed to be retrieved</param>
        /// <returns>List of archived tasks</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTask>> GetArchivedMakandraTasksForRole(int role)
        {
            var tasks = await _db.Tasks
                .Include(t => t.State)
                .Where(t => t.ResponsibleRoleId == role && t.Archived == true)
                .ToListAsync();
            return tasks;
        }

        /// <summary>
        /// Retrieves all <see cref="MakandraTask"/> tasks that are currently in the database
        /// </summary>
        /// <returns>List of all tasks in the database</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTask>> GetMakandraTasks()
        {
            List<MakandraTask> tasks = await _db.Tasks
                .Where(t => t.Archived == false)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tasks;
        }

        /// <summary>
        /// Retrieves all <see cref="MakandraTask"/> tasks what are currentliy in the database
        /// and have the <c>Archived</c> attribute set to true
        /// </summary>
        /// <returns>List of all archived tasks from the database</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTask>> GetArchivedMakandraTasks()
        {
            List<MakandraTask> tasks = await _db.Tasks
                .Where(t => t.Archived == true)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return tasks;
        }
        /// <summary>
        /// Gets the Tasks that are part of the provided procedure
        /// </summary>
        /// <param name="id"> the Id of the procedure</param>
        /// <returns>List of Tasks</returns>
        /// <author>Florian Fendt</author>
        public async Task<List<MakandraTask>> GetMakandraTasksFromProcedureId(int id)
        {
            List<MakandraTask> tasks = await _db.Tasks
                .Where(x => x.ProcedureId == id)
                .ToListAsync();

            return tasks;
        }

        /// <summary>
        /// Imports <see cref="MakandraTask"/> as initial seeds for the databse. See
        /// <see cref="SeedData"/>-folder for specific data details.
        /// The method reads an input file path, checks wheather any data is alredy
        /// present and if not, seeds the data of the file the input string is
        /// pointing towards.
        /// It goes through all instances of the category <c>task</c> in the seeding
        /// JSON, then retrieves all attributes from the corresponding fields and
        /// creates a new <see cref="MakandraTask"/> filled with those attributes.
        /// </summary>
        /// <param name="jsonFilePath">Path to the JSON file containing the seed data</param>
        /// <author>Thomas Dworschak</author>
        public async Task ImportMakandraTasks(string jsonFilePath)
        {
            var jsonData = await File.ReadAllTextAsync(jsonFilePath);
            var jsonDocument = JsonDocument.Parse(jsonData);

            var root = jsonDocument.RootElement;

            var onboardingProcesses = root.GetProperty("onboarding_processes");

            foreach (var process in onboardingProcesses.EnumerateArray())
            {
                var tasks = process.GetProperty("tasks");

                foreach (var row in tasks.EnumerateArray())
                {
                    var task = new MakandraTask
                    {
                        Id = row.GetProperty("id").GetInt32(),
                        Name = row.GetProperty("name").GetString(),
                        Instruction = row.TryGetProperty("instruction", out JsonElement instructionElement) && instructionElement.ValueKind != JsonValueKind.Null ? instructionElement.GetString() : null,
                        ResponsibleRoleId = row.TryGetProperty("responsible_role_id", out JsonElement responsibleRoleIdElement) && responsibleRoleIdElement.ValueKind != JsonValueKind.Null ? (int?)responsibleRoleIdElement.GetInt32() : null,
                        TargetDate = row.GetProperty("target_date").GetDateTime(),
                        Notes = row.TryGetProperty("notes", out JsonElement notesElement) && notesElement.ValueKind != JsonValueKind.Null ? notesElement.GetString() : null,
                        StateId = row.GetProperty("state_id").GetInt32(),
                        ProcedureId = row.GetProperty("procedure_id").GetInt32(),
                        AssigneeId = row.TryGetProperty("assignee_id", out JsonElement assigneeIdElement) && assigneeIdElement.ValueKind != JsonValueKind.Null ? (int?)assigneeIdElement.GetInt32() : null,
                        Archived = row.GetProperty("archived").GetBoolean()
                    };

                    await AddMakandraTask(task);
                }
            }
        }


    }
}
