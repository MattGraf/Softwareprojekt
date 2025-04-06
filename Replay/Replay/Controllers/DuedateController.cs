using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Replay.ViewModels.DuedateViewModels;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;
using Replay.Models;

using Replay.Authorization;

namespace Replay.Controllers
{

    /// <summary>
    /// Controller to change the views for the <see cref="Duedate"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class DuedateController : Controller
    {


        private DuedateContainer _duedateContainer;
        private TaskTemplateContainer _taskTemplateContainer;

        /// <summary>
        /// Creates a DuedateController with the needed Containers
        /// </summary>
        /// <param name="duedateContainer">Container for the connection to the database</param>
        /// <param name="taskTemplateContainer">Container for the connection to the database</param>
        /// <author>Matthias Grafberger</author>
        public DuedateController(
            DuedateContainer duedateContainer, TaskTemplateContainer taskTemplateContainer)
        {
            _duedateContainer = duedateContainer;
            _taskTemplateContainer = taskTemplateContainer;
        }

        /// <summary>
        /// Change the View to overview of the <see cref="Duedate"/>s
        /// </summary>
        /// <returns>View to overview of the <see cref="Duedate"/>s</returns>
        /// <author>Matthias Grafberger</author>
        [PermissionChecker("")]
        public IActionResult Index() {
            DuedateIndexViewModel duedateIndexViewModel = new DuedateIndexViewModel();
            duedateIndexViewModel.GenerateDuedateViewModels(_duedateContainer);
            return View(duedateIndexViewModel);
        }

        /// <summary>
        /// Deletes a <see cref="Duedate"> if no <see cref="TaskTemplate"/> exists with this one
        /// </summary>
        /// <param name="duedateIndexViewModel">Information about the overview of the <see cref="Duedate"/>s</param>
        /// <param name="ID">ID of the <see cref="Duedate"/> which is wanted to deleted</param>
        /// <returns>The new overview with errors eventually</returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult Index(DuedateIndexViewModel duedateIndexViewModel) {

            if (ModelState.IsValid) {

                if (duedateIndexViewModel.NewSortId > 0) {
                    duedateIndexViewModel.SortId = duedateIndexViewModel.NewSortId;
                    duedateIndexViewModel.GenerateDuedateViewModels(_duedateContainer);
                    return View(duedateIndexViewModel);
                }
                
                Duedate duedate = _duedateContainer.GetDuedateFromId(duedateIndexViewModel.DuedateIdToDelete).Result;

                if (duedate is null) {
                    duedateIndexViewModel.NotFound = true;
                    return View(duedateIndexViewModel);
                }

                List<TaskTemplate> taskTemplates = _taskTemplateContainer.GetAllTaskTemplates().Result;

                foreach (TaskTemplate taskTemplate in taskTemplates) {
                    if (taskTemplate.DuedateID == duedateIndexViewModel.DuedateIdToDelete) {
                        duedateIndexViewModel.DeleteFailed = true;
                        duedateIndexViewModel.TaskTemplatsWithDuedate.Add(taskTemplate.Name);
                    }
                }

                if(duedateIndexViewModel.DeleteFailed) {
                    duedateIndexViewModel.DeleteFailedName = duedate.Name;
                    duedateIndexViewModel.GenerateDuedateViewModels(_duedateContainer);
                    return View(duedateIndexViewModel);
                }

                _duedateContainer.DeleteDuedate(duedate);

                duedateIndexViewModel.GenerateDuedateViewModels(_duedateContainer);
                return View(duedateIndexViewModel);
            }

            return View(duedateIndexViewModel);
        }


        /// <summary>
        /// Change the View to create-view of the <see cref="Duedate">s
        /// </summary>
        /// <returns>View to create-view of the <see cref="Duedate">s</returns>
        /// <author>Matthias Grafberger</author>
        [PermissionChecker("")]
        public IActionResult Create() {
            DuedateCreateViewModel duedateCreateViewModel = new DuedateCreateViewModel();
            return View(duedateCreateViewModel);
        }

