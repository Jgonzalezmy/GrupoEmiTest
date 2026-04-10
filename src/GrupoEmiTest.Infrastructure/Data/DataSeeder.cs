using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Domain.Entities;
using GrupoEmiTest.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GrupoEmiTest.Infrastructure.Data;

/// <summary>
/// Handles initial data seeding for the application database.
/// Seeding is skipped when the Employees table already contains records (idempotent).
/// </summary>
public sealed class DataSeeder(GrupoEmiTestDBContext context, IPasswordHasher passwordHasher)
{
    /// <summary>
    /// Applies pending migrations and seeds initial data if the database is empty.
    /// </summary>
    public async Task SeedAsync()
    {
        await context.Database.MigrateAsync();

        if (!await context.User.AnyAsync())
        {
            var users = BuildUsers();
            await context.User.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }

        if (await context.Employee.AnyAsync())
            return;

        var departments = BuildDepartments();
        await context.Set<Department>().AddRangeAsync(departments);
        await context.SaveChangesAsync();

        var projects = BuildProjects();
        await context.Set<Project>().AddRangeAsync(projects);
        await context.SaveChangesAsync();

        var employees = BuildEmployees(departments, projects);
        await context.Employee.AddRangeAsync(employees);
        await context.SaveChangesAsync();
    }

    private List<ApplicationUser> BuildUsers() =>
    [
        ApplicationUser.Create(
            username:     "admin.grupoemi",
            email:        "admin@grupoemi.com",
            passwordHash: passwordHasher.Hash("GrupoEmi2026!"),
            role:         RoleType.Admin).Value,
        ApplicationUser.Create(
            username:     "user.grupoemi",
            email:        "user@grupoemi.com",
            passwordHash: passwordHasher.Hash("GrupoEmi2026!"),
            role:         RoleType.User).Value
    ];

    private static List<Department> BuildDepartments() =>
    [
        Department.Create("Engineering").Value,
        Department.Create("Marketing").Value,
        Department.Create("Human Resources").Value
    ];

    private static List<Project> BuildProjects() =>
    [
        Project.Create("ERP System Modernization",   "Full migration of the legacy ERP to a modern cloud-based architecture."),
        Project.Create("Digital Marketing Platform", "Integrated platform for campaign management and analytics."),
        Project.Create("Talent Management System",   "Internal tool for tracking recruitment, onboarding, and performance reviews.")
    ];

