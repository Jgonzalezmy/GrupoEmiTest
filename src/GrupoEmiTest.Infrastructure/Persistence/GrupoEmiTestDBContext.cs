using GrupoEmiTest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GrupoEmiTest.Infrastructure.Persistence;

/// <summary>
/// Represents the Entity Framework Core database context for the GrupoEmiTest application.
/// Provides access to the underlying database and exposes <see cref="DbSet{TEntity}"/> properties
/// for each entity in the domain model.
/// </summary>
public partial class GrupoEmiTestDBContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="GrupoEmiTestDBContext"/> using the default constructor.
    /// This constructor is typically used by design-time tools such as EF Core migrations.
    /// </summary>
    public GrupoEmiTestDBContext() { }

    /// <summary>
    /// Initializes a new instance of <see cref="GrupoEmiTestDBContext"/> with the specified options.
    /// This constructor is used when the context is resolved through dependency injection.
    /// </summary>
    /// <param name="options">
    /// The options to be used by the <see cref="DbContext"/>, including the database provider
    /// and connection string configuration.
    /// </param>
    public GrupoEmiTestDBContext(DbContextOptions<GrupoEmiTestDBContext> options)
        : base(options) { }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> for the <see cref="Employee"/> entity.
    /// Provides CRUD access to the <c>Employee</c> table in the database.
    /// </summary>
    public virtual DbSet<Employee> Employee { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> for the <see cref="ApplicationUser"/> entity.
    /// Provides CRUD access to the <c>Users</c> table in the database.
    /// </summary>
    public virtual DbSet<ApplicationUser> User { get; set; }

    /// <summary>
    /// Configures the database provider and connection string when no options have been
    /// injected via the constructor. This serves as a fallback for scenarios such as
    /// design-time tooling or direct instantiation without a DI container.
    /// </summary>
    /// <param name="optionsBuilder">
    /// A builder used to create or modify options for this context. If the builder is already
    /// configured, this method performs no action.
    /// </param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback: configure via connection string if no options were injected
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:SqlServer");
        }
    }

    /// <summary>
    /// Configures the entity model when the context is first created. This method discovers
    /// and applies all <see cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration{TEntity}"/>
    /// implementations defined in the current assembly, keeping entity configurations
    /// organized and separated from the context class.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder used to construct the model for this context. Entity configurations,
    /// relationships, and constraints are defined through this builder.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all IEntityTypeConfiguration<T> implementations found in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GrupoEmiTestDBContext).Assembly);
    }
}
