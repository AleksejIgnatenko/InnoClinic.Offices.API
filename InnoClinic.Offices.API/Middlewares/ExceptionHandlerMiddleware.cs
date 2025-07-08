using System.Text.Json;
using FluentValidation;
using InnoClinic.Offices.Core.Exceptions;
using Serilog;

namespace InnoClinic.Offices.API.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var statusCode = StatusCodes.Status400BadRequest;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = ex.Errors });
            await context.Response.WriteAsync(result);
        }
        catch (ExceptionWithStatusCode ex)
        {
            var statusCode = (int)ex.HttpStatusCode;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            await context.Response.WriteAsync(result);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            await context.Response.WriteAsync(result);
        }
    }
}
