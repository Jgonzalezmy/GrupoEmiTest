using GrupoEmiTest.Application.DTOs.Response;
using GrupoEmiTest.Domain.Entities;

namespace GrupoEmiTest.Application.Extensions
{
    /// <summary>
    /// Provides extension methods for mapping <see cref="Employee"/> entities
    /// to their corresponding response DTOs.
    /// </summary>
    public static class EmployeeExtension
    {
        /// <summary>
        /// Maps an <see cref="Employee"/> entity to an <see cref="EmployeeResponse"/> DTO,
        /// including department name, yearly bonus calculation, and position history.
        /// </summary>
        /// <param name="employee">The <see cref="Employee"/> entity to map. Must not be <see langword="null"/>.</param>
        /// <returns>An <see cref="EmployeeResponse"/> populated with the entity's data.</returns>
        public static EmployeeResponse ToResponse(this Employee employee)
        {
            return new EmployeeResponse(
                Id: employee.Id,
                Name: employee.Name,
                CurrentPosition: employee.CurrentPosition,
                DepartmentName: employee.Department?.Name ?? string.Empty,
                Salary: employee.Salary,
                YearlyBonus: employee.CalculateYearlyBonus(),
                PositionHistories: employee.PositionHistories
                    .Select(ph => ph.ToResponse())
                    .ToList(),
                EmployeeProjects: employee.EmployeeProjects
                    .Select(ep => ep.ToResponse())
                    .ToList()
            );
        }
    }
}
