namespace GrupoEmiTest.API.Middleware;

/// <summary>
/// Custom middleware that logs the details of every incoming HTTP request:
/// method, path, query string, and response status code.
/// It also measures and logs how long each request takes to process.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initialises a new instance of <see cref="RequestLoggingMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware component in the pipeline.</param>
    /// <param name="logger">The logger used to write request details.</param>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Intercepts the HTTP request, logs its details, invokes the next middleware,
    /// and then logs the response status code and elapsed time.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var startTime = DateTime.UtcNow;

        _logger.LogInformation(
            "[REQUEST] {Method} {Path}{QueryString} | TraceId: {TraceId}",
            request.Method,
            request.Path,
            request.QueryString,
            context.TraceIdentifier);

        await _next(context);

        var elapsed = (DateTime.UtcNow - startTime).TotalMilliseconds;

        _logger.LogInformation(
            "[RESPONSE] {Method} {Path} => {StatusCode} | Elapsed: {Elapsed:F1}ms | TraceId: {TraceId}",
            request.Method,
            request.Path,
            context.Response.StatusCode,
            elapsed,
            context.TraceIdentifier);
    }
}

