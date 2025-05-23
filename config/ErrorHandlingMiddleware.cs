using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using UserProviderApi.Models;
using UserProviderApi.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace UserProviderApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (!context.Response.HasStarted)
            {
                if (context.Response.StatusCode == 401)
                {
                    await HandleUnauthorizedResponse(context);
                }
                else if (context.Response.StatusCode == 403)
                {
                    await HandleForbiddenResponse(context);
                }
                else if (context.Response.StatusCode == 404)
                {
                    await HandleNotFoundResponse(context);
                }
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        
        var apiResponse = new ApiResponse<object?>
        {
            Status = "error",
            Datas = null,
            Message = "Internal Server Error",
        };

        switch (exception)
        {
            case ApiException apiException:
                response.StatusCode = apiException.StatusCode;
                apiResponse.Message = apiException.Message;
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError(exception, "An unhandled exception occurred");
                break;
        }

        await response.WriteAsJsonAsync(apiResponse);
    }

    private async Task HandleUnauthorizedResponse(HttpContext context)
    {
        var response = new ApiResponse<object?>
        {
            Status = "error",
            Message = "Authentication required",
            Datas = null
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleForbiddenResponse(HttpContext context)
    {
        var response = new ApiResponse<object?>
        {
            Status = "error",
            Message = "You don't have permission to access this resource",
            Datas = null
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleNotFoundResponse(HttpContext context)
    {
        var response = new ApiResponse<object?>
        {
            Status = "error",
            Message = "The requested resource was not found",
            Datas = null
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(response);
    }
}