        /// <summary>
        /// Reacts to a creation-request of a <see cref="Duedate"/>
        /// Creates a <see cref="Duedate"/> with the given attributes from the view
        /// 
        /// Redirect to overview when creation is successful, else the view stays
        /// </summary>
        /// <param name="duedateCreateViewModel">Manipulated Model with the wanted information about the <see cref="Duedate"></param>
        /// <returns>Next view</returns>
        /// <author>Matthias Grafberger</author>  
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult Create(DuedateCreateViewModel duedateCreateViewModel) {
            duedateCreateViewModel.Exists = false;
            duedateCreateViewModel.ParseError = false;
            
            if (ModelState.IsValid) {
                
                if (_duedateContainer.DuedateNameExists(duedateCreateViewModel.Name).Result) {
                    duedateCreateViewModel.Exists = true;
                    return View(duedateCreateViewModel);
                }


                bool check = Duedate.CreateDuedate(_duedateContainer, duedateCreateViewModel.Name, duedateCreateViewModel.Days);

                if(!check) {
                    duedateCreateViewModel.ParseError = true;
                    return View(duedateCreateViewModel);
                }

                return RedirectToAction("Index");
        
            }

            

            return View(duedateCreateViewModel);
        }

        /// <summary>
        /// Sends the Id of the <see cref="Duedate"/> which is to be edited to the edit function without relieving it in the url
        /// </summary>
        /// <param name="duedateIndexViewModel">View Model with the id of the <see cref="Duedate"/> which is to be edited</param>
        /// <returns>Redirection to the edit function</returns>
        /// <author>Matthias Grafberger</author>
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult EditBeginn([Bind("DuedateIdToEdit")] DuedateIndexViewModel duedateIndexViewModel) {
            TempData["DuedateIdToEdit"] = duedateIndexViewModel.DuedateIdToEdit;
            return RedirectToAction("Edit");
        }

        /// <summary>
        /// Change the View to edit-view of the <see cref="Duedate">s
        /// </summary>
        /// <returns>View to edit-view of the <see cref="Duedate">s</returns>
        /// <author>Matthias Grafberger</author>
        [PermissionChecker("")]
        public IActionResult Edit() {
            int id = (int) TempData["DuedateIdToEdit"];

            Duedate duedate = _duedateContainer.GetDuedateFromId(id).Result;

            DuedateEditViewModel duedateEditViewModel = new DuedateEditViewModel {
                ID = id,
                Name = duedate.Name,
                Days = duedate.Days.ToString(),
                Exists = false,
                ParseError = false
            };

            return View(duedateEditViewModel);
        }

        /// <summary>
        /// Reacts to a edit-request of a <see cref="Duedate"/>
        /// Edit a <see cref="Duedate"/> with the given attributes from the view
        /// 
        /// Redirect to overview when creation is successful, else the view stays
        /// </summary>
        /// <param name="duedateEditViewModel">Manipulated Model with the wanted information about the <see cref="Duedate"></param>
        /// <returns>Next view</returns>
        /// <author>Matthias Grafberger</author> 
        [HttpPost]
        [PermissionChecker("")]
        public IActionResult Edit(DuedateEditViewModel duedateEditViewModel) {
            duedateEditViewModel.Exists = false;
            duedateEditViewModel.ParseError = false;
            
            if (ModelState.IsValid) {
                
                if (_duedateContainer.DuedateNameExistsWithoutID(duedateEditViewModel.Name, duedateEditViewModel.ID).Result) {
                    duedateEditViewModel.Exists = true;
                    return View(duedateEditViewModel);
                }


                bool check = Duedate.EditDuedate(_duedateContainer, duedateEditViewModel.ID, duedateEditViewModel.Name, duedateEditViewModel.Days);

                if(!check) {
                    duedateEditViewModel.ParseError = true;
                    return View(duedateEditViewModel);
                }

                return RedirectToAction("Index");
        
            }

            return View(duedateEditViewModel);
        }
    }
}