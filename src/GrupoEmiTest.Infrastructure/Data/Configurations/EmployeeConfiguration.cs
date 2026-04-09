using GrupoEmiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoEmiTest.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the EF Core mapping for the <see cref="Employee"/> entity,
/// including its relationships with <see cref="Department"/>, <see cref="PositionHistory"/>,
/// and the many-to-many link to <see cref="Project"/> via <see cref="EmployeeProject"/>.
/// </summary>
public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.CurrentPosition)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.Salary)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_Employees_Name");

        // Employee → Department (many-to-one)
        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Employee → EmployeeProject (one-to-many side of the M:N)
        builder.HasMany(e => e.EmployeeProjects)
            .WithOne(ep => ep.Employee)
            .HasForeignKey(ep => ep.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}