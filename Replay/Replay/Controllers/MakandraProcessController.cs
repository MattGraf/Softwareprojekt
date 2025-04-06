using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Replay.Models;
using Replay.Authorization;
using Replay.ViewModels;
using Replay.ViewModels.Process;
using Replay.Models.Account;
using Replay.Models.MTM;
using Replay.Container.Account.MTM;

namespace Replay.Controllers
{
    public class MakandraProcessController : Controller
    {
        public MakandraContext MakandraContext;
        public MakandraProcessContainer MakandraProcessContainer;
        public List<MakandraProcess> Processes;
        public UserContainer UserContainer;
        public RoleContainer RoleContainer;
        public TaskTemplateContainer TaskTemplateContainer;
        public MakandraProcessRoleContainer MakandraProcessRoleContainer;
        public DepartmentContainer DepartmentContainer;
        public ContractTypesContainer ContractTypesContainer;
        public MakandraTaskContainer MakandraTaskContainer;
        public MakandraTaskStateContainer MakandraTaskStateContainer;
        public MakandraTaskRoleContainer MakandraTaskRoleContainer;
        public DuedateContainer DuedateContainer;
        public ProcedureContainer ProcedureContainer;
        public TaskTemplateDepartmentContainer TaskTemplateDepartmentContainer;
        public TaskTemplateContractTypeContainer TaskTemplateContractTypeContainer;
        public UserRolesContainer UserRolesContainer;
        public ProcedureDepartmentContainer ProcedureDepartmentContainer;

