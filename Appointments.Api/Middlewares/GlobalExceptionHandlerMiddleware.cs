using Appointments.Application.Exceptions;
using Newtonsoft.Json;

namespace Appointments.Api.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var endpoint = context.GetEndpoint();
                string controllerName = string.Empty;
                string actionName = string.Empty;

                if (endpoint != null)
                {
                    var routeValues = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
                    if (routeValues != null)
                    {
                        controllerName = routeValues.ControllerName;
                        actionName = routeValues.ActionName;
                    }
                }

                _logger.LogError(ex, "An unhandled exception has occurred in {ControllerName}/{ActionName}.", controllerName, actionName);

                await HandleExceptionAsync(context, ex, controllerName, actionName);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, string controllerName, string actionName)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                HttpException httpException => httpException.StatusCode,
                _ => StatusCodes.Status500InternalServerError,
            };

            // Preparamos la respuesta de error
            var errorResponse = new
            {
                context.Response.StatusCode,
                Message = "An error occurred while processing your request.",
                Detailed = exception.Message ?? "No additional details are available.",
                Controller = controllerName,
                Action = actionName
            };

            var errorJson = JsonConvert.SerializeObject(errorResponse);
            return context.Response.WriteAsync(errorJson);
        }
    }
}

