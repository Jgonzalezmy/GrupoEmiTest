namespace GrupoEmiTest.Domain.Enums;

/// <summary>
/// Represents the category of an error returned by the application.
/// Used to classify failures and map them to appropriate HTTP responses or handling strategies.
/// </summary>
public enum ErrorType
{
    /// <summary>A general, unclassified failure.</summary>
    Failure = 0,

    /// <summary>One or more input values failed validation rules.</summary>
    Validation = 1,

    /// <summary>An unexpected problem occurred during processing.</summary>
    Problem = 2,

    /// <summary>The requested resource could not be found.</summary>
    NotFound = 3,

    /// <summary>The operation conflicts with the current state of the resource.</summary>
    Conflict = 4,

    /// <summary>The caller is not authenticated or the credentials are invalid.</summary>
    Unauthorized = 5
}