        /// <summary>
        /// Creates a new MakandraProcessController using a lot of containers.
        /// </summary>
        /// <param name="MakandraContext">A MakandraContext</param>
        /// <param name="MakandraProcessContainer">A MakandraProcessContainer</param>
        /// <param name="UserContainer">A UserContainer</param>
        /// <param name="RoleContainer">A RoleContainer</param>
        /// <param name="TaskTemplateContainer">A TaskTemplateContainer</param>
        /// <param name="MakandraProcessRoleContainer">A MakandraProcessRoleContainer</param>
        /// <param name="DepartmentContainer">A DepartmentContainer</param>
        /// <param name="ContractTypesContainer">A ContractTypesContainer</param>
        /// <param name="MakandraTaskContainer">A MakandraTaskContainer</param>
        /// <param name="MakandraTaskStateContainer">A MakandraTaskStateContainer</param>
        /// <param name="MakandraTaskRoleContainer">A MakandraTaskRoleContainer</param>
        /// <param name="DuedateContainer">A DuedateContainer</param>
        /// <param name="ProcedureContainer">A ProcedureContainer</param>
        /// <param name="TaskTemplateDepartmentContainer">A TaskTemplateDepartmentContainer</param>
        /// <param name="TaskTemplateContractTypeContainer">A TaskTemplateContractTypeContainer</param>
        /// <param name="UserRolesContainer">A UserRolesContainer</param>
        /// <author>Arian Scheremet</author>         
        public MakandraProcessController(MakandraContext MakandraContext = null, MakandraProcessContainer MakandraProcessContainer = null, UserContainer UserContainer = null, RoleContainer RoleContainer = null, TaskTemplateContainer TaskTemplateContainer = null, MakandraProcessRoleContainer MakandraProcessRoleContainer = null, DepartmentContainer DepartmentContainer = null, ContractTypesContainer ContractTypesContainer = null, MakandraTaskContainer MakandraTaskContainer = null, MakandraTaskStateContainer MakandraTaskStateContainer = null, MakandraTaskRoleContainer MakandraTaskRoleContainer = null, DuedateContainer DuedateContainer = null, ProcedureContainer ProcedureContainer = null, TaskTemplateDepartmentContainer TaskTemplateDepartmentContainer = null, TaskTemplateContractTypeContainer TaskTemplateContractTypeContainer = null, UserRolesContainer UserRolesContainer = null, ProcedureDepartmentContainer procedureDepartmentContainer = null, DepartmentContainer departmentContainer = null)
        {
            if (MakandraContext is null) this.MakandraContext = new MakandraContext();
            else this.MakandraContext = MakandraContext;
            if (MakandraProcessContainer is null) this.MakandraProcessContainer = new MakandraProcessContainer(this.MakandraContext);
            else this.MakandraProcessContainer = MakandraProcessContainer;
            if (RoleContainer is null) this.RoleContainer = new RoleContainer(this.MakandraContext);
            else this.RoleContainer = RoleContainer;
            if (UserContainer is null) this.UserContainer = new UserContainer(this.MakandraContext);
            else this.UserContainer = UserContainer;
            if (TaskTemplateContainer is null) this.TaskTemplateContainer = new TaskTemplateContainer(this.MakandraContext);
            else this.TaskTemplateContainer = TaskTemplateContainer;
            if (MakandraProcessRoleContainer is null) this.MakandraProcessRoleContainer = new MakandraProcessRoleContainer(this.MakandraContext);
            else this.MakandraProcessRoleContainer = MakandraProcessRoleContainer;
            if (DepartmentContainer is null) this.DepartmentContainer = new DepartmentContainer(this.MakandraContext);
            else this.DepartmentContainer = DepartmentContainer;
            if (ContractTypesContainer is null) this.ContractTypesContainer = new ContractTypesContainer(this.MakandraContext);
            else this.ContractTypesContainer = ContractTypesContainer;
            if (MakandraTaskContainer is null) this.MakandraTaskContainer = new MakandraTaskContainer(this.MakandraContext);
            else this.MakandraTaskContainer = MakandraTaskContainer;
            if (MakandraTaskStateContainer is null) this.MakandraTaskStateContainer = new MakandraTaskStateContainer(this.MakandraContext);
            else this.MakandraTaskStateContainer = MakandraTaskStateContainer;
            if (MakandraTaskRoleContainer is null) this.MakandraTaskRoleContainer = new MakandraTaskRoleContainer(this.MakandraContext);
            else this.MakandraTaskRoleContainer = MakandraTaskRoleContainer;
            if (DuedateContainer is null) this.DuedateContainer = new DuedateContainer(this.MakandraContext);
            else this.DuedateContainer = DuedateContainer;
            if (ProcedureContainer is null) this.ProcedureContainer = new ProcedureContainer(this.MakandraContext);
            else this.ProcedureContainer = ProcedureContainer;
            if (TaskTemplateDepartmentContainer is null) this.TaskTemplateDepartmentContainer = new TaskTemplateDepartmentContainer(this.MakandraContext);
            else this.TaskTemplateDepartmentContainer = TaskTemplateDepartmentContainer;
            if (TaskTemplateContractTypeContainer is null) this.TaskTemplateContractTypeContainer = new TaskTemplateContractTypeContainer(this.MakandraContext);
            else this.TaskTemplateContractTypeContainer = TaskTemplateContractTypeContainer;
            if (UserRolesContainer is null) this.UserRolesContainer = new UserRolesContainer(this.MakandraContext);
            else this.UserRolesContainer = UserRolesContainer;

            if (ProcedureDepartmentContainer is null) this.ProcedureDepartmentContainer = new ProcedureDepartmentContainer(this.MakandraContext);
            else this.ProcedureDepartmentContainer = ProcedureDepartmentContainer;

            Processes = this.MakandraProcessContainer.GetProcesses().Result;
        }

        /// <summary>
        /// Redirects the User to the MakandraProcess Index page.
        /// </summary>
        /// <author>Arian Scheremet</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Index()
        {
            MakandraProcessIndexViewModel indexViewModel = new MakandraProcessIndexViewModel
            {
                MakandraProcesses = Processes,
            };

            return View(indexViewModel);
        }

