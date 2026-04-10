namespace GrupoEmiTest.API.Middleware.Extensions;

/// <summary>
/// Extension method to register <see cref="RequestLoggingMiddleware"/> in the pipeline.
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the <see cref="RequestLoggingMiddleware"/> to the application's request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The updated <paramref name="app"/> for method chaining.</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        => app.UseMiddleware<RequestLoggingMiddleware>();
}