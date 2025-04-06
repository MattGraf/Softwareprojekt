using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Replay.Models;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace Replay.Container
{
    /// <summary>
    /// Class to manage databse methods for <see cref="Department"/>
    /// </summary>
    /// <author>Thomas Dworschak</author>
    public class DepartmentContainer
    {
        private readonly MakandraContext _db;
        public DepartmentContainer(MakandraContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This method checks, if an input <see cref="Department"/>
        /// of the same name is already present in the database.
        /// If not, it is added to the database
        /// </summary>
        /// <param name="department"><see cref="Department"/> that is intended to be added to the database</param>
        /// <author>Thomas Dworschak</author>
        public async void AddDepartment(Department department)
        {
            var departmentExists = await _db.Departments
                .FirstOrDefaultAsync<Department>(d => d.Name == department.Name);
            
            if (departmentExists is not null)
            {
                return;
            }

            _db.Add(department);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all possible entries of <see cref="Department"/>
        /// from the database
        /// </summary>
        /// <returns>List of departments</returns>#
        /// <author>Thomas Dworschak</author>
        public async Task<List<Department>>GetDepartments()
        {
            List<Department> departments = await _db.Departments
                .OrderBy(d => d.Id)
                .ToListAsync();

            return departments;
        }

        /// <summary>
        /// Takes an input <see cref="Department"/> and overwrites
        /// the attributes of a department in the database with
        /// identical Id.
        /// If no such department is present in the databse,
        /// the input department is added instead
        /// </summary>
        /// <param name="department"><see cref="Department"/> that is supposed to be added to the database</param>
        /// <author>Thomas Dworschak</author>
        public async void UpdateDepartment(Department department)
        {
            var departmentToUpdate = await _db.Departments
                .FirstOrDefaultAsync<Department>(d => d.Id == department.Id);
            
            if (departmentToUpdate is null)
            {
                AddDepartment(department);
            }
            else
            {
                departmentToUpdate.Name = department.Name;
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a <see cref="Department"/> from the database
        /// based in the department Id
        /// </summary>
        /// <param name="department"><see cref="Department"/> that is supposed to be removed from the database</param>
        /// <author>Thomas Dworschak</author>
        public async void DeleteDepartment(Department department)
        {
            var departmentToDelete = await _db.Departments
                .FirstOrDefaultAsync<Department>(d => d.Id == department.Id);

            if (departmentToDelete is null)
            {
                return;
            }

            _db.Remove(departmentToDelete);

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a <see cref="Department"/> from the database
        /// based on an id.
        /// </summary>
        /// <param name="id">Id of a <see cref="Department"/> that is supposed to be retrieved</param>
        /// <returns><see cref="Department"/> with matching id</returns>
        /// <exception cref="KeyNotFoundException">If no <see cref="Department"/> with corresponding Id has been found</exception>
        /// <author>Thomas Dworschak</author>
        public async Task<Department> GetDepartmentFromId(int id)
        {
            var department = await _db.Departments
                .FirstOrDefaultAsync<Department>(d => d.Id == id);

            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            return department;
        }

        /// <summary>
        /// Imports a Json-string with <see cref="Duedate"/>s in the database
        /// </summary>
        /// <param name="jsonFile">Json-string to  be imported</param>
        /// <author>Matthias Grafberger</author>
        public void Import(string jsonFile) {

            if (jsonFile is null) return;

                
            List<Department> departments = new List<Department>();

            try {
                departments = JsonSerializer.Deserialize<List<Department>>(jsonFile);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                return;
            }
            departments.ForEach(e => {
                int h = e.IsValid();
                if (h == 0)
                {
                    AddDepartment(e);
                } else {
                    Console.WriteLine("Department couldn't added to database, because of a invalid state");
                }
            });
        }
    }
}