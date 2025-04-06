using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenQA.Selenium.DevTools.V123.Browser;
using Replay.Models;
using Replay.Models.MTM;
using Replay.Models.Account;
using Replay.Models.Account.MTM;

namespace Replay.Data
{
    /// <summary>
    /// Managing the saved database
    /// </summary>
    /// <author>Matthias Grafberger, Felix Nebel</author>
    public class MakandraContext : DbContext
    {
        public virtual DbSet<TaskTemplate> TaskTemplates {get; set;}
        public virtual DbSet<Duedate> Duedates {get; set;}
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Procedure> Procedures {get; set;}
        public virtual DbSet<MakandraProcess> Processes {get; set;}
        public virtual DbSet<MakandraTask> Tasks {get; set;}
        public virtual DbSet<MakandraTaskState> States { get; set; }
        public virtual DbSet<MakandraTaskRole> TaskRoles { get; set; }
        public virtual DbSet<User> Users {get; set;}
        public virtual DbSet<Role> Roles {get; set;}
        public virtual DbSet<UserRole> UserRoles { get; set; } = default!;
        public virtual DbSet<RolePermission> RolePermissions { get; set; } = default!;
        public virtual DbSet<TaskTemplateContractType> TaskTemplateContractTypes {get; set;} = default!;
        public virtual DbSet<TaskTemplateRole> TaskTemplateRoles {get; set;} = default!;
        public  virtual DbSet<MakandraProcessRole> MakandraProcessRoles {get; set;} = default!;
        public virtual DbSet<Permission> Permissions { get; set; } = default!;
        public virtual DbSet<ContractType> ContractTypes {get; set;}
        public virtual DbSet<ProcedureDepartment> ProcedureDepartments {get; set;}
        public virtual DbSet<TaskTemplateDepartment> TaskTemplateDepartments {get; set;}





        private string DbPath;

        /// <summary>
        /// Create new context with database which is located in the Persistence-Folder with possible options
        /// </summary>
        /// <param name="options">Options of the database which can be added</param>
        /// <author>Matthias Grafberger</author>
        public MakandraContext(DbContextOptions<MakandraContext> options) : base(options) {
            DbPath = Path.Combine(Environment.CurrentDirectory, "Persistence", "makandra.db");
        }

        /// <summary>
        /// Create new context with database which is located in the Persistence-Folder without possible options
        /// </summary>
        /// <param name="options">Options of the database which can be added</param>
        /// <author>Matthias Grafberger</author>
        public MakandraContext() {
            DbPath = Path.Combine(Environment.CurrentDirectory, "Persistence", "makandra.db");
        }

        /// <summary>
        /// Change the options of the database
        /// </summary>
        /// <param name="options">New options</param>
        /// <author>Matthias Grafberger</author>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
            options.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             modelBuilder.Entity<Procedure>()
            .Ignore(p => p.EstablishingContractType);
            // Many-to-Many Table Creation for User-Role
            registerUserRoleRelation(modelBuilder);

            // Many-to-Many Table Creation for Role-Permission
            registerRolePermissionRelation(modelBuilder);

            // Many-to-Many Table Creation for MakandraTask - Role
            registerMakandraTaskRoleRelation(modelBuilder);

            // Setting unique constraint that don't work with code-first in .net 6.0 can be changed to code-first after 6.1
            registerUniqueConstraints(modelBuilder);

            // Many-to-Many Table Creation for TaskTemplate-ContractType
            registerTaskTemplateContractTypeRelation(modelBuilder);

            // Many-to-Many Table Creation for Procedure-Department
            registerProcedureDepartmentRelation(modelBuilder);

            // Many-to-Many Table Creation for TaskTemplate-Role
            registerTaskTemplateRoleRelation(modelBuilder);
            
            // Many-to-Many Table Creation for MakandraProcess-Role
            registerMakandraProcessRoleRelation(modelBuilder);

            // 1:N Table Creation for Duedate-TaskTemplate
            registerTaskTemplateDuedateRelation(modelBuilder);

            // Many-to-Many Table Creation for TaskTemplate-Department
            registerTaskTemplateDepartmentRelation(modelBuilder);

