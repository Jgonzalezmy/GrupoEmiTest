using GrupoEmiTest.Domain.Enums;

namespace GrupoEmiTest.Domain.Common
{
    /// <summary>
    /// Represents a domain error with a code, description, and type.
    /// </summary>
    public record Error
    {
        /// <summary>
        /// Represents the absence of an error.
        /// </summary>
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

        /// <summary>
        /// Represents an error caused by a null value being provided.
        /// </summary>
        public static readonly Error NullValue = new(
            "General.Null",
            "Null value was provided",
            ErrorType.Failure);

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> record.
        /// </summary>
        /// <param name="code">A unique code identifying the error.</param>
        /// <param name="description">A human-readable description of the error.</param>
        /// <param name="type">The <see cref="ErrorType"/> that categorizes the error.</param>
        public Error(string code, string description, ErrorType type)
        {
            Code = code;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result"/>.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <returns>A <see cref="Result"/> representing a failure with the given <paramref name="error"/>.</returns>
        public static implicit operator Result(Error error) => Result.Failure(error);

        /// <summary>
        /// Gets the unique code identifying the error.
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets the human-readable description of the error.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the <see cref="ErrorType"/> that categorizes the error.
        /// </summary>
        public ErrorType Type { get; }

        /// <summary>
        /// Creates a new <see cref="Error"/> of type <see cref="ErrorType.Failure"/>.
        /// </summary>
        /// <param name="code">A unique code identifying the error.</param>
        /// <param name="description">A human-readable description of the error.</param>
        /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.Failure"/>.</returns>
        public static Error Failure(string code, string description) =>
            new(code, description, ErrorType.Failure);

        /// <summary>
        /// Creates a new <see cref="Error"/> of type <see cref="ErrorType.NotFound"/>.
        /// </summary>
        /// <param name="code">A unique code identifying the error.</param>
        /// <param name="description">A human-readable description of the error.</param>
        /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.NotFound"/>.</returns>
        public static Error NotFound(string code, string description) =>
            new(code, description, ErrorType.NotFound);

        /// <summary>
        /// Creates a new <see cref="Error"/> of type <see cref="ErrorType.Problem"/>.
        /// </summary>
        /// <param name="code">A unique code identifying the error.</param>
        /// <param name="description">A human-readable description of the error.</param>
        /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.Problem"/>.</returns>
        public static Error Problem(string code, string description) =>
            new(code, description, ErrorType.Problem);

        /// <summary>
        /// Creates a new <see cref="Error"/> of type <see cref="ErrorType.Conflict"/>.
        /// </summary>
        /// <param name="code">A unique code identifying the error.</param>
        /// <param name="description">A human-readable description of the error.</param>
        /// <returns>A new <see cref="Error"/> instance with <see cref="ErrorType.Conflict"/>.</returns>
        public static Error Conflict(string code, string description) =>
            new(code, description, ErrorType.Conflict);
    }
}