    private static List<Employee> BuildEmployees(List<Department> departments, List<Project> projects)
    {
        var engineering = departments[0];
        var marketing   = departments[1];
        var hr          = departments[2];

        var erp = projects[0];
        var dmp = projects[1];
        var tms = projects[2];

        return
        [
            Seed("Carlos Méndez",      PositionType.DepartmentManager, 8_500m,  engineering,
                [ History(PositionType.RegularEmployee,   new(2020, 3, 1),  new(2022, 6, 30)),
                  History(PositionType.TeamLead,          new(2022, 7, 1),  new(2023, 8, 31)),
                  History(PositionType.DepartmentManager, new(2023, 9, 1)) ],
                [erp, dmp]),

            Seed("Ana García",         PositionType.TeamLead,          6_200m,  marketing,
                [ History(PositionType.RegularEmployee, new(2019, 1, 15), new(2021, 12, 31)),
                  History(PositionType.TeamLead,        new(2022, 1, 1)) ],
                [dmp]),

            Seed("Luis Torres",        PositionType.RegularEmployee,   3_800m,  hr,
                [ History(PositionType.RegularEmployee, new(2023, 4, 10)) ],
                [tms]),

            Seed("María Rodríguez",    PositionType.RegularEmployee,   4_200m,  engineering,
                [ History(PositionType.RegularEmployee, new(2022, 9, 5)) ],
                [erp, tms]),

            Seed("Roberto Sánchez",    PositionType.Director,          12_000m, engineering,
                [ History(PositionType.RegularEmployee,   new(2015, 2, 1),  new(2018, 3, 31)),
                  History(PositionType.TeamLead,          new(2018, 4, 1),  new(2020, 6, 30)),
                  History(PositionType.DepartmentManager, new(2020, 7, 1),  new(2022, 12, 31)),
                  History(PositionType.Director,          new(2023, 1, 1)) ],
                [erp, dmp, tms]),

            Seed("Patricia López",     PositionType.VicePresident,     15_000m, marketing,
                [ History(PositionType.DepartmentManager, new(2016, 5, 1),  new(2019, 8, 31)),
                  History(PositionType.GeneralManager,    new(2019, 9, 1),  new(2022, 6, 30)),
                  History(PositionType.VicePresident,     new(2022, 7, 1)) ],
                [dmp]),

            Seed("Diego Fernández",    PositionType.TeamLead,          5_500m,  engineering,
                [ History(PositionType.RegularEmployee, new(2020, 6, 1),  new(2023, 5, 31)),
                  History(PositionType.TeamLead,        new(2023, 6, 1)) ],
                [erp, tms]),

            Seed("Valentina Martínez", PositionType.RegularEmployee,   3_600m,  marketing,
                [ History(PositionType.RegularEmployee, new(2023, 8, 14)) ],
                [dmp]),

            Seed("Andrés Herrera",     PositionType.DepartmentManager, 7_800m,  hr,
                [ History(PositionType.RegularEmployee,   new(2018, 3, 1),  new(2021, 2, 28)),
                  History(PositionType.TeamLead,          new(2021, 3, 1),  new(2023, 7, 31)),
                  History(PositionType.DepartmentManager, new(2023, 8, 1)) ],
                [tms]),

            Seed("Camila Jiménez",     PositionType.RegularEmployee,   4_000m,  engineering,
                [ History(PositionType.RegularEmployee, new(2022, 11, 7)) ],
                [erp]),

            Seed("Javier Morales",     PositionType.TeamLead,          5_800m,  hr,
                [ History(PositionType.RegularEmployee, new(2020, 1, 20), new(2022, 9, 30)),
                  History(PositionType.TeamLead,        new(2022, 10, 1)) ],
                [tms]),

            Seed("Sofía Castro",       PositionType.RegularEmployee,   3_500m,  marketing,
                [ History(PositionType.RegularEmployee, new(2024, 2, 1)) ],
                [dmp]),

            Seed("Miguel Ángel Reyes", PositionType.GeneralManager,    11_000m, engineering,
                [ History(PositionType.TeamLead,          new(2017, 4, 1),  new(2019, 9, 30)),
                  History(PositionType.DepartmentManager, new(2019, 10, 1), new(2022, 11, 30)),
                  History(PositionType.GeneralManager,    new(2022, 12, 1)) ],
                [erp]),

            Seed("Lucía Vargas",       PositionType.RegularEmployee,   4_100m,  engineering,
                [ History(PositionType.RegularEmployee, new(2023, 3, 15)) ],
                [erp, dmp]),

            Seed("Tomás Guerrero",     PositionType.TeamLead,          5_600m,  marketing,
                [ History(PositionType.RegularEmployee, new(2021, 7, 1),  new(2023, 6, 30)),
                  History(PositionType.TeamLead,        new(2023, 7, 1)) ],
                [dmp]),

            Seed("Isabel Ramírez",     PositionType.RegularEmployee,   3_700m,  hr,
                [ History(PositionType.RegularEmployee, new(2022, 5, 23)) ],
                [tms]),

            Seed("Sebastián Mendoza",  PositionType.DepartmentManager, 8_200m,  marketing,
                [ History(PositionType.RegularEmployee,   new(2017, 9, 1),  new(2020, 4, 30)),
                  History(PositionType.TeamLead,          new(2020, 5, 1),  new(2022, 10, 31)),
                  History(PositionType.DepartmentManager, new(2022, 11, 1)) ],
                [dmp, tms]),

            Seed("Natalia Flores",     PositionType.RegularEmployee,   4_300m,  engineering,
                [ History(PositionType.RegularEmployee, new(2024, 1, 8)) ],
                [erp]),

            Seed("Emilio Ortega",      PositionType.RegularEmployee,   3_900m,  hr,
                [ History(PositionType.RegularEmployee, new(2023, 10, 2)) ],
                [tms])
        ];
    }

    /// <summary>
    /// Builds an <see cref="Employee"/> via the domain factory and attaches its related collections.
    /// EF Core sets the FK values through relationship fixup when SaveChangesAsync is called.
    /// </summary>
    private static Employee Seed(
        string name,
        PositionType position,
        decimal salary,
        Department department,
        PositionHistory[] histories,
        Project[] projects)
    {
        var employee = Employee.Create(name, position, salary, department.Id).Value;
        employee.PositionHistories = histories.ToList();
        employee.EmployeeProjects  = projects.Select(p => new EmployeeProject { Project = p }).ToList();
        return employee;
    }

    /// <summary>
    /// Creates a <see cref="PositionHistory"/> for seeding without a known employee ID.
    /// EF Core resolves the FK through the parent collection during SaveChangesAsync.
    /// </summary>
    private static PositionHistory History(PositionType position, DateTime start, DateTime? end = null)
        => PositionHistory.Create(0, position, start, end).Value;
}
