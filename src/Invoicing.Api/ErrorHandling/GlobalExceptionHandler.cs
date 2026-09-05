namespace Invoicing.Api.ErrorHandling;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Invoicing.Application.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
  private readonly ILogger<GlobalExceptionHandler> _logger;

  public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
  {
    _logger = logger;
  }

  public async ValueTask<bool> TryHandleAsync(
      HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken)
  {
    var (statusCode, title) = MapException(exception);

    _logger.LogError(exception, "Kezeletlen kivétel történt: {Message}", exception.Message);

    var problemDetails = new ProblemDetails
    {
      Status = statusCode,
      Title = title,
      Detail = exception.Message,
      Instance = httpContext.Request.Path
    };

    httpContext.Response.StatusCode = statusCode;

    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

    return true;
  }

  private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
  {
    NotFoundException => (StatusCodes.Status404NotFound, "A hivatkozott erőforrás nem található"),
    ArgumentException => (StatusCodes.Status400BadRequest, "Érvénytelen bemeneti adat"),
    InvalidOperationException => (StatusCodes.Status400BadRequest, "A művelet nem hajtható végre"),
    _ => (StatusCodes.Status500InternalServerError, "Váratlan szerverhiba történt")
  };
}