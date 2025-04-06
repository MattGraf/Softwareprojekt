using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Replay.Authorization;
using Replay.Container.Account;
using Replay.Container.Account.MTM;
using Replay.Models;
using Replay.SeedData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Replay.Models.MTM;
using Replay.Models.Account;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MakandraContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MakandraContext")));

AddContainers();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();


// Adds Session to save filters. See MakandraTaskController. Session time set to 30 minutes.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();
ApplyMigrations(app);

/// Method to seed data for first startup
/// <author>Thomas Dworschak, Matthias Grafberger</author>
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedPermissions.InitializeRoles(services);
    SeedRoles.InitializeRoles(services);

    MakandraProcessContainer makandraProcessContainer = services.GetRequiredService<MakandraProcessContainer>();

    // Call TaskContainer for Task Seeds
    var taskContainer = services.GetRequiredService<MakandraTaskContainer>();

    SeedMakandraProcesses.InitializeMakandraProcesses(services);
    SeedDepartments.Initialize(services);
    SeedDuedates.Initialize(services);
    SeedStates.InitializeStates(services);
    SeedContractTypes.InitializeContractTypes(services);
    SeedUsers.InitializeUser(services);
    SeedProcedures.InitializeProcedures(services);
    SeedTaskTemplates.InitializeTaskTemplates(services);
    // Seed Tasks
    if (!taskContainer.GetMakandraTasks().Result.Any())
    {
        await taskContainer.ImportMakandraTasks("SeedData/SeedTasks1.json");
        await taskContainer.ImportMakandraTasks("SeedData/SeedTasks2.json");
        await taskContainer.ImportMakandraTasks("SeedData/SeedTasks3.json");
    }

}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


app.Run();



/// Method to keep the adding of Containers in a centralised location.
/// <author>Felix Nebel</author>
void AddContainers()
{
    builder.Services.AddScoped<UserContainer>();
    builder.Services.AddScoped<RoleContainer>();
    builder.Services.AddScoped<UserRolesContainer>();
    builder.Services.AddScoped<PermissionContainer>();
    builder.Services.AddScoped<RolePermissionContainer>();
    builder.Services.AddScoped<ContractTypesContainer>();
    builder.Services.AddScoped<MakandraTaskStateContainer>();
    builder.Services.AddScoped<ProcedureContainer>();
    builder.Services.AddScoped<MakandraTaskContainer>();
    builder.Services.AddScoped<MakandraTaskRoleContainer>();
    builder.Services.AddScoped<DuedateContainer>();
    builder.Services.AddScoped<DepartmentContainer>();
    builder.Services.AddScoped<TaskTemplateContainer>();
    builder.Services.AddScoped<TaskTemplateContractTypeContainer>();
    builder.Services.AddScoped<TaskTemplateDepartmentContainer>();
    builder.Services.AddScoped<TaskTemplateRoleContainer>();
    builder.Services.AddScoped<MakandraProcessContainer>();
    builder.Services.AddScoped<MakandraProcessRoleContainer>();
    builder.Services.AddScoped<ProcedureDepartmentContainer>();
}

void ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MakandraContext>();
        dbContext.Database.Migrate();
    }
}

