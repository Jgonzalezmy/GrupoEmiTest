using GrupoEmiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoEmiTest.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the EF Core mapping for the <see cref="Department"/> entity,
/// including its one-to-many relationship with <see cref="Employee"/>.
/// </summary>
public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(d => d.Name)
            .IsUnique()
            .HasDatabaseName("IX_Departments_Name");
    }
}
