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
            role:         RoleType.Admin),
        ApplicationUser.Create(
            username:     "user.grupoemi",
            email:        "user@grupoemi.com",
            passwordHash: passwordHasher.Hash("GrupoEmi2026!"),
            role:         RoleType.User)
    ];

    private static List<Department> BuildDepartments() =>
    [
        new() { Name = "Engineering" },
        new() { Name = "Marketing" },
        new() { Name = "Human Resources" }
    ];

    private static List<Project> BuildProjects() =>
    [
        new()
        {
            Name        = "ERP System Modernization",
            Description = "Full migration of the legacy ERP to a modern cloud-based architecture."
        },
        new()
        {
            Name        = "Digital Marketing Platform",
            Description = "Integrated platform for campaign management and analytics."
        },
        new()
        {
            Name        = "Talent Management System",
            Description = "Internal tool for tracking recruitment, onboarding, and performance reviews."
        }
    ];

    private static List<Employee> BuildEmployees(List<Department> departments, List<Project> projects)
    {
        var engineering = departments[0];
        var marketing = departments[1];
        var hr = departments[2];

        var erp = projects[0];
        var dmp = projects[1];
        var tms = projects[2];

        return
        [
            // ── Existing employees ─────────────────────────────────────────────
            new()
            {
                Name            = "Carlos Méndez",
                CurrentPosition = PositionType.DepartmentManager,
                Salary          = 8_500m,
                Department      = engineering,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee,   StartDate = new DateTime(2020, 3, 1),  EndDate = new DateTime(2022, 6, 30) },
                    new() { Position = PositionType.TeamLead,          StartDate = new DateTime(2022, 7, 1),  EndDate = new DateTime(2023, 8, 31) },
                    new() { Position = PositionType.DepartmentManager, StartDate = new DateTime(2023, 9, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = erp },
                    new() { Project = dmp }
                ]
            },
            new()
            {
                Name            = "Ana García",
                CurrentPosition = PositionType.TeamLead,
                Salary          = 6_200m,
                Department      = marketing,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2019, 1, 15), EndDate = new DateTime(2021, 12, 31) },
                    new() { Position = PositionType.TeamLead,        StartDate = new DateTime(2022, 1, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = dmp }
                ]
            },
            new()
            {
                Name            = "Luis Torres",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 3_800m,
                Department      = hr,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2023, 4, 10) }
                ],
                EmployeeProjects =
                [
                    new() { Project = tms }
                ]
            },
            new()
            {
                Name            = "María Rodríguez",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 4_200m,
                Department      = engineering,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2022, 9, 5) }
                ],
                EmployeeProjects =
                [
                    new() { Project = erp },
                    new() { Project = tms }
                ]
            },

            // ── New employees ──────────────────────────────────────────────────
            new()
            {
                Name            = "Roberto Sánchez",
                CurrentPosition = PositionType.Director,
                Salary          = 12_000m,
                Department      = engineering,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee,   StartDate = new DateTime(2015, 2, 1),  EndDate = new DateTime(2018, 3, 31) },
                    new() { Position = PositionType.TeamLead,          StartDate = new DateTime(2018, 4, 1),  EndDate = new DateTime(2020, 6, 30) },
                    new() { Position = PositionType.DepartmentManager, StartDate = new DateTime(2020, 7, 1),  EndDate = new DateTime(2022, 12, 31) },
                    new() { Position = PositionType.Director,          StartDate = new DateTime(2023, 1, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = erp },
                    new() { Project = dmp },
                    new() { Project = tms }
                ]
            },
            new()
            {
                Name            = "Patricia López",
                CurrentPosition = PositionType.VicePresident,
                Salary          = 15_000m,
                Department      = marketing,
                PositionHistories =
                [
                    new() { Position = PositionType.DepartmentManager, StartDate = new DateTime(2016, 5, 1),  EndDate = new DateTime(2019, 8, 31) },
                    new() { Position = PositionType.GeneralManager,    StartDate = new DateTime(2019, 9, 1),  EndDate = new DateTime(2022, 6, 30) },
                    new() { Position = PositionType.VicePresident,     StartDate = new DateTime(2022, 7, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = dmp }
                ]
            },
            new()
            {
                Name            = "Diego Fernández",
                CurrentPosition = PositionType.TeamLead,
                Salary          = 5_500m,
                Department      = engineering,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2020, 6, 1), EndDate = new DateTime(2023, 5, 31) },
                    new() { Position = PositionType.TeamLead,        StartDate = new DateTime(2023, 6, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = erp },
                    new() { Project = tms }
                ]
            },
            new()
            {
                Name            = "Valentina Martínez",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 3_600m,
                Department      = marketing,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2023, 8, 14) }
                ],
                EmployeeProjects =
                [
                    new() { Project = dmp }
                ]
            },
            new()
            {
                Name            = "Andrés Herrera",
                CurrentPosition = PositionType.DepartmentManager,
                Salary          = 7_800m,
                Department      = hr,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee,   StartDate = new DateTime(2018, 3, 1),  EndDate = new DateTime(2021, 2, 28) },
                    new() { Position = PositionType.TeamLead,          StartDate = new DateTime(2021, 3, 1),  EndDate = new DateTime(2023, 7, 31) },
                    new() { Position = PositionType.DepartmentManager, StartDate = new DateTime(2023, 8, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = tms }
                ]
            },
            new()
            {
                Name            = "Camila Jiménez",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 4_000m,
                Department      = engineering,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2022, 11, 7) }
                ],
                EmployeeProjects =
                [
                    new() { Project = erp }
                ]
            },
            new()
            {
                Name            = "Javier Morales",
                CurrentPosition = PositionType.TeamLead,
                Salary          = 5_800m,
                Department      = hr,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2020, 1, 20), EndDate = new DateTime(2022, 9, 30) },
                    new() { Position = PositionType.TeamLead,        StartDate = new DateTime(2022, 10, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = tms }
                ]
            },
            new()
            {
                Name            = "Sofía Castro",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 3_500m,
                Department      = marketing,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2024, 2, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = dmp }
                ]
            },
            new()
            {
                Name            = "Miguel Ángel Reyes",
                CurrentPosition = PositionType.GeneralManager,
                Salary          = 11_000m,
                Department      = engineering,
                PositionHistories =
                [
                    new() { Position = PositionType.TeamLead,          StartDate = new DateTime(2017, 4, 1),  EndDate = new DateTime(2019, 9, 30) },
                    new() { Position = PositionType.DepartmentManager, StartDate = new DateTime(2019, 10, 1), EndDate = new DateTime(2022, 11, 30) },
                    new() { Position = PositionType.GeneralManager,    StartDate = new DateTime(2022, 12, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = erp }
                ]
            },
            new()
            {
                Name            = "Lucía Vargas",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 4_100m,
                Department      = engineering,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2023, 3, 15) }
                ],
                EmployeeProjects =
                [
                    new() { Project = erp },
                    new() { Project = dmp }
                ]
            },
            new()
            {
                Name            = "Tomás Guerrero",
                CurrentPosition = PositionType.TeamLead,
                Salary          = 5_600m,
                Department      = marketing,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2021, 7, 1), EndDate = new DateTime(2023, 6, 30) },
                    new() { Position = PositionType.TeamLead,        StartDate = new DateTime(2023, 7, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = dmp }
                ]
            },
            new()
            {
                Name            = "Isabel Ramírez",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 3_700m,
                Department      = hr,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2022, 5, 23) }
                ],
                EmployeeProjects =
                [
                    new() { Project = tms }
                ]
            },
            new()
            {
                Name            = "Sebastián Mendoza",
                CurrentPosition = PositionType.DepartmentManager,
                Salary          = 8_200m,
                Department      = marketing,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee,   StartDate = new DateTime(2017, 9, 1),  EndDate = new DateTime(2020, 4, 30) },
                    new() { Position = PositionType.TeamLead,          StartDate = new DateTime(2020, 5, 1),  EndDate = new DateTime(2022, 10, 31) },
                    new() { Position = PositionType.DepartmentManager, StartDate = new DateTime(2022, 11, 1) }
                ],
                EmployeeProjects =
                [
                    new() { Project = dmp },
                    new() { Project = tms }
                ]
            },
            new()
            {
                Name            = "Natalia Flores",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 4_300m,
                Department      = engineering,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2024, 1, 8) }
                ],
                EmployeeProjects =
                [
                    new() { Project = erp }
                ]
            },
            new()
            {
                Name            = "Emilio Ortega",
                CurrentPosition = PositionType.RegularEmployee,
                Salary          = 3_900m,
                Department      = hr,
                PositionHistories =
                [
                    new() { Position = PositionType.RegularEmployee, StartDate = new DateTime(2023, 10, 2) }
                ],
                EmployeeProjects =
                [
                    new() { Project = tms }
                ]
            }
        ];
    }
}