        /// <summary>Redirects the User to the MakandraProcess Create page if the User is Administrator</summary>
        /// <author>Arian Scheremet</author>
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> Create()
        {
            MakandraProcessCreateViewModel CreateViewModel = new MakandraProcessCreateViewModel
            {
                AllTasks = TaskTemplateContainer,
                AllRoles = RoleContainer,
                Roles = new RoleSelectionViewModel[RoleContainer.GetRoles().Count()]
            };

            int i = 0;
            foreach (var r in RoleContainer.GetRoles())
            {
                CreateViewModel.Roles[i] = new RoleSelectionViewModel
                {
                    Name = r.Name,
                    Id = r.Id,
                    IsSelected = false
                };
                i++;
            }

            return View(CreateViewModel);
        }

        /// <summary>
        /// Creates a MakandraProcess and posts it to the Website if the User is Administrator
        /// </summary>
        /// <param name="createViewModel">A MakandraProcessCreateViewModel with values entered by the user on the Create page</param>
        /// <author>Arian Scheremet</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> Create([Bind("Name, Tasks, EditAccess, Roles")] MakandraProcessCreateViewModel createViewModel)
        {
            if (ModelState.IsValid)
            {
                foreach (var k in createViewModel.Roles)
                {
                    if (k.IsSelected)
                    {
                        createViewModel.EditAccess.Add(RoleContainer.GetRoleFromId(k.Id).Result);
                    }
                }
                createViewModel.EditAccess.Add(RoleContainer.GetRoleFromName("Administrator").Result);

                MakandraProcess.CreateMakandraProcess(MakandraProcessContainer, MakandraProcessRoleContainer, RoleContainer, createViewModel.Name, createViewModel.Tasks, createViewModel.EditAccess);
                Processes = MakandraProcessContainer.GetProcesses().Result;
                return RedirectToAction("Index");
            }

            return View(createViewModel);
        }

        /// <summary>
        /// Redirects the User to a specific MakandraProcess' Edit page if they have permission to edit this MakandraProcess
        /// </summary>
        /// <param name="Id">The Id of the MakandraProcess that is to be edited</param>
        /// <author>Arian Scheremet</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Edit(int Id)
        {
            if (ModelState.IsValid)
            {
                MakandraProcess ProcessToEdit = await MakandraProcessContainer.GetProcessFromId(Id);
                if (!CheckPermissions(ProcessToEdit).Result) return RedirectToAction("Index");
                if (ProcessToEdit is null) return RedirectToAction("Index");

                int[] Ids = MakandraProcessRoleContainer.GetAssociatedRoleIDsFromMakandraProcess(ProcessToEdit).Result;

                RoleSelectionViewModel[] RoleSelection = new RoleSelectionViewModel[RoleContainer.GetRoles().Count()];
                int i = 0;
                foreach (Role r in RoleContainer.GetRoles())
                {
                    RoleSelection[i] = new RoleSelectionViewModel
                    {
                        Name = r.Name,
                        Id = r.Id,
                        IsSelected = false
                    };
                    if (Ids.Contains(r.Id)) RoleSelection[i].IsSelected = true;
                    i++;
                }

                MakandraProcessEditViewModel EditViewModel = new MakandraProcessEditViewModel
                {
                    Roles = RoleContainer,
                    AllTaskTemplatesForThisProcess = TaskTemplateContainer,
                    Name = ProcessToEdit.Name,
                    Id = ProcessToEdit.Id,
                    RoleSelection = RoleSelection
                };

                return View(EditViewModel);
            }
            return View("Index");
        }

