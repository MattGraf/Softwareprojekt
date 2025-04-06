using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Replay.Container;
using Replay.ViewModels;
using Replay.Data;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Replay.Models;
using Replay.Models.MTM;

using Replay.ViewModels.TaskTemplateViewModels;
using Replay.Authorization;

using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Replay.ViewModels.Process;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Replay.Models.Account;
using Replay.Container.Account.MTM;
using Replay.Models.Account.MTM;

namespace Replay.Controllers
{
    /// <summary>
    /// Controller to change the views for the <see cref="TaskTemplate"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class TaskTemplateController : Controller
    {
        private ContractTypesContainer _contractTypesContainer;
        private DuedateContainer _duedateContainer;
        private DepartmentContainer _departmentContainer;
        private TaskTemplateContainer _taskTemplateContainer;
        private TaskTemplateContractTypeContainer _taskTemplateContractTypeContainer;
        private TaskTemplateDepartmentContainer _taskTemplateDepartmentContainer;
        private TaskTemplateRoleContainer _taskTemplateRoleContainer;

        private MakandraProcessContainer _makandraProcessContainer;
        private MakandraProcessRoleContainer _makandraProcessRoleContainer;
        private RoleContainer _roleContainer;
        private UserRolesContainer _userRolesContainer;
        private UserContainer _userContainer;

        /// <summary>
        /// Creates a TaskTemplateController with the needed Container
        /// </summary>
        /// <param name="contractTypesContainer">Container for the connection to the database</param>
        /// <param name="duedateContainer">Container for the connection to the database</param>
        /// <param name="departmentContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateContractTypeContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateDepartmentContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateRoleContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessContainer">Container for the connection to the database</param>
        /// <param name="makandraProcessRoleContainer">Container for the connection to the database</param>
        /// <param name="roleContainer">Container for the connection to the database</param>
        /// <param name="userRolesContainer">Container for the connection to the database</param>
        /// <param name="userContainer">Container for the connection to the database</param>
        /// <author>Matthias Grafberger</author>
        public TaskTemplateController(
            ContractTypesContainer contractTypesContainer, DuedateContainer duedateContainer, DepartmentContainer departmentContainer, TaskTemplateContainer taskTemplateContainer, TaskTemplateContractTypeContainer taskTemplateContractTypeContainer, TaskTemplateDepartmentContainer taskTemplateDepartmentContainer, TaskTemplateRoleContainer taskTemplateRoleContainer, MakandraProcessContainer makandraProcessContainer, MakandraProcessRoleContainer makandraProcessRoleContainer, RoleContainer roleContainer, UserRolesContainer userRolesContainer, UserContainer userContainer)
        {
            _contractTypesContainer = contractTypesContainer;
            _duedateContainer = duedateContainer;
            _departmentContainer = departmentContainer;
            _taskTemplateContainer = taskTemplateContainer;
            _taskTemplateContractTypeContainer = taskTemplateContractTypeContainer;
            _taskTemplateDepartmentContainer = taskTemplateDepartmentContainer;
            _taskTemplateRoleContainer = taskTemplateRoleContainer;
            _makandraProcessContainer = makandraProcessContainer;
            _makandraProcessRoleContainer = makandraProcessRoleContainer;
            _roleContainer = roleContainer;
            _userRolesContainer = userRolesContainer;
            _userContainer = userContainer;


        }

        /// <summary>
        /// Change the View to overview of the TaskTemplates
        /// </summary>
        /// <returns>View to overview of the TaskTemplates</returns>
        /// <author>Matthias Grafberger</author>
        [PermissionChecker("")]
        public IActionResult Index() {
            int indexFilter;

            var filterIdClaim = User.FindFirst("IndexFilter");
            indexFilter = filterIdClaim != null ? int.Parse(filterIdClaim.Value) : 0;
   
            if (filterIdClaim is null) {
                var identity = (ClaimsIdentity) User.Identity;

                identity.AddClaim(new Claim("IndexFilter", indexFilter.ToString()));
 
                HttpContext.SignOutAsync();
                HttpContext.SignInAsync(new ClaimsPrincipal(identity));
            }

            ViewData["OldProcessId"] = indexFilter;
            ViewData["OldSortId"] = 1;

            User user = _userContainer.GetLoggedInUser(HttpContext).Result;

            TaskTemplateUserIndexViewModel taskTemplateUserIndexViewModel = new TaskTemplateUserIndexViewModel();
            taskTemplateUserIndexViewModel.ProcessId = indexFilter;
            taskTemplateUserIndexViewModel.NewProcessId = indexFilter;
            taskTemplateUserIndexViewModel.SortId = 1;
            taskTemplateUserIndexViewModel.SetTaskTemplateUserIndexViewModelListWithUser(user, _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer, _taskTemplateContainer, _taskTemplateRoleContainer);
            return View(taskTemplateUserIndexViewModel);
        }

        /// <summary>
        /// Reload the overview-view of the <see cref="TaskTemplate"/>s
        /// </summary>
        /// <param name="taskTemplateUserArchivedViewModel">Manipulated view of the overview</param>
        /// <param name="search">The number which decides the sorting of the <see cref="TaskTemplate">s</param>
        /// <returns>New View</returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [PermissionChecker("")]
        [ValidateAntiForgeryToken]
        public IActionResult Index(TaskTemplateUserIndexViewModel taskTemplateUserIndexViewModel) {

            User user = _userContainer.GetLoggedInUser(HttpContext).Result;

            if (ModelState.IsValid) {

                if(taskTemplateUserIndexViewModel.Filter) {
                    taskTemplateUserIndexViewModel.ProcessId = taskTemplateUserIndexViewModel.NewProcessId;

                    var identity = (ClaimsIdentity) User.Identity;

                    var oldClaim = identity.FindFirst("IndexFilter");
                    if (oldClaim != null)
                    {
                        identity.RemoveClaim(oldClaim);
                    }

                    identity.AddClaim(new Claim("IndexFilter", taskTemplateUserIndexViewModel.NewProcessId.ToString()));

                    HttpContext.SignOutAsync();
                    HttpContext.SignInAsync(new ClaimsPrincipal(identity));

                }
                if(taskTemplateUserIndexViewModel.NewSortId != 0) taskTemplateUserIndexViewModel.SortId = taskTemplateUserIndexViewModel.NewSortId;

                ViewData["OldProcessId"] = taskTemplateUserIndexViewModel.ProcessId;
                ViewData["OldSortId"] = taskTemplateUserIndexViewModel.SortId;

                taskTemplateUserIndexViewModel.SetTaskTemplateUserIndexViewModelListWithUser(user, _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer, _taskTemplateContainer, _taskTemplateRoleContainer);
                taskTemplateUserIndexViewModel.Filter = false;
                return View(taskTemplateUserIndexViewModel);
            }

            taskTemplateUserIndexViewModel.SetTaskTemplateUserIndexViewModelListWithUser(user, _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer, _taskTemplateContainer, _taskTemplateRoleContainer);
            return View(taskTemplateUserIndexViewModel);
        }

        /// <summary>
        /// Change the View to edit-view of the TaskTemplates
        /// </summary>
        /// <returns>View to edit-view of the TaskTemplates</returns>
        /// <author>Matthias Grafberger</author>
        [PermissionChecker("")]
        public IActionResult Edit() {
            int id = (int) TempData["EditId"];
            bool index = (bool) TempData["Index"];

            TaskTemplate taskTemplate = _taskTemplateContainer.GetTaskTemplateFromId(id).Result;

            TaskTemplateEditViewModel taskTemplateEditViewModel = new TaskTemplateEditViewModel();
            taskTemplateEditViewModel.Initialize(_departmentContainer);

            taskTemplateEditViewModel.ID = id;
            taskTemplateEditViewModel.Name = taskTemplate.Name;
            taskTemplateEditViewModel.Instruction = taskTemplate.Instruction;
            taskTemplateEditViewModel.Duedate = taskTemplate.DuedateID;
            taskTemplateEditViewModel.DefaultResponsible = taskTemplate.DefaultResponsible;
            taskTemplateEditViewModel.Archived = taskTemplate.Archived;
            taskTemplateEditViewModel.MakandraProcessId = taskTemplate.MakandraProcessId;
            taskTemplateEditViewModel.Index = index;
            taskTemplateEditViewModel.Duedates = _duedateContainer.GetDuedates().Result;

            for (int i = 0; i < 4; i++) {
                foreach (TaskTemplateContractType taskTemplateContractType in taskTemplate.TaskTemplateContractTypes) {
                    if (taskTemplateEditViewModel.ContractTypes[i].Name.Equals(taskTemplateContractType.ContractType.Name)) {
                        taskTemplateEditViewModel.ContractTypes[i].IsSelected = true;
                    }
                }
            }

            foreach (DepartmentViewModel departmentViewModel in taskTemplateEditViewModel.Departments) {
                foreach (TaskTemplateDepartment taskTemplateDepartment in taskTemplate.TaskTemplateDepartments) {
                    if (departmentViewModel.Name.Equals(taskTemplateDepartment.Department.Name)) {
                        departmentViewModel.IsSelected = true;
                    }
                }
            }

            User user = _userContainer.GetLoggedInUser(HttpContext).Result;

            taskTemplateEditViewModel.InitializeProcess(user, _roleContainer, _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer);

            return View(taskTemplateEditViewModel);
        }

        /// <summary>
        /// Gives the id of the <see cref="TaskTemplate"/> to the edit-function without relieving the index to the url 
        /// </summary>
        /// <param name="taskTemplateUserArchivedViewModel">ViewModel with the id of the <see cref="TaskTemplate"> which is to be edited</param>
        /// <returns>Edit-view of the <see cref="TaskTemplate"/></returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditIndex([Bind("EditId")] TaskTemplateUserIndexViewModel taskTemplateUserIndexViewModel) {
            TempData["EditId"] = taskTemplateUserIndexViewModel.EditId;
            TempData["Index"] = true;

            return RedirectToAction("Edit");
        }

