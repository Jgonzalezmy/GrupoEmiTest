using GrupoEmiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoEmiTest.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the EF Core mapping for the <see cref="Project"/> entity,
/// including its one-to-many relationship with <see cref="EmployeeProject"/>.
/// </summary>
public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Projects_Name");
    }
}