            // One-to-Many Table Creation for Procedure-MakandraTask
            registerProcedureMakandraTaskRelation(modelBuilder);

        }

        [Obsolete("This is a temporary solution, can be done with code-first as soon as .net 6.1.")]
        private void registerUniqueConstraints(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();

            modelBuilder.Entity<ContractType>().HasIndex(ContractType => ContractType.Name).IsUnique();
            modelBuilder.Entity<Department>().HasIndex(Department => Department.Name).IsUnique();
            modelBuilder.Entity<Duedate>().HasIndex(Duedate => Duedate.Name).IsUnique();
            modelBuilder.Entity<MakandraTaskState>().HasIndex(MakandraTaskState => MakandraTaskState.Name).IsUnique();

            //modelBuilder.Entity<TaskTemplate>().HasIndex(TaskTemplate => TaskTemplate.InstructionName).IsUnique();
            //modelBuilder.Entity<MakandraTask>().HasIndex(MakandraTask => MakandraTask.NotesName).IsUnique();

        }

        private void registerUserRoleRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
            .HasKey(s => new { s.RoleId, s.UserId });

            modelBuilder.Entity<UserRole>()
            .HasOne(su => su.Role)
            .WithMany(s => s.UserRoles)
            .HasForeignKey(su => su.RoleId);