        /// <summary>
        /// Gives the id of the <see cref="TaskTemplate"/> to the edit-function without relieving the index to the url 
        /// </summary>
        /// <param name="taskTemplateUserArchivedViewModel">ViewModel with the id of the <see cref="TaskTemplate"> which is to be edited</param>
        /// <returns>Edit-view of the <see cref="TaskTemplate"/></returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditArchive([Bind("EditId")] TaskTemplateUserArchivedViewModel taskTemplateUserArchivedViewModel) {
            TempData["EditId"] = taskTemplateUserArchivedViewModel.EditId;
            TempData["Index"] = false;

            return RedirectToAction("Edit");
        }


        /// <summary>
        /// Reacts to a edit-request of a <see cref="TaskTemplate"/>
        /// Edit a <see cref="TaskTemplate"/> with the given attributes from the view
        /// 
        /// Redirect to overview when creation is successful, else the view stays
        /// </summary>
        /// <param name="taskTemplateEditViewModel">Manipulated Model with the wanted information about the taskTemplate</param>
        /// <returns>Next view</returns>
        /// <author>Matthias Grafberger</author> 
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult Edit(TaskTemplateEditViewModel taskTemplateEditViewModel) {

            if (ModelState.IsValid) {

                Duedate duedate = _duedateContainer.GetDuedateFromId(taskTemplateEditViewModel.Duedate).Result;

                List<ContractType> contractTypes = taskTemplateEditViewModel.GetContractTypes();
                List<Department> departments = taskTemplateEditViewModel.GetDepartments(_departmentContainer);

                TaskTemplate.EditTaskTemplate(_taskTemplateContainer, _taskTemplateContractTypeContainer, _contractTypesContainer, _taskTemplateDepartmentContainer, _makandraProcessContainer, _makandraProcessRoleContainer, _roleContainer, _taskTemplateRoleContainer, taskTemplateEditViewModel.ID, taskTemplateEditViewModel.Name, taskTemplateEditViewModel.Instruction, contractTypes, departments, duedate, taskTemplateEditViewModel.DefaultResponsible, taskTemplateEditViewModel.Archived, taskTemplateEditViewModel.MakandraProcessId);

                if (taskTemplateEditViewModel.Index) {
                    return RedirectToAction("Index");
                } else {
                    return RedirectToAction("ArchivedTaskTemplatesIndex");
                }
            }

            taskTemplateEditViewModel.Duedates = _duedateContainer.GetDuedates().Result;

            User user = _userContainer.GetLoggedInUser(HttpContext).Result;

            taskTemplateEditViewModel.Initialize(_departmentContainer);
            taskTemplateEditViewModel.InitializeProcess(user, _roleContainer, _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer);

            return View(taskTemplateEditViewModel);
        }
        