        /// <summary>
        /// Saves changes made to a MakandraProcess by the User if they have permission to edit this MakandraProcess
        /// </summary>
        /// <param name="editViewModel">A MakandraProcessEditViewModel with values entered by the User on a MakandraProcess' edit page</param>
        /// <author>Arian Scheremet</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("")]
        public async Task<IActionResult> Edit([Bind("Id, Name, Tasks, EditAccess, RoleSelection")] MakandraProcessEditViewModel editViewModel)
        {
            MakandraProcess permissionCheck = MakandraProcessContainer.GetProcessFromId(editViewModel.Id).Result;
            if (!CheckPermissions(permissionCheck).Result) return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                List<Role> NewRoles = new List<Role>();

                foreach (var k in editViewModel.RoleSelection)
                {
                    if (k.IsSelected)
                    {
                        NewRoles.Add(RoleContainer.GetRoleFromId(k.Id).Result);
                    }
                }
                NewRoles.Add(RoleContainer.GetRoleFromName("Administrator").Result);

                MakandraProcess.EditMakandraProcess(MakandraProcessContainer, MakandraProcessRoleContainer, RoleContainer, editViewModel.Id, editViewModel.Name, NewRoles);
                return RedirectToAction("Index");
            }
            return View(editViewModel);
        }

        /// <summary>
        /// Redirects the user to a specific MakandraProcess' Details page
        /// </summary>
        /// <param name="Id">The Id of the MakandraProcess that is to be viewed</param>
        /// <author>Arian Scheremet</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Details(int Id)
        {
            MakandraProcess ProcessToView = await MakandraProcessContainer.GetProcessFromId(Id);
            if (ProcessToView is null) return RedirectToAction("Index");
            if (ProcessToView.MakandraProcessRoles is null) ProcessToView.MakandraProcessRoles = new List<MakandraProcessRole>();
            if (ProcessToView.Tasks is null) ProcessToView.Tasks = new List<TaskTemplate>();

            int[] RoleIDs = MakandraProcessRoleContainer.GetAssociatedRoleIDsFromMakandraProcess(ProcessToView).Result;
            RoleSelectionViewModel[] RoleSelection = new RoleSelectionViewModel[RoleIDs.Count()];
            int i = 0;
            foreach (int r in RoleIDs)
            {
                Role role = RoleContainer.GetRoleFromId(r).Result;
                RoleSelection[i] = new RoleSelectionViewModel
                {
                    Name = role.Name,
                    Id = r,
                    IsSelected = true
                };
                i++;
            }

            List<TaskTemplate> NotArchivedTasks = TaskTemplateContainer.GetTaskTemplatesNotArchivedWithProcessId(ProcessToView.Id).Result;
            List<TaskTemplate> ArchivedTasks = TaskTemplateContainer.GetTaskTemplatesArchivedWithProcessId(ProcessToView.Id).Result;

            MakandraProcessDetailsViewModel DetailsViewModel = new MakandraProcessDetailsViewModel
            {
                Name = ProcessToView.Name,
                EditAccess = ProcessToView.MakandraProcessRoles,
                Tasks = NotArchivedTasks,
                ArchivedTasks = ArchivedTasks,
                RoleSelection = RoleSelection,
                Id = ProcessToView.Id
            };

            return View(DetailsViewModel);
        }

        /// <summary>
        /// Redirects the User to a specific MakandraProcess' Start page if the User has permission to start this MakandraProcess
        /// </summary>
        /// <param name="Id">The Id of the MakandraProcess that is to be started as a Procedure</param>
        /// <author>Arian Scheremet</author>
        [PermissionChecker("")]
        public async Task<IActionResult> Start(int Id)
        {
            MakandraProcess processToStart = await MakandraProcessContainer.GetProcessFromId(Id);
            if (!CheckPermissions(processToStart).Result) return RedirectToAction("Index");
            if (processToStart is null) return RedirectToAction("Index");

            MakandraProcessStartViewModel startViewModel = new MakandraProcessStartViewModel();
            startViewModel.SetMakandraProcessStartViewModel();
            startViewModel.ProcessId = processToStart.Id;
            startViewModel.DefaultResponsiblePerson = UserContainer.GetLoggedInUser(HttpContext).Result;
            startViewModel.Name = processToStart.Name;
            startViewModel.Deadline = new DateTime(2024, 8, 1);

            return View(startViewModel);
        }

