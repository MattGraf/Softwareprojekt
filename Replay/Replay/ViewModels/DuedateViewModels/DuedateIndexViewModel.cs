using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Replay.Models;

namespace Replay.ViewModels.DuedateViewModels
{
    /// <summary>
    /// ViewModel for overview of the <see cref="Duedate"/>s
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class DuedateIndexViewModel
    {
        public List<DuedateViewModel> DuedateViewModels {get; set;} = new List<DuedateViewModel>();
        public bool DeleteFailed {get; set;} = false;
        public List<string> TaskTemplatsWithDuedate {get; set;} = new List<string>();
        public string? DeleteFailedName {get; set;}

        public bool NotFound {get; set;} = false;

        public int DuedateIdToDelete {get; set;}
        public int DuedateIdToEdit {get; set;}

        public int SortId {get; set;}

        public int NewSortId {get; set;}

        /// <summary>
        /// Saves all saved <see cref="Duedate"/>s from the database in a own list by creation
        /// </summary>
        /// <author>Matthias Grafberger</author>
        public DuedateIndexViewModel() {
            SortId = 1;
        }

        /// <summary>
        /// Saves all saved <see cref="Duedate"/>s from the database
        /// </summary>
        /// <author>Matthias Grafberger</author>
        public void GenerateDuedateViewModels(DuedateContainer duedateContainer) {
            DuedateViewModels = new List<DuedateViewModel>();

            List<Duedate> duedates = duedateContainer.GetDuedates().Result;

            foreach (Duedate duedate in duedates) {
                DuedateViewModels.Add(new DuedateViewModel {
                    Name = duedate.Name,
                    Days = duedate.Days,
                    ID = duedate.ID
                });
            }

            SortTaskTemplateViewModelList();
        }

        /// <summary>
        /// Sort the saved List
        /// </summary>
        /// <author>Matthias Grafberger</author>
        public void SortTaskTemplateViewModelList() {
            if (SortId == 1) {
                DuedateViewModels = DuedateViewModels
                    .OrderBy(t => t.Name)
                    .ThenBy(t => t.Days)
                    .ToList();
            } else {
                DuedateViewModels = DuedateViewModels
                    .OrderBy(t => t.Days)
                    .ThenBy(t => t.Name)
                    .ToList();
            }
        }
    }
}