using System.Diagnostics.CodeAnalysis;

namespace GrupoEmiTest.Domain.Common;

/// <summary>
/// Represents the outcome of an operation, indicating whether it succeeded or failed.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">
    /// <see langword="true"/> if the operation succeeded; <see langword="false"/> if it failed.
    /// </param>
    /// <param name="error">
    /// The <see cref="Common.Error"/> associated with the result.
    /// Must be <see cref="Error.None"/> when <paramref name="isSuccess"/> is <see langword="true"/>,
    /// and must not be <see cref="Error.None"/> when <paramref name="isSuccess"/> is <see langword="false"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="isSuccess"/> is <see langword="true"/> but <paramref name="error"/> is not <see cref="Error.None"/>,
    /// or when <paramref name="isSuccess"/> is <see langword="false"/> but <paramref name="error"/> is <see cref="Error.None"/>.
    /// </exception>
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the operation succeeded; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the operation failed; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the <see cref="Common.Error"/> associated with the result.
    /// Returns <see cref="Error.None"/> when the operation was successful.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Creates a successful <see cref="Result"/> with no value.
    /// </summary>
    /// <returns>A <see cref="Result"/> instance representing a successful operation.</returns>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a successful <see cref="Result{TValue}"/> carrying the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value returned by the successful operation.</typeparam>
    /// <param name="value">The value produced by the successful operation.</param>
    /// <returns>A <see cref="Result{TValue}"/> instance representing a successful operation with the given <paramref name="value"/>.</returns>
    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    /// <summary>
    /// Creates a failed <see cref="Result"/> with the specified error.
    /// </summary>
    /// <param name="error">The <see cref="Common.Error"/> that describes the failure.</param>
    /// <returns>A <see cref="Result"/> instance representing a failed operation.</returns>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value that would have been returned on success.</typeparam>
    /// <param name="error">The <see cref="Common.Error"/> that describes the failure.</param>
    /// <returns>A <see cref="Result{TValue}"/> instance representing a failed operation.</returns>
    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);
}

/// <summary>
/// Represents the outcome of an operation that returns a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value returned when the operation succeeds.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value produced by the operation, or <see langword="default"/> on failure.</param>
    /// <param name="isSuccess">
    /// <see langword="true"/> if the operation succeeded; <see langword="false"/> if it failed.
    /// </param>
    /// <param name="error">
    /// The <see cref="Error"/> associated with the result.
    /// Must be <see cref="Error.None"/> on success and a non-<see cref="Error.None"/> value on failure.
    /// </param>
    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value produced by the successful operation.
    /// </summary>
    /// <value>The value of type <typeparamref name="TValue"/> returned by the operation.</value>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to access the value of a failed result.
    /// </exception>
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    /// <summary>
    /// Implicitly converts a <typeparamref name="TValue"/> to a <see cref="Result{TValue}"/>.
    /// Returns a successful result if the value is not <see langword="null"/>;
    /// otherwise returns a failure result with <see cref="Error.NullValue"/>
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>
    /// A successful <see cref="Result{TValue}"/> wrapping <paramref name="value"/>, 
    /// or a failure result with <see cref="Error.NullValue"/> if <paramref name="value"/> is <see langword="null"/>.
    /// </returns>
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> that describes the failure.</param>
    /// <returns>A <see cref="Result{TValue}"/> instance representing a failed operation with the given <paramref name="error"/>.</returns>
    public static implicit operator Result<TValue>(Error error) => new Result<TValue>(default, false, error);

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> representing a validation failure with the specified error.
    /// </summary>
    /// <param name="error">The <see cref="Error"/> that describes the validation failure.</param>
    /// <returns>A <see cref="Result{TValue}"/> instance representing a validation failure.</returns>
    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}