        /// <summary>
        /// Starts a MakandraProcess as a Procedure if the User has permission to start this MakandraProcess
        /// </summary>
        /// <param name="startViewModel">A MakandraProcessStartViewModel with values entered by the user on a MakandraProcess' Start page</param>
        /// <author>Arian Scheremet</author>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionChecker("")]
        public async Task<IActionResult> Start([Bind("ProcessId, Name, Deadline, EstablishingContractType, TargetDepartment, ReferencePerson, ResponsiblePerson, Departments")] MakandraProcessStartViewModel startViewModel)
        {
            MakandraProcess processToStart = MakandraProcessContainer.GetProcessFromId(startViewModel.ProcessId).Result;
            if (!CheckPermissions(processToStart).Result) return RedirectToAction("Index");
            if (ModelState.IsValid)
            {

                List<Department> selectedDepartments = new List<Department>();

                foreach (var d in startViewModel.Departments)
                {
                    if (d.IsSelected)
                    {
                        selectedDepartments.Add(DepartmentContainer.GetDepartmentFromId(d.Id).Result);
                    }
                }

                List<TaskTemplate> taskTemplates = TaskTemplateContainer.GetTaskTemplatesNotArchivedWithProcessId(startViewModel.ProcessId).Result;
                User selectedResponsiblePerson = UserContainer.GetUserFromId(startViewModel.ResponsiblePerson).Result;
                User selectedReferencePerson = UserContainer.GetUserFromId(startViewModel.ReferencePerson).Result;
                ContractType selectedEstablishingContractType = ContractTypesContainer.GetContractTypeFromID(startViewModel.EstablishingContractType).Result;
                processToStart.Tasks = taskTemplates;


                Procedure startedProcedure = Procedure.CreateProcedure(processToStart, startViewModel.Deadline, selectedReferencePerson, selectedResponsiblePerson, startViewModel.Name, selectedEstablishingContractType, selectedDepartments, MakandraTaskContainer, MakandraTaskStateContainer, MakandraTaskRoleContainer, DuedateContainer, ProcedureContainer, RoleContainer, UserContainer, TaskTemplateDepartmentContainer, TaskTemplateContractTypeContainer, ProcedureDepartmentContainer, DepartmentContainer).Result;

                return RedirectToAction("Index", "Procedure");
            }

            startViewModel.DefaultResponsiblePerson = this.UserContainer.GetUserFromId(startViewModel.ResponsiblePerson).Result;
            return View(startViewModel);
        }

        /// <summary>
        /// Deletes a MakandraProcess with the given Id from the database if the User is Administrator
        /// </summary>
        /// <param name="Id">The Id of the MakandraProcess that is to be deleted</param>
        /// <author>Arian Scheremet</author>
        [PermissionChecker("Administrator")]
        public async Task<IActionResult> Delete(int Id)
        {
            MakandraProcess ProcessToDelete = await MakandraProcessContainer.GetProcessFromId(Id);
            if (ProcessToDelete is null) return RedirectToAction("Create");
            MakandraProcessContainer.DeleteProcess(ProcessToDelete);
            Processes = MakandraProcessContainer.GetProcesses().Result;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Checks if the logged in User is in a given MakandraProcess' Edit/Start access list
        /// </summary>
        /// <param name="makandraProcess">The MakandraProcess that is attempted to be edited/started</param>
        /// <author>Felix Nebel</author>
        public async Task<bool> CheckPermissions(MakandraProcess makandraProcess)
        {
            User user = await UserContainer.GetLoggedInUser(HttpContext);
            List<Role> userRoles = (await UserRolesContainer.GetRolesFromUser(user)).ToList();
            List<Role> processRoles = await MakandraProcessRoleContainer.GetRolesFromProcess(makandraProcess);
            bool isAllowed = false;
            foreach (Role r in processRoles)
            {
                isAllowed = userRoles.Contains(r);
                if (isAllowed)
                    break;
            }

            return isAllowed;
        }
    }
}