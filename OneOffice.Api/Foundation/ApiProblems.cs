using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OneOffice.Api.Foundation;

internal static class ApiProblems
{
    public static ProblemDetails Create(HttpContext context, int status)
    {
        var code = status switch
        {
            400 => "request.invalid", 401 => "auth.session_invalid", 403 => "access.denied",
            404 => "resource.not_found", 409 => "concurrency.conflict",
            429 => "rate_limit.exceeded", _ => "server.unexpected"
        };
        var problem = new ProblemDetails
        {
            Status = status, Type = $"urn:oneoffice:problem:{code}",
            Title = status >= 500 ? "An unexpected error occurred." : "The request could not be completed."
        };
        problem.Extensions["code"] = code;
        problem.Extensions["traceId"] = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        return problem;
    }
}

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var status = exception is BadHttpRequestException ? 400 : 500;
        // Deliberately omit exception/message/body/headers: they can contain credentials.
        logger.LogError("Request failure {StatusCode} {TraceId}", status, context.TraceIdentifier);
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(ApiProblems.Create(context, status),
            options: (System.Text.Json.JsonSerializerOptions?)null, contentType: "application/problem+json", cancellationToken: cancellationToken);
        return true;
    }
}
