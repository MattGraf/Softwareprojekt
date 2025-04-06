using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Replay.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Duedates",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Days = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duedates", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    EstablishingContractTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferencePersonId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResponsiblePersonId = table.Column<int>(type: "INTEGER", nullable: false),
                    basedProcessId = table.Column<int>(type: "INTEGER", nullable: false),
                    Archived = table.Column<bool>(type: "INTEGER", nullable: false),
                    completedTasks = table.Column<int>(type: "INTEGER", nullable: false),
                    openTasks = table.Column<int>(type: "INTEGER", nullable: false),
                    inprogressTasks = table.Column<int>(type: "INTEGER", nullable: false),
                    progressbar = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProcedureId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplates",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Instruction = table.Column<string>(type: "TEXT", nullable: true),
                    DuedateID = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultResponsible = table.Column<string>(type: "TEXT", nullable: false),
                    Archived = table.Column<bool>(type: "INTEGER", nullable: false),
                    MakandraProcessId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TaskTemplates_Duedates_DuedateID",
                        column: x => x.DuedateID,
                        principalTable: "Duedates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTemplates_Processes_MakandraProcessId",
                        column: x => x.MakandraProcessId,
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MakandraProcessRoles",
                columns: table => new
                {
                    MakandraProcessId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MakandraProcessRoles", x => new { x.MakandraProcessId, x.RoleID });
                    table.ForeignKey(
                        name: "FK_MakandraProcessRoles_Processes_MakandraProcessId",
                        column: x => x.MakandraProcessId,
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MakandraProcessRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    PermissionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Instruction = table.Column<string>(type: "TEXT", nullable: true),
                    ResponsibleRoleId = table.Column<int>(type: "INTEGER", nullable: true),
                    TargetDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    StateId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcedureId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssigneeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Archived = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Roles_ResponsibleRoleId",
                        column: x => x.ResponsibleRoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureDepartments",
                columns: table => new
                {
                    ProcedureID = table.Column<int>(type: "INTEGER", nullable: false),
                    DepartmentID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureDepartments", x => new { x.ProcedureID, x.DepartmentID });
                    table.ForeignKey(
                        name: "FK_ProcedureDepartments_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcedureDepartments_Procedures_ProcedureID",
                        column: x => x.ProcedureID,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplateContractTypes",
                columns: table => new
                {
                    TaskTemplateID = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplateContractTypes", x => new { x.TaskTemplateID, x.ContractTypeID });
                    table.ForeignKey(
                        name: "FK_TaskTemplateContractTypes_ContractTypes_ContractTypeID",
                        column: x => x.ContractTypeID,
                        principalTable: "ContractTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTemplateContractTypes_TaskTemplates_TaskTemplateID",
                        column: x => x.TaskTemplateID,
                        principalTable: "TaskTemplates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplateDepartments",
                columns: table => new
                {
                    TaskTemplateID = table.Column<int>(type: "INTEGER", nullable: false),
                    DepartmentID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplateDepartments", x => new { x.TaskTemplateID, x.DepartmentID });
                    table.ForeignKey(
                        name: "FK_TaskTemplateDepartments_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTemplateDepartments_TaskTemplates_TaskTemplateID",
                        column: x => x.TaskTemplateID,
                        principalTable: "TaskTemplates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplateRoles",
                columns: table => new
                {
                    TaskTemplateID = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplateRoles", x => new { x.TaskTemplateID, x.RoleID });
                    table.ForeignKey(
                        name: "FK_TaskTemplateRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskTemplateRoles_TaskTemplates_TaskTemplateID",
                        column: x => x.TaskTemplateID,
                        principalTable: "TaskTemplates",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskRoles",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskRoles", x => new { x.TaskId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TaskRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskRoles_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractTypes_Name",
                table: "ContractTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ProcedureId",
                table: "Departments",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Duedates_Name",
                table: "Duedates",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MakandraProcessRoles_RoleID",
                table: "MakandraProcessRoles",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name",
                table: "Permissions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureDepartments_DepartmentID",
                table: "ProcedureDepartments",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_Name",
                table: "States",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskRoles_RoleId",
                table: "TaskRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssigneeId",
                table: "Tasks",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProcedureId",
                table: "Tasks",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ResponsibleRoleId",
                table: "Tasks",
                column: "ResponsibleRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_StateId",
                table: "Tasks",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplateContractTypes_ContractTypeID",
                table: "TaskTemplateContractTypes",
                column: "ContractTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplateDepartments_DepartmentID",
                table: "TaskTemplateDepartments",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplateRoles_RoleID",
                table: "TaskTemplateRoles",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_DuedateID",
                table: "TaskTemplates",
                column: "DuedateID");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_MakandraProcessId",
                table: "TaskTemplates",
                column: "MakandraProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MakandraProcessRoles");

            migrationBuilder.DropTable(
                name: "ProcedureDepartments");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "TaskRoles");

            migrationBuilder.DropTable(
                name: "TaskTemplateContractTypes");

            migrationBuilder.DropTable(
                name: "TaskTemplateDepartments");

            migrationBuilder.DropTable(
                name: "TaskTemplateRoles");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "ContractTypes");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "TaskTemplates");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "Duedates");

            migrationBuilder.DropTable(
                name: "Processes");
        }
    }
}