            modelBuilder.Entity<UserRole>()
            .HasOne(su => su.User)
            .WithMany(s => s.UserRoles)
            .HasForeignKey(su => su.UserId);
        }

        private void registerRolePermissionRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RolePermission>()
            .HasKey(s => new { s.RoleId, s.PermissionId });

            modelBuilder.Entity<RolePermission>()
            .HasOne(su => su.Role)
            .WithMany(s => s.RolePermissions)
            .HasForeignKey(su => su.RoleId);

            modelBuilder.Entity<RolePermission>()
            .HasOne(su => su.Permission)
            .WithMany(s => s.RolePermission)
            .HasForeignKey(su => su.PermissionId);
        }

        /// <summary>
        /// Register the table information of the many-to-many-table between <see cref="MakandraTask"/> and <see cref="Role"/>
        /// </summary>
        /// <param name="modelBuilder">Information about the whole database</param>
        /// <author>Thomas Dworschak, Matthias Grafberger</author>
        private void registerMakandraTaskRoleRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MakandraTaskRole>()
                .HasKey(s => new { s.TaskId, s.RoleId});
            
            modelBuilder.Entity<MakandraTaskRole>()
                .HasOne(su => su.Task)
                .WithMany(s => s.EditAccess)
                .HasForeignKey(su => su.TaskId);

            modelBuilder.Entity<MakandraTaskRole>()
                .HasOne(su => su.Role)
                .WithMany(s => s.TaskRoles)
                .HasForeignKey(su => su.RoleId);
        }

        /// <summary>
        /// Register the table informations of the many-to-many-table between <see cref="TaskTemplate"/> and <see cref="ContractType">
        /// </summary>
        /// <param name="modelBuilder">Information about the whole database</param>
        /// <author>Matthias Grafberger</author>
       private void registerTaskTemplateContractTypeRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskTemplateContractType>()
            .HasKey(s => new { s.TaskTemplateID, s.ContractTypeID });

            modelBuilder.Entity<TaskTemplateContractType>()
            .HasOne(su => su.TaskTemplate)
            .WithMany(s => s.TaskTemplateContractTypes)
            .HasForeignKey(su => su.TaskTemplateID);

            modelBuilder.Entity<TaskTemplateContractType>()
            .HasOne(su => su.ContractType)
            .WithMany(s => s.TaskTemplateContractTypes)
            .HasForeignKey(su => su.ContractTypeID);
        }


        /// <summary>
        /// Register the table informations of the many-to-many-table between <see cref="Procedure"/> and <see cref="Department">
        /// </summary>
        /// <param name="modelBuilder">Information about the whole database</param>
        /// <author>Matthias Grafberger</author>
        private void registerProcedureDepartmentRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcedureDepartment>()
            .HasKey(s => new {s.ProcedureID, s.DepartmentID});

            modelBuilder.Entity<ProcedureDepartment>()
            .HasOne(su => su.Procedure)
            .WithMany(s => s.ProcedureDepartments)
            .HasForeignKey(su => su.ProcedureID);

            modelBuilder.Entity<ProcedureDepartment>()
            .HasOne(su => su.Department)
            .WithMany(s => s.ProcedureDepartments)
            .HasForeignKey(su => su.DepartmentID);
        }

        /// <summary>
        /// Register the table informations of the many-to-many-table between <see cref="TaskTemplate"/> and <see cref="Role">
        /// </summary>
        /// <param name="modelBuilder">Information about the whole database</param>
        /// <author>Matthias Grafberger</author>
        private void registerTaskTemplateRoleRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskTemplateRole>()
            .HasKey(s => new {s.TaskTemplateID, s.RoleID});

            modelBuilder.Entity<TaskTemplateRole>()
            .HasOne(su => su.TaskTemplate)
            .WithMany(s => s.EditAccess)
            .HasForeignKey(su => su.TaskTemplateID);

            modelBuilder.Entity<TaskTemplateRole>()
            .HasOne(su => su.Role)
            .WithMany(s => s.TaskTemplateRoles)
            .HasForeignKey(su => su.RoleID);
        }

        /// <summary>
        /// Register the table informations of the many-to-many-table between <see cref="MakandraProcess"/> and <see cref="Role">
        /// </summary>
        /// <param name="modelBuilder">Information about the whole database</param>
        /// <author>Matthias Grafberger</author>
        private void registerMakandraProcessRoleRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MakandraProcessRole>()
            .HasKey(s => new {s.MakandraProcessId, s.RoleID});

            modelBuilder.Entity<MakandraProcessRole>()
            .HasOne(su => su.MakandraProcess)
            .WithMany(s => s.MakandraProcessRoles)
            .HasForeignKey(su => su.MakandraProcessId);

            modelBuilder.Entity<MakandraProcessRole>()
            .HasOne(su => su.Role)
            .WithMany(s => s.MakandraProcessRoles)
            .HasForeignKey(su => su.RoleID);
        }
        /// <summary>
        /// Register reference key from <see cref="TaskTemplate"/> to <see cref="Duedate"/>
        /// </summary>
        /// <param name="modelBuilder">Information about the whole database</param>
        /// <author>Matthias Grafberger</author>
        private void registerTaskTemplateDuedateRelation(ModelBuilder modelBuilder) {
            modelBuilder.Entity<TaskTemplate>()
            .HasOne(s => s.Duedate)
            .WithMany(s => s.TaskTemplates)
            .HasForeignKey(s => s.DuedateID);
        }


        /// <summary>
        /// Register the table informations of the many-to-many-table between <see cref="TaskTemplate"/> and <see cref="Department">
        /// </summary>
        /// <param name="modelBuilder">Information about the whole database</param>
        /// <author>Matthias Grafberger</author>
        private void registerTaskTemplateDepartmentRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskTemplateDepartment>()
            .HasKey(s => new {s.TaskTemplateID, s.DepartmentID});

            modelBuilder.Entity<TaskTemplateDepartment>()
            .HasOne(su => su.TaskTemplate)
            .WithMany(s => s.TaskTemplateDepartments)
            .HasForeignKey(su => su.TaskTemplateID);

            modelBuilder.Entity<TaskTemplateDepartment>()
            .HasOne(su => su.Department)
            .WithMany(s => s.TaskTemplateDepartments)
            .HasForeignKey(su => su.DepartmentID);
        }
        /// <summary>
        /// Register the table informations of the one-to-many-table between <see cref="Procedure"/> and <see cref="MakandraTask">
        /// </summary>
        /// <param name="modelBuilder">Information about the whole database</param>
        /// <author>Florian Fendt</author>
        private void registerProcedureMakandraTaskRelation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Procedure>()
            .HasMany(e => e.makandraTasks)
            .WithOne(e => e.Procedure)
            .HasForeignKey(e => e.ProcedureId)
            .IsRequired();
        }

        
    }
}
