using System.Diagnostics.Contracts;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.AspNetCore.Http;
using Replay.Authorization;
using Replay.Container.Account.MTM;
using Replay.Models;
using Replay.Models.Account;
using Replay.Models.Account.MTM;
using Replay.Models.MTM;
using Replay.ViewModels;
using Replay.ViewModels.Task;

namespace Replay.Controllers
{
    /// <summary>
    /// Container to navigate between different Views of <see cref="MakandraTask"/>
    /// Provides the following views: <see cref="Index"/>, <see cref="Archive"/>,
    /// <see cref="AllTasks"/>, <see cref="AllArchivedTasks"/>, <see cref="Detail"/>,
    /// <see cref="Edit"/>.
    /// Additionally, methods are provided to reset the filter in the overview views:
    /// <see cref="ResetFilterIndex"/>, <see cref="ResetFilterArchive"/>,
    /// <see cref="ResetFilterAllTasks"/>, <see cref="ResetFilterAllArchivedTasks"/>.
    /// Lastly, a method <see cref="IndexUpdateTaskStatus"/> is provided to
    /// make a short cut edit of the <see cref="MakandraTaskStatus"/> of a Task
    /// possible
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskController : Controller
    {
        private MakandraTaskContainer _taskContainer;
        private readonly List<MakandraTask> Tasks;
        private readonly UserContainer _userContainer;
        private readonly UserRolesContainer _userRolesContainer;
        private readonly RoleContainer _roleContainer;
        private readonly ProcedureContainer _procedureContainer;
        private readonly MakandraTaskStateContainer _statesContainer;
        private readonly MakandraTaskRoleContainer _taskRoleContainer;
        public MakandraTaskController(MakandraTaskContainer taskContainer, UserContainer userContainer, UserRolesContainer userRolesContainer, RoleContainer roleContainer, ProcedureContainer procedureContainer, MakandraTaskStateContainer stateContainer, MakandraTaskRoleContainer taskRoleContainer)
        {
            _taskContainer = taskContainer;
            _userContainer = userContainer;
            _userRolesContainer = userRolesContainer;
            _roleContainer = roleContainer;
            _procedureContainer = procedureContainer;
            _statesContainer = stateContainer;
            _taskRoleContainer = taskRoleContainer;

            Tasks = _taskContainer.GetMakandraTasks().Result;
        }

        /// <summary>
        /// Based on the logged in <see cref="User"/> and his roles (<see cref="Role"/>), 
        /// this method creates a new <see cref="MakandraTaskUserViewModel"/> and fills
        /// its attributes.
        /// It fetches all <see cref="MakandraTask"/> tasks for this user, his roles,
        /// and the tasks of the <see cref="Procedure"/> he is responsible for.
        /// The method takes sorting parameters from the <see cref="Index.cshtml"/> view
        /// to sort these task lists by calling <see cref="SortTaskListByDate"/>
        /// and <see cref="FilterTaskListByProcedure"/>
        /// </summary>
        /// <param name="userSortOrder">The date sort pattern for the tasks directly assigned to the user. Can be <c>asc</c> or <c>desc</c></param>
        /// <param name="responsibleSortOrder">The date sort pattern for the tasks of a procedure the user is responsible for. Can be <c>asc</c> or <c>desc</c></param>
        /// <param name="roleSortOrder">The date sort pattern for the tasks assigned to one or more of the user's roles. Can be <c>asc</c> or <c>desc</c></param>
        /// <param name="procedureFilter">The name of the procedure that will be used to display only tasks attached to this procedure. This filter is set for a 30 minute session</param>
        /// <returns><see cref="MakandraTaskUserViewModel"/> Index view with tasks for user, roles, and responsible procedures</returns>
        /// <author>Thomas Dworschak</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Index(string userSortOrder, string responsibleSortOrder, string[] roleSortOrder, string procedureFilter)
        {
            if (string.IsNullOrEmpty(procedureFilter))
            {
                procedureFilter = HttpContext.Session.GetString("ProcedureFilter");
            }
            else
            {
                HttpContext.Session.SetString("ProcedureFilter", procedureFilter);
            }

            ViewData["UserDateSortParam"] = userSortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["ResponsibleDateSortParam"] = responsibleSortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["ProcedureFilter"] = procedureFilter;

            var loggedInUser = await _userContainer.GetLoggedInUser(HttpContext);

            if (loggedInUser is null)
            {
                return NotFound();
            }

            var loggedInRoles = (await _userRolesContainer.GetRolesFromUser(loggedInUser)).ToList();
            var responsibleProcedures = await _procedureContainer.getProceduresFromUser(loggedInUser);
            List<int> responsibleProcduresIds = responsibleProcedures.Select(p => p.Id).ToList();

            var tasksForUser = await _taskContainer.GetMakandraTasksForUser(loggedInUser.Id);
            var tasksForResponsibleUser = await _taskContainer.GetMakandraTasksForResponsiblePerson(responsibleProcduresIds);
            var tasksForResponsibleUserUnassigned = tasksForResponsibleUser.Where(t => t.AssigneeId != loggedInUser.Id).ToList();

            var indexViewModel = new MakandraTaskUserViewModel(new MakandraTaskDetailViewModel())
            {
                User = loggedInUser,
                Roles = loggedInRoles,
            };

            indexViewModel.Tasklist = await indexViewModel.FillTaskList(tasksForUser, _roleContainer, _userContainer, _procedureContainer, _statesContainer);
            indexViewModel.TasksForRoles = await indexViewModel.FillTasksForRoles(loggedInRoles, _taskContainer, _roleContainer, _userContainer, _procedureContainer, _statesContainer);
            indexViewModel.TasksForResponsible = await indexViewModel.FillTaskList(tasksForResponsibleUserUnassigned, _roleContainer, _userContainer, _procedureContainer, _statesContainer);

            if (!string.IsNullOrEmpty(procedureFilter))
            {
                indexViewModel.Tasklist = await indexViewModel.FilterTaskListByProcedure(indexViewModel.Tasklist, procedureFilter, _procedureContainer);
                indexViewModel.TasksForResponsible = await indexViewModel.FilterTaskListByProcedure(indexViewModel.TasksForResponsible, procedureFilter, _procedureContainer);

                for (int i = 0; i < indexViewModel.TasksForRoles.Count; i++)
                {
                    var roleTasks = indexViewModel.TasksForRoles[i];
                    var filteredTasks = await indexViewModel.FilterTaskListByProcedure(roleTasks, procedureFilter, _procedureContainer);
                    indexViewModel.TasksForRoles[i] = filteredTasks;
                }
            }

            indexViewModel.Tasklist = indexViewModel.SortTaskListByDate(indexViewModel.Tasklist, userSortOrder);
            indexViewModel.TasksForResponsible = indexViewModel.SortTaskListByDate(indexViewModel.TasksForResponsible, responsibleSortOrder);

            for (int i = 0; i < loggedInRoles.Count; i++)
            {
                string currentRoleSortOrder = roleSortOrder.Length > i ? roleSortOrder[i] : null;
                ViewData[$"Role{i}DateSortParam"] = currentRoleSortOrder == "date_asc" ? "date_desc" : "date_asc";

                indexViewModel.TasksForRoles[i] = indexViewModel.SortTaskListByDate(indexViewModel.TasksForRoles[i], currentRoleSortOrder);
            }

            return View(indexViewModel);
        }

        /// <summary>
        /// Changes the <see cref="MakandraTaskState"/> of tasks listed in the <see cref="Index.cshtml"/>
        /// and the <see cref="Archive.cshtml"/> based on their id to the state name handed
        /// to the method.
        /// Serves as a shortcut to change the state instead of navigating to the <see cref="Edit.cshtml"/> 
        /// of each task.
        /// </summary>
        /// <param name="id">Id of <see cref="MakandraTask"/> whose state should be changed</param>
        /// <param name="stateName">Name of the state that the current task state should be changed to</param>
        /// <returns>OkResult in case of success, NotFound in case of fail</returns>
        /// <author>Thomas Dworschak</author>
        [HttpPost]
        [PermissionChecker("")]
        public async Task<IActionResult> IndexUpdateTaskStatus(int id, string stateName)
        {
            MakandraTask task = await _taskContainer.GetMakandraTaskFromId(id);
            MakandraTaskState state = await _statesContainer.GetMakandraTaskStateFromName(stateName);

            if (task == null || state == null)
            {
                return NotFound();
            }

            task.State = state;

            if (state.Name.Equals("In Bearbeitung") && task.Assignee is null)
            {
                var loggedInUser = await _userContainer.GetLoggedInUser(HttpContext);
                task.AssigneeId = loggedInUser.Id;
                task.Assignee = loggedInUser;
            }

            if (state.Name.Equals("Erledigt"))
            {
                task.Archived = true;
            }
            await _taskContainer.UpdateMakandraTask(task);

            return Ok();
        }

        /// <summary>
        /// Resets the <c>procedureFilter</c> of the <see cref="Index"/> or <see cref="Archive"/> methods
        /// as they are set for a 30 minute session.
        /// </summary>
        /// <returns>Redirection to the index view</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<IActionResult> ResetFilterIndex()
        {
            HttpContext.Session.Remove("ProcedureFilter");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Resets the <c>procedureFilter</c> of the <see cref="AllTasks"/> method
        /// as they are set for a 30 minute session.
        /// </summary>
        /// <returns>Redirection to the all tasks view</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<IActionResult> ResetFilterAllTasks()
        {
            HttpContext.Session.Remove("ProcedureFilter");
            return RedirectToAction("AllTasks");
        }

        /// <summary>
        /// Resets the <c>procedureFilter</c> of the <see cref="AllArchivedTasks"/> method
        /// as they are set for a 30 minute session.
        /// </summary>
        /// <returns>Redirection to the archived tasks view</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<IActionResult> ResetFilterAllArchivedTasks()
        {
            HttpContext.Session.Remove("ProcedureFilter");
            return RedirectToAction("AllArchivedTasks");
        }

        /// <summary>
        /// Resets the <c>procedureFilter</c> of the <see cref="Index"/> or <see cref="Archive"/> methods
        /// as they are set for a 30 minute session.
        /// </summary>
        /// <returns>Redirection to the archive view</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<IActionResult> ResetFilterArchive()
        {
            HttpContext.Session.Remove("ProcedureFilter");
            return RedirectToAction("Archive");
        }


        /// <summary>
        /// Generates a list of all non-archived <see cref="MakandraTask"/> tasks
        /// currently in the database.
        /// Serves as an overview for supervisors.
        /// The list supports sorting by date by taking the sorting parameter
        /// from the <see cref="AllTasks.cshtml"/> by calling <see cref="SortTaskListByDate"/> and
        /// filtering by the name of a <see cref="Procedure"/>
        /// </summary>
        /// <param name="sortOrder">The date sort pattern for the tasks. Can be <c>asc</c> or <c>desc</c></param>
        /// <param name="procedureFilter">Name of the <see cref="Procedure"/> to be filtered for</param>
        /// <returns><see cref="MakandraTaskUserViewModel"/> allTasksViewModel with the tasklist filled with all active tasks</returns>
        /// <author>Thomas Dworschak</author>
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> AllTasks(string sortOrder, string procedureFilter)
        {
            if (string.IsNullOrEmpty(procedureFilter))
            {
                procedureFilter = HttpContext.Session.GetString("ProcedureFilter");
            }
            else
            {
                HttpContext.Session.SetString("ProcedureFilter", procedureFilter);
            }


            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["ProcedureFilter"] = procedureFilter;

            var loggedInUser = await _userContainer.GetLoggedInUser(HttpContext);

            if (loggedInUser is null)
            {
                return NotFound();
            }

            var allTasks = await _taskContainer.GetMakandraTasks();
            var allTasksViewModel = new MakandraTaskUserViewModel(new MakandraTaskDetailViewModel())
            {
                User = loggedInUser,
            };

            allTasksViewModel.Tasklist = await allTasksViewModel.FillTaskList(allTasks, _roleContainer, _userContainer, _procedureContainer, _statesContainer);

            if (!string.IsNullOrEmpty(procedureFilter))
            {
                allTasksViewModel.Tasklist = await allTasksViewModel.FilterTaskListByProcedure(allTasksViewModel.Tasklist, procedureFilter, _procedureContainer);

            }
            allTasksViewModel.Tasklist = allTasksViewModel.SortTaskListByDate(allTasksViewModel.Tasklist, sortOrder);
            

            return View(allTasksViewModel);
        }

        /// <summary>
        /// Generates a list of all archived <see cref="MakandraTask"/> tasks
        /// currently in the database.
        /// Serves as an overview for supervisors.
        /// The list supports sorting by date by taking the sorting parameter
        /// from the <see cref="AllArchivedTasks.cshtml"/> by calling <see cref="SortTaskListByDate"/>
        /// and filtering by name of a <see cref="Procedure"/>
        /// </summary>
        /// <param name="sortOrder">The date sort pattern for the archived tasks. Can be <c>asc</c> or <c>desc</c></param>
        /// <param name="procedureFilter">The name of the <see cref="Procedure"/> to be filtered for</param> 
        /// <returns><see cref="MakandraTaskUserViewModel"/> allTasksViewModel with the tasklist filled with all active tasks</returns>
        /// <author>Thomas Dworschak</author>
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> AllArchivedTasks(string sortOrder, string procedureFilter)
        {
            if (string.IsNullOrEmpty(procedureFilter))
            {
                procedureFilter = HttpContext.Session.GetString("ProcedureFilter");
            }
            else
            {
                HttpContext.Session.SetString("ProcedureFilter", procedureFilter);
            }

            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["ProcedureFilter"] = procedureFilter;

            var loggedInUser = await _userContainer.GetLoggedInUser(HttpContext);

            if (loggedInUser is null)
            {
                return NotFound();
            }

            var allTasks = await _taskContainer.GetArchivedMakandraTasks();
            var allTasksViewModel = new MakandraTaskUserViewModel(new MakandraTaskDetailViewModel())
            {
                User = loggedInUser,
            };

            allTasksViewModel.Tasklist = await allTasksViewModel.FillTaskList(allTasks, _roleContainer, _userContainer, _procedureContainer, _statesContainer);

            if (!string.IsNullOrEmpty(procedureFilter))
            {
                allTasksViewModel.Tasklist = await allTasksViewModel.FilterTaskListByProcedure(allTasksViewModel.Tasklist, procedureFilter, _procedureContainer);
            }

            allTasksViewModel.Tasklist = allTasksViewModel.SortTaskListByDate(allTasksViewModel.Tasklist, sortOrder);

            return View(allTasksViewModel);
        }

        /// <summary>
        /// Based on the logged in <see cref="User"/> and his roles (<see cref="Role"/>), 
        /// this method creates a new <see cref="MakandraTaskArchiveUserViewModel"/> and fills
        /// its attributes.
        /// It fetches all <see cref="MakandraTask"/> tasks for this user, his roles,
        /// and the tasks of the <see cref="Procedure"/> he is responsible for that have been archived.
        /// The method takes sorting parameters from the <see cref="Index.cshtml"/> view
        /// to sort these task lists by calling <see cref="SortTaskListByDate"/>
        /// and <see cref="FilterTaskListByProcedure"/>
        /// </summary>
        /// <param name="userSortOrder">The date sort pattern for the tasks directly assigned to the user. Can be <c>asc</c> or <c>desc</c></param>
        /// <param name="responsibleSortOrder">The date sort pattern for the tasks of a procedure the user is responsible for. Can be <c>asc</c> or <c>desc</c></param>
        /// <param name="roleSortOrder">The date sort pattern for the tasks assigned to one or more of the user's roles. Can be <c>asc</c> or <c>desc</c></param>
        /// <param name="procedureFilter">The name of the procedure that will be used to display only tasks attached to this procedure. This filter is set for a 30 minute session</param>
        /// <returns><see cref="MakandraTaskArchiveUserViewModel"/> Archive view with archived tasks for user, roles, and responsible procedures</returns>
        /// <author>Thomas Dworschak</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Archive(string userSortOrder, string responsibleSortOrder, string[] roleSortOrder, string procedureFilter)
        {
            if (string.IsNullOrEmpty(procedureFilter))
            {
                procedureFilter = HttpContext.Session.GetString("ProcedureFilter");
            }
            else
            {
                HttpContext.Session.SetString("ProcedureFilter", procedureFilter);
            }

            ViewData["UserDateSortParam"] = userSortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["ResponsibleDateSortParam"] = responsibleSortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["ProcedureFilter"] = procedureFilter;

            var loggedInUser = await _userContainer.GetLoggedInUser(HttpContext);

            if (loggedInUser is null)
            {
                return NotFound();
            }

            var loggedInRoles = (await _userRolesContainer.GetRolesFromUser(loggedInUser)).ToList();
            var responsibleProcedures = await _procedureContainer.getProceduresFromUser(loggedInUser);
            List<int> responsibleProcduresIds = responsibleProcedures.Select(p => p.Id).ToList();

            var archivedTasksForUser = await _taskContainer.GetArchivedMakandraTasksForUser(loggedInUser.Id);
            var archivedTasksForResponsibleUser = await _taskContainer.GetArchivedMakandraTasksForResponsiblePerson(responsibleProcduresIds);
            var archivedTasksForResponsibleUSerUnassigned = archivedTasksForResponsibleUser.Where(t => t.AssigneeId != loggedInUser.Id).ToList();

            var archiveViewModel = new MakandraTaskArchiveUserViewModel(new MakandraTaskDetailViewModel())
            {
                User = loggedInUser,
                Roles = loggedInRoles,
            };

            archiveViewModel.Tasklist = await archiveViewModel.FillTaskList(archivedTasksForUser, _roleContainer, _userContainer, _procedureContainer, _statesContainer);
            archiveViewModel.TasksForRoles = await archiveViewModel.FillArchivedTasksForRoles(loggedInRoles, _roleContainer, _userContainer, _procedureContainer, _statesContainer);
            archiveViewModel.TasksForResponsible = await archiveViewModel.FillTaskList(archivedTasksForResponsibleUSerUnassigned, _roleContainer, _userContainer, _procedureContainer, _statesContainer);

            if (!string.IsNullOrEmpty(procedureFilter))
            {
                archiveViewModel.Tasklist = await archiveViewModel.FilterTaskListByProcedure(archiveViewModel.Tasklist, procedureFilter);
                archiveViewModel.TasksForResponsible = await archiveViewModel.FilterTaskListByProcedure(archiveViewModel.TasksForResponsible, procedureFilter);

                for (int i = 0; i < archiveViewModel.TasksForRoles.Count; i++)
                {
                    var roleTasks = archiveViewModel.TasksForRoles[i];
                    var filteredTasks = await archiveViewModel.FilterTaskListByProcedure(roleTasks, procedureFilter);
                    archiveViewModel.TasksForRoles[i] = filteredTasks;
                }
            }

            archiveViewModel.Tasklist = archiveViewModel.SortTaskListByDate(archiveViewModel.Tasklist, userSortOrder);
            archiveViewModel.TasksForResponsible = archiveViewModel.SortTaskListByDate(archiveViewModel.TasksForResponsible, responsibleSortOrder);

            for (int i = 0; i < loggedInRoles.Count; i++)
            {
                string currentRoleSortOrder = roleSortOrder.Length > i ? roleSortOrder[i] : null;
                ViewData[$"Role{i}DateSortParam"] = currentRoleSortOrder == "date_asc" ? "date_desc" : "date_asc";

                archiveViewModel.TasksForRoles[i] = archiveViewModel.SortTaskListByDate(archiveViewModel.TasksForRoles[i], currentRoleSortOrder);
            }

            return View(archiveViewModel);
        }

        /// <summary>
        /// Creates a <see cref="MakandraTaskDetailViewModel"/> based on the
        /// <see cref="MakandraTask"/> Id of a task
        /// </summary>
        /// <param name="id">Id of the task whose details are supposed to be displayed</param>
        /// <returns><see cref="MakandraTaskDetailViewModel"/>of the desired task. Returns <c>NotFound</c> in case an invalid Id has been handed</returns>
        /// <author>Thomas Dworschak</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Detail(int id)
        {
            var task = await _taskContainer.GetMakandraTaskFromId(id);

            if (task is null)
            {
                return NotFound();
            }

            MakandraTaskDetailViewModel detailViewModel = new MakandraTaskDetailViewModel();

            return View(await detailViewModel.CreateDetailViewModel(task, _roleContainer, _userContainer, _procedureContainer, _statesContainer));
        }

        /// <summary>
        /// Creates a <see cref="MakandraTaskEditViewModel"/> based on the id handed to the method
        /// by retrieving a <see cref="MakandraTask"/> by calling the method <see cref="GetAccessibleTask"/>
        /// which makes sure, that the Edit caller has the right to edit the task.
        /// To achieve correct access rights, a list of <see cref="Role"/> roles the logged-in <see cref="User"/> has
        /// and <see cref="Procedure"/> procedures this user is responsible for. These informations are handed to
        /// <c>GetAccessibleTask</c>
        /// If no such task is returned, the method returns with a <c>NotFound</c> error
        /// Then the attribute values of the retrieved task together with the list of roles
        /// associated with the task are used to fill the edit view model by
        /// calling <see cref="CreateEditViewModel"/>
        /// </summary>
        /// <param name="id">Id of the <see cref="MakandraTask"/> task that is supposed to be edited</param>
        /// <returns><see cref="MakandraTaskEditViewModel"/> with the attributes of the <see cref="MakandraTask"/> with the same id. Or a <c>NotFound</c> error if no task has been found</returns>
        /// <author>Thomas Dworschak</author>
        [HttpGet]
        [PermissionChecker("")]
        public async Task<IActionResult> Edit(int id)
        {
            var loggedInUser = await _userContainer.GetLoggedInUser(HttpContext);
            var loggedInRoles = await _userRolesContainer.GetRolesFromUser(loggedInUser);
            var responsibleProcedures = await _procedureContainer.getProceduresFromUser(loggedInUser);

            List<int> responsibleProcduresIds = responsibleProcedures.Select(p => p.Id).ToList();
            List<int> roleIds = loggedInRoles.Select(r => r.Id).ToList();

            var taskTet = await _taskContainer.GetMakandraTaskFromId(id);

            var taskToEdit = await _taskContainer.GetAccessibleTask(id, loggedInUser.Id, responsibleProcduresIds, roleIds);

            if (taskToEdit == null)
            {
                return NotFound();
            }

            var allRoles = _roleContainer.GetRoles();
            var alreadySelectedRoles = await _taskRoleContainer.GetRolesFromTask(taskToEdit);

            var selectedRoles = new List<MakandraTaskRoleViewModel>();
            var selectedRoleIds = new List<int>();

            foreach (Role role in allRoles)
            {
                MakandraTaskRoleViewModel model = new MakandraTaskRoleViewModel
                {
                    Name = role.Name,
                    Id = role.Id,
                    IsSelected = alreadySelectedRoles != null && alreadySelectedRoles.Any(rt => rt.RoleId == role.Id)
                };

                selectedRoles.Add(model);
                selectedRoleIds.Add(model.Id);
            }

            var taskEditViewModel = await new MakandraTaskEditViewModel().CreateEditViewModel(taskToEdit, selectedRoles, selectedRoleIds, _roleContainer, _statesContainer, _userContainer);

            return View(taskEditViewModel);
        }

        /// <summary>
        /// Takes an <see cref="MakandraTaskEditViewModel"/> with possibly altered attributes
        /// and hands the changed values down to the <see cref="MakandraTask"/>-Model,
        /// where the changes are applied directly.
        /// Before calling <see cref="UpdateTaskDetails"/> in the model, this method
        /// gathers all <see cref="Role"/> roles that should have edit access and
        /// have been selected in the view.
        /// </summary>
        /// <param name="taskEditViewModel"></param>
        /// <returns>In case of a successful update, the method returns to the <see cref="Index.cshtml"/> view</returns>
        /// <author>Thomas Dworschak</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("")]
        public async Task<IActionResult> Edit([Bind("Id, Name, Instruction, SelectedRoleId, PossibleEditors, TargetDate, Notes, SelectedStateId, SelectedAssigneeId, Archived")] MakandraTaskEditViewModel taskEditViewModel)
        { 
            if (!ModelState.IsValid)
            {
                taskEditViewModel.SetPossibleTaskStates(_statesContainer);
                return View(taskEditViewModel);
            }

            MakandraContext db = new MakandraContext();        
            List<int> selectedEditors = new List<int>();
            List<Role?> roles = new List<Role?>();

            foreach (var possibleEditorId in taskEditViewModel.PossibleEditors)
            {
                if (possibleEditorId.IsSelected)
                {
                    selectedEditors.Add(possibleEditorId.Id);
                }
            }

            foreach (var id in selectedEditors)
            {
                roles.Add(await _roleContainer.GetRoleFromId(id));
            }

            var taskToEdit = await _taskContainer.GetMakandraTaskFromId(taskEditViewModel.Id);

            if (taskToEdit == null)
            {
                return NotFound();
            }
            

            await taskToEdit.UpdateTaskDetails(
                _roleContainer,
                _taskRoleContainer,
                _statesContainer,
                _userContainer,
                taskEditViewModel.Name,
                taskEditViewModel.Instruction,
                taskEditViewModel.TargetDate,
                taskEditViewModel.Notes,
                taskEditViewModel.SelectedAssigneeId,
                taskEditViewModel.Archived,
                taskEditViewModel.SelectedRoleId,
                taskEditViewModel.SelectedStateId,
                roles,
                selectedEditors
            );

            _taskContainer = new MakandraTaskContainer(db);
            await _taskContainer.UpdateMakandraTask(taskToEdit);

            return RedirectToAction("Index");
        }
    }
}