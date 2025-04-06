using Replay.Models;
using Replay.Models.Account;

namespace Replay.ViewModels.Task
{
    /// <summary>
    /// View Model to display all <see cref="MakandraTask"/> tasks.
    /// These tasks are stored in lists of <see cref="MakandraTaskDetailViewModel"/> view models.
    /// Sorted in three lists: The first list collects all tasks for one <see cref="User"/>
    /// The second list collects all tasks for one <see cref="Procedure"/> where
    /// one user is labelled as responsible person.
    /// The third lists collects lists of tasks for each <see cref="Role"/> in a set of roles.
    /// To fill these lists, the class offers several helper methords.
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class MakandraTaskUserViewModel
    {
        [Required]
        public User User { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
        public List<MakandraTaskDetailViewModel> Tasklist { get; set; } = new List<MakandraTaskDetailViewModel>();
        public List<MakandraTaskDetailViewModel> TasksForResponsible { get; set; } = new List<MakandraTaskDetailViewModel>();
        public List<List<MakandraTaskDetailViewModel>> TasksForRoles { get; set; } = new List<List<MakandraTaskDetailViewModel>>();
        private readonly MakandraTaskDetailViewModel _detailViewModel;
        public MakandraTaskUserViewModel() { }
        public MakandraTaskUserViewModel(MakandraTaskDetailViewModel detailViewModel)
        {
            _detailViewModel = detailViewModel;
        }
        
        /// <summary>
        /// Based on an input list of <see cref="MakandraTask"/> tasks
        /// this method creates a new <see cref="MakandraTaskDetailViewModel"/>
        /// for each tasks and adds it to a list of detail view models.
        /// </summary>
        /// <param name="taskList">List of <see cref="MakandraTask"/> tasks that should be added to the list</param>
        /// <param name="roleContainer">Container for <see cref="Role"/> that is needed in the <see cref="CreateDetailViewModel"/>-method</param>
        /// <param name="userContainer">Container for <see cref="User"/> that is needed in the <see cref="CreateDetailViewModel"/>-method</param>
        /// <param name="procedureContainer">Container for <see cref="Procedure"/> that is needed in the <see cref="CreateDetailViewModel"/>-method</param>
        /// <param name="stateContainer">Container for <see cref="MakandraTaskState"/> that is needed in the <see cref="CreateDetailViewModel"/>-method</param>
        /// <returns>A list of <see cref="MakandraTaskDetailViewModel"/></returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTaskDetailViewModel>> FillTaskList(List<MakandraTask> taskList, RoleContainer roleContainer, UserContainer userContainer, ProcedureContainer procedureContainer, MakandraTaskStateContainer stateContainer)
        {
            List<MakandraTaskDetailViewModel> tasksDetailViewList = new List<MakandraTaskDetailViewModel>();
            
            foreach(MakandraTask task in taskList)
            {
                MakandraTaskDetailViewModel detailViewModel = await _detailViewModel.CreateDetailViewModel(task, roleContainer, userContainer, procedureContainer, stateContainer);
                tasksDetailViewList.Add(detailViewModel);
            }

            return tasksDetailViewList;
        }

        /// <summary>
        /// This method creates a list of <see cref="MakandraTaskViewModel"/> view models
        /// based on <see cref="MakandraTask"/> tasks that are associated with one
        /// <see cref="Role"/> role.
        /// It takes all tasks where this role is labeled as responsible
        /// and no other <see cref="User"/> assignee has been entered.
        /// This task list is then handed over to <see cref="FillTaskList"/>
        /// and lastly attached to the wanted list of view models.
        /// </summary>
        /// <param name="userRole"><see cref="Role"/> for which <see cref="MakandraTask"/> tasks are to be found</param>
        /// <param name="taskContainer">Container for <see cref="MakandraTask"/> tasks to get the tasks</param>
        /// <param name="roleContainer">Container for <see cref="Role"/> that is needed in the <see cref="FillTaskList"/>-method</param>
        /// <param name="userContainer">Container for <see cref="User"/> that is needed in the <see cref="FillTaskList"/>-method</param>
        /// <param name="procedureContainer">Container for <see cref="Procedure"/> that is needed in the <see cref="FillTaskList"/>-method</param>
        /// <param name="stateContainer">Container for <see cref="MakandraTaskState"/> that is needed in the <see cref="FillTaskList"/>-method</param>
        /// <returns>A list of <see cref="MakandraTaskDetailViewModel"/></returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTaskDetailViewModel>> FillTasksForRole(Role userRole, MakandraTaskContainer taskContainer, RoleContainer roleContainer, UserContainer userContainer, ProcedureContainer procedureContainer, MakandraTaskStateContainer stateContainer)
        {
            List<MakandraTaskDetailViewModel> tasksDetailViewList = new List<MakandraTaskDetailViewModel>();

            List<MakandraTask> tasksForRole = (await taskContainer.GetMakandraTasksForRole(userRole.Id))
                .Where(t => t.AssigneeId == null)
                .ToList();

            tasksDetailViewList = await FillTaskList(tasksForRole, roleContainer, userContainer, procedureContainer, stateContainer);

            return tasksDetailViewList;
        }

        /// <summary>
        /// This methods fills separate lists of <see cref="MakandraTaskDetailViewModel"/> view models
        /// for each <see cref="Role"/> given in an input list of roles by looping through
        /// the <see cref="FillTasksForRole"/>-method for each input role
        /// </summary>
        /// <param name="userRoles">List of <see cref="Role"/> roles for which all <see cref="MakandraTask"/> tasks are to be collected that have a role but no <see cref="User"/> assigned</param>
        /// <param name="taskContainer">Container for <see cref="MakandraTask"/> that is needed in the <see cref="FillTasksForRole"/>-method</param>
        /// <param name="roleContainer">Container for <see cref="Role"/> that is needed in the <see cref="FillTasksForRole"/>-method</param>
        /// <param name="userContainer">Container for <see cref="User"/> that is needed in the <see cref="FillTasksForRole"/>-method</param>
        /// <param name="procedureContainer">Container for <see cref="Procedure"/> that is needed in the <see cref="FillTasksForRole"/>-method</param>
        /// <param name="stateContainer">Container for <see cref="MakandraTaskState"/> that is needed in the <see cref="FillTasksForRole"/>-method</param>
        /// <returns>A list of lists of <see cref="MakandraTaskDetailViewModel"/>: A collection of tasks for each role</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<List<MakandraTaskDetailViewModel>>> FillTasksForRoles(List<Role> userRoles, MakandraTaskContainer taskContainer, RoleContainer roleContainer, UserContainer userContainer, ProcedureContainer procedureContainer, MakandraTaskStateContainer stateContainer)
        {
            List<List<MakandraTaskDetailViewModel>> tasksForRoles = new List<List<MakandraTaskDetailViewModel>>();

            foreach(Role role in userRoles)
            {
                List<MakandraTaskDetailViewModel> tasksForOneRole = new List<MakandraTaskDetailViewModel>();

                tasksForOneRole = await FillTasksForRole(role, taskContainer, roleContainer, userContainer, procedureContainer, stateContainer);
                tasksForRoles.Add(tasksForOneRole);
            }

            return tasksForRoles;
        }

        /// <summary>
        /// Helper method that sorts a list of <see cref="MakandraTaskDetailViewModel"/> view models
        /// by date according to <c>sortOrder</c>
        /// </summary>
        /// <param name="taskList">List of <see cref="MakandraTaskDetailViewModel"/> whose elements are supposed to be sorted by date</param>
        /// <param name="sortOrder">String of sort pattern. Can be <c>asc</c> or <c>desc</c></param>
        /// <returns>The input list of view models, but sorted by date</returns>
        /// <author>Thomas Dworschak</author>
        public List<MakandraTaskDetailViewModel> SortTaskListByDate(List<MakandraTaskDetailViewModel> taskList, string sortOrder)
        {
            switch(sortOrder)
            {
                case "date_desc":
                    taskList = taskList.OrderByDescending(t => t.TargetDate).ToList();
                    break;
                case "date_asc":
                    taskList = taskList.OrderBy(t => t.TargetDate).ToList();
                    break;
            }

            return taskList;
        }

        /// <summary>
        /// Helper method that filers a list of <see cref="MakandraTaskDetailViewModel"/> view models
        /// to include only those view models whose associated <see cref="Procedure"/> matches
        /// the procedure name of the input string
        /// </summary>
        /// <param name="taskList">List of <see cref="MakandraTaskDetailViewModel"/> whose elements are supposed to be filtered by a procedure name</param>
        /// <param name="procedureFilter">String of the <see cref="Procedure"/> name</param>
        /// <param name="procedureContainer">Container for <see cref="Procedure"/> to find the procedure with <c>procedureFilter</c> name</param>
        /// <returns>A filtered list of view models</returns>
        /// <author>Thomas Dworschak</author>
        public async Task<List<MakandraTaskDetailViewModel>> FilterTaskListByProcedure(List<MakandraTaskDetailViewModel> taskList, string procedureFilter, ProcedureContainer procedureContainer)
        {
            List<Procedure> allProcedures = await procedureContainer.getAllProcedures();
            Procedure filteredProcedure = allProcedures.Find(p => p.name.Equals(procedureFilter));

            string filteredName = filteredProcedure?.name;

            return taskList = taskList.Where(t => t.Procedure.Equals(filteredName)).ToList();
        }
    }
}