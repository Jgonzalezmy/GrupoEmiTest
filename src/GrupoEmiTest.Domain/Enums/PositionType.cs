namespace GrupoEmiTest.Domain.Enums;

/// <summary>
/// Represents the different position types an employee can hold.
/// Values at or above <see cref="TeamLead"/> are considered managerial positions
/// and qualify for a 20% yearly bonus instead of the standard 10%.
/// </summary>
public enum PositionType
{
    /// <summary>Standard individual-contributor role.</summary>
    RegularEmployee = 1,

    /// <summary>First-level manager — leads a technical team.</summary>
    TeamLead = 2,

    /// <summary>Mid-level manager — oversees a department.</summary>
    DepartmentManager = 3,

    /// <summary>Upper-level manager — oversees multiple departments.</summary>
    GeneralManager = 4,

    /// <summary>Executive manager — leads a business unit.</summary>
    Director = 5,

    /// <summary>Top-level executive.</summary>
    VicePresident = 6
}

