using GrupoEmiTest.Domain.Common;
using GrupoEmiTest.Domain.Enums;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GrupoEmiTest.API.Extensions;

/// <summary>
/// Extension methods for converting <see cref="Result"/> failures into RFC-compliant
/// <c>application/problem+json</c> responses.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a failed <see cref="Result"/> into an <see cref="IActionResult"/> whose body
    /// matches the <c>ValidationProblemDetails</c> schema, including <c>traceId</c>.
    /// </summary>
    public static IActionResult ToProblemResult(this Result result, ControllerBase controller)
    {
        var error = result.Error;

        var (statusCode, title, type) = error.Type switch
        {
            ErrorType.Unauthorized => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "https://tools.ietf.org/html/rfc9110#section-15.5.2"),

            ErrorType.NotFound => (
                StatusCodes.Status404NotFound,
                "Not Found",
                "https://tools.ietf.org/html/rfc9110#section-15.5.5"),

            ErrorType.Conflict => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "https://tools.ietf.org/html/rfc9110#section-15.5.10"),

            ErrorType.Problem => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "https://tools.ietf.org/html/rfc9110#section-15.6.1"),

            _ => (
                StatusCodes.Status400BadRequest,
                "One or more validation errors occurred.",
                "https://tools.ietf.org/html/rfc9110#section-15.5.1")
        };

        var modelState = new ModelStateDictionary();
        modelState.AddModelError(error.Code, error.Description);

        var problemDetails = controller.ProblemDetailsFactory.CreateValidationProblemDetails(
            controller.HttpContext,
            modelState,
            statusCode: statusCode,
            title: title,
            type: type);

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }
}
