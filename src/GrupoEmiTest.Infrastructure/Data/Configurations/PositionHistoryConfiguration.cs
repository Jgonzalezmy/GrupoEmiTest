using GrupoEmiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrupoEmiTest.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the EF Core mapping for the <see cref="PositionHistory"/> entity,
/// including its many-to-one relationship with <see cref="Employee"/>.
/// </summary>
public sealed class PositionHistoryConfiguration : IEntityTypeConfiguration<PositionHistory>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<PositionHistory> builder)
    {
        builder.ToTable("PositionHistories");

        builder.HasKey(ph => ph.Id);

        builder.Property(ph => ph.Position)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ph => ph.StartDate)
            .IsRequired();

        builder.Property(ph => ph.EndDate);

        builder.HasIndex(ph => ph.EmployeeId)
            .HasDatabaseName("IX_PositionHistories_EmployeeId");

        // PositionHistory → Employee (many-to-one)
        builder.HasOne(ph => ph.Employee)
            .WithMany(e => e.PositionHistories)
            .HasForeignKey(ph => ph.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