/// <summary>
        /// Change the View to create-view of the TaskTemplates
        /// </summary>
        /// <returns>View to create-view of the TaskTemplates</returns>
        /// <author>Matthias Grafberger</author>
        [PermissionChecker("")]
        public IActionResult Create() {

            List<Duedate> duedates = _duedateContainer.GetDuedates().Result;

            User user = _userContainer.GetLoggedInUser(HttpContext).Result;

            TaskTemplateCreateViewModel taskTemplateCreateViewModel = new TaskTemplateCreateViewModel();
            taskTemplateCreateViewModel.Initialize(_departmentContainer);
            taskTemplateCreateViewModel.Duedates = duedates;
            taskTemplateCreateViewModel.InitializeProcess(user, _roleContainer,  _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer);
            return View(taskTemplateCreateViewModel);
        }




        /// <summary>
        /// Reacts to a creation-request of a <see cref="TaskTemplate"/>
        /// Creates a <see cref="TaskTemplate"/> with the given attributes from the view
        /// 
        /// Redirect to overview when creation is successful, else the view stays
        /// </summary>
        /// <param name="TaskTemplateCreateViewModel">Manipulated Model with the wanted information about the taskTemplate</param>
        /// <returns>Next view</returns>
        /// <author>Matthias Grafberger</author>                
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult Create(TaskTemplateCreateViewModel taskTemplateCreateViewModel) {

            if (ModelState.IsValid) {
                Duedate duedate = _duedateContainer.GetDuedateFromId(taskTemplateCreateViewModel.Duedate).Result;

                List<ContractType> contractTypes = taskTemplateCreateViewModel.GetContractTypes();
                List<Department> departments = taskTemplateCreateViewModel.GetDepartments(_departmentContainer);

                TaskTemplate.CreateTaskTemplate(_taskTemplateContainer, _taskTemplateContractTypeContainer, _contractTypesContainer, _taskTemplateDepartmentContainer, _makandraProcessContainer, _makandraProcessRoleContainer, _roleContainer, _taskTemplateRoleContainer, taskTemplateCreateViewModel.Name, taskTemplateCreateViewModel.Instruction, contractTypes, departments, duedate, taskTemplateCreateViewModel.DefaultResponsible, taskTemplateCreateViewModel.Archived, taskTemplateCreateViewModel.MakandraProcessId);

                return RedirectToAction("Index");
            }

            User user = _userContainer.GetLoggedInUser(HttpContext).Result;

            taskTemplateCreateViewModel.Duedates = _duedateContainer.GetDuedates().Result;
            taskTemplateCreateViewModel.InitializeProcess(user, _roleContainer, _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer);

            return View(taskTemplateCreateViewModel);
        }
        

        /// <summary>
        /// Change the View to archive-view of the <see cref="TaskTemplate"/>s of the <see cref="Models.Account.User"/>
        /// </summary>
        /// <returns>View to archive-view of the <see cref="TaskTemplate"/>s</returns>
        /// <author>Matthias Grafberger</author>
        [PermissionChecker("")]
        public IActionResult ArchivedTaskTemplatesIndex() {
            int archiveFilter;

            var filterIdClaim = User.FindFirst("ArchiveFilter");
            archiveFilter = filterIdClaim != null ? int.Parse(filterIdClaim.Value) : 0;
   
            if (filterIdClaim is null) {
                var identity = (ClaimsIdentity) User.Identity;

                identity.AddClaim(new Claim("ArchiveFilter", archiveFilter.ToString()));
 
                HttpContext.SignOutAsync();
                HttpContext.SignInAsync(new ClaimsPrincipal(identity));
            }

            ViewData["OldProcessId"] = archiveFilter;
            ViewData["OldSortId"] = 1;

            User user = _userContainer.GetLoggedInUser(HttpContext).Result;

            TaskTemplateUserArchivedViewModel taskTemplateUserArchivedViewModel = new TaskTemplateUserArchivedViewModel();
            taskTemplateUserArchivedViewModel.ProcessId = archiveFilter;
            taskTemplateUserArchivedViewModel.NewProcessId = archiveFilter;
            taskTemplateUserArchivedViewModel.SortId = 1;
            taskTemplateUserArchivedViewModel.SetTaskTemplateUserArchivedViewModelListWithUser(user, _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer, _taskTemplateContainer, _taskTemplateRoleContainer);
            return View(taskTemplateUserArchivedViewModel);
        }

        /// <summary>
        /// Reload thea archive-view with the new sorting
        /// </summary>
        /// <param name="taskTemplateUserArchivedViewModel">Manipulated view of the archive-overview</param>
        /// <param name="search">The number which decides the sorting of the <see cref="TaskTemplate">s</param>
        /// <returns>New View</returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult ArchivedTaskTemplatesIndex(TaskTemplateUserArchivedViewModel taskTemplateUserArchivedViewModel) {
             if (ModelState.IsValid) {

                if(taskTemplateUserArchivedViewModel.Filter) {
                    taskTemplateUserArchivedViewModel.ProcessId = taskTemplateUserArchivedViewModel.NewProcessId;
                
                    var identity = (ClaimsIdentity) User.Identity;

                    var oldClaim = identity.FindFirst("ArchiveFilter");
                    if (oldClaim != null)
                    {
                        identity.RemoveClaim(oldClaim);
                    }

                    identity.AddClaim(new Claim("ArchiveFilter", taskTemplateUserArchivedViewModel.NewProcessId.ToString()));

                    HttpContext.SignOutAsync();
                    HttpContext.SignInAsync(new ClaimsPrincipal(identity));

                }
                if(taskTemplateUserArchivedViewModel.NewSortId != 0) taskTemplateUserArchivedViewModel.SortId = taskTemplateUserArchivedViewModel.NewSortId;

                ViewData["OldProcessId"] = taskTemplateUserArchivedViewModel.ProcessId;
                ViewData["OldSortId"] = taskTemplateUserArchivedViewModel.SortId;

                User user = _userContainer.GetLoggedInUser(HttpContext).Result;
                
                taskTemplateUserArchivedViewModel.SetTaskTemplateUserArchivedViewModelListWithUser(user, _userRolesContainer, _makandraProcessRoleContainer, _makandraProcessContainer, _taskTemplateContainer, _taskTemplateRoleContainer);
                taskTemplateUserArchivedViewModel.Filter = false;
                return View(taskTemplateUserArchivedViewModel);
            }

            return View(taskTemplateUserArchivedViewModel);
        }

        /* TODO
        [HttpPost]
        public IActionResult ArchivedTaskTemplatesIndex() {

        }
        */

        /// <summary>
        /// Changes the view to the detail-view of a specific <see cref="TaskTemplate"/>
        /// </summary>
        /// <returns>detail-view of the specific <see cref="TaskTemplate"/></returns>
        /// <author>Matthias Grafberger</author>
        [PermissionChecker("")]
        public IActionResult Details() {
            int id = (int) TempData["DetailsId"];
            int index = (int) TempData["Index"];

            int processId = 0;
            if (index == 3) {
                processId = (int) TempData["ProcessId"];
            }

            TaskTemplate taskTemplate = _taskTemplateContainer.GetTaskTemplateFromId(id).Result;

            TaskTemplateDetailViewModel taskTemplateDetailViewModel = new TaskTemplateDetailViewModel(_makandraProcessContainer, taskTemplate, index, processId);
            return View(taskTemplateDetailViewModel);
        }

        /// <summary>
        /// Gives the id of the <see cref="TaskTemplate"/> to the detail-function without relieving the index to the url 
        /// </summary>
        /// <param name="taskTemplateUserArchivedViewModel">ViewModel with the id of the <see cref="TaskTemplate"> of which the detail-view is wanted</param>
        /// <returns>Detail-view of the <see cref="TaskTemplate"/></returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsIndex([Bind("DetailsId")] TaskTemplateUserIndexViewModel taskTemplateUserIndexViewModel) {
            TempData["DetailsId"] = taskTemplateUserIndexViewModel.DetailsId;
            TempData["Index"] = 1;

            return RedirectToAction("Details");
        }

        /// <summary>
        /// Gives the id of the <see cref="TaskTemplate"/> to the detail-function without relieving the index to the url 
        /// </summary>
        /// <param name="taskTemplateUserArchivedViewModel">ViewModel with the id of the <see cref="TaskTemplate"> of which the detail-view is wanted</param>
        /// <returns>Detail-view of the <see cref="TaskTemplate"/></returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        public IActionResult DetailsProcess(MakandraProcessDetailsViewModel makandraProcessDetailsViewModel) {
            TempData["DetailsId"] = makandraProcessDetailsViewModel.TaskTemplateId;
            TempData["Index"] = 3;
            TempData["ProcessId"] = makandraProcessDetailsViewModel.Id;

            return RedirectToAction("Details");
        }

        /// <summary>
        /// Returns to the view of the process if it is the source of the function
        /// </summary>
        /// <returns>View of the process if it is the source of the function</returns>
        /// <author>Matthias Grafberger</author>
        public IActionResult DetailsProcessReturn() {
           return RedirectToAction("Details", "MakandraProcess", new { id = @TempData["ProcessId"] });
           }

        /// <summary>
        /// Gives the id of the <see cref="TaskTemplate"/> to the detail-function without relieving the index to the url 
        /// </summary>
        /// <param name="taskTemplateUserArchivedViewModel">ViewModel with the id of the <see cref="TaskTemplate"> of which the detail-view is wanted</param>
        /// <returns>Detail-view of the <see cref="TaskTemplate"/></returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsArchive([Bind("DetailsId")] TaskTemplateUserArchivedViewModel taskTemplateUserArchivedViewModel) {
            TempData["DetailsId"] = taskTemplateUserArchivedViewModel.DetailsId;
            TempData["Index"] = 2;

            return RedirectToAction("Details");
        }

       /// <summary>
        /// Archives the <see cref="TaskTemplate"/> with the in the ViewModel given id
        /// </summary>
        /// <param name="taskTemplateUserIndexViewModel">Contains the id of the <see cref="TaskTemplate"/> which is to be archived</param>
        /// <returns>The Index-overview of <see cref="TaskTemplate"/></returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult Archive([Bind("ArchiveId")] TaskTemplateUserIndexViewModel taskTemplateUserIndexViewModel) {
            _taskTemplateContainer.TaskTemplateArchive(taskTemplateUserIndexViewModel.ArchiveId);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Restores the <see cref="TaskTemplate"/> with the in the ViewModel given id
        /// </summary>
        /// <param name="taskTemplateUserArchivedViewModel">Contains the id of the <see cref="TaskTemplate"/> which is to be restored</param>
        /// <returns>The Archive-overview of <see cref="TaskTemplate"/></returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult Restore([Bind("RestoreId")] TaskTemplateUserArchivedViewModel taskTemplateUserArchivedViewModel) {
            _taskTemplateContainer.TaskTemplateRestore(taskTemplateUserArchivedViewModel.RestoreId);

            return RedirectToAction("ArchivedTaskTemplatesIndex");
        }
    }
}