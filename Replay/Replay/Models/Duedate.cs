using System.ComponentModel.DataAnnotations.Schema;

namespace Replay.Models
{

    /// <summary>
    /// The Duedate Class save for a specific Duedate the relative Day-Difference to the Deadline of the Procedure
    /// </summary>
    /// <author>Matthias Grafberger</author>
    public class Duedate
    {
        [Key]
        public int ID {get; set;}
        public string Name {set; get;}
        public int Days {set; get;}

        [NotMapped]
        public virtual List<TaskTemplate> TaskTemplates {get; set;} = new List<TaskTemplate>();


        /// <summary>
        /// Creates a <see cref="Duedate"/> and saves it in the database
        /// </summary>
        /// <param name="duedateContainer">Container for the connection to the database</param>
        /// <param name="name">Name of the new <see cref="Duedate"/></param>
        /// <param name="days">Relative days to the deadline of the new <see cref="Duedate"/></param>
        /// <returns>If the days could transformed in a number</returns>
        /// <author>Matthias Grafberger</author>
        public static bool CreateDuedate(DuedateContainer duedateContainer, string name, string days) {
            int daysInt;

            if (!int.TryParse(days, out daysInt)) return false;

            Duedate duedate = new Duedate {
                Name = name,
                Days = daysInt
            };
            
            duedateContainer.AddDuedate(duedate);

            return true;
        }

        /// <summary>
        /// Edits a <see cref="Duedate"/> and saves it in the database
        /// </summary>
        /// <param name="duedateContainer">Container for the connection to the database</param>
        /// <param name="id">Id of the <see cref="Duedate"/> which is wanted to edited</param>
        /// <param name="name">New Name of the <see cref="Duedate"/></param>
        /// <param name="days">New Relative days to the deadline of the <see cref="Duedate"/></param>
        /// <returns>If the days could transformed in a number</returns>
        /// <author>Matthias Grafberger</author>
        public static bool EditDuedate(DuedateContainer duedateContainer, int id, string name, string days) {
            int daysInt;

            if (!int.TryParse(days, out daysInt)) return false;

            Duedate duedate = new Duedate {
                ID = id,
                Name = name,
                Days = daysInt
            };

            duedateContainer.UpdateDuedate(duedate);

            return true;
        }

        /// <summary>
        /// Checks if a Duedate is valid to save it in the database
        /// </summary>
        /// <returns>0 if it's valid, a error value else</returns>
        /// <author>Matthias Grafberger</author>
        public int IsValid() {
            if (Name is null) return 1;
            return 0;
        }
    }
}