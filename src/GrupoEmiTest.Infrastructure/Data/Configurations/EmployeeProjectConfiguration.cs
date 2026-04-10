using GrupoEmiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoEmiTest.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the EF Core mapping for the <see cref="EmployeeProject"/> join entity,
/// which represents the many-to-many relationship between <see cref="Employee"/> and <see cref="Project"/>.
/// </summary>
public sealed class EmployeeProjectConfiguration : IEntityTypeConfiguration<EmployeeProject>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<EmployeeProject> builder)
    {
        builder.ToTable("EmployeeProjects");

        builder.HasKey(ep => new { ep.EmployeeId, ep.ProjectId });

        // EmployeeProject → Employee
        builder.HasOne(ep => ep.Employee)
            .WithMany(e => e.EmployeeProjects)
            .HasForeignKey(ep => ep.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // EmployeeProject → Project
        builder.HasOne(ep => ep.Project)
            .WithMany(p => p.EmployeeProjects)
            .HasForeignKey(ep => ep.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
