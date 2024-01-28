using HallOfFameWebApi.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Net.Mime;

namespace HallOfFameWebApi.Filters
{
    public class HandleExceptionFilter : IExceptionFilter
    {
        private readonly IHostEnvironment _environment;

        public HandleExceptionFilter(IHostEnvironment environment)
        {
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            WriteToLog(context);
            SetContextResult(context);
            context.ExceptionHandled = true;
        }

        private void SetContextResult(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = (int)GetStatusCode(context.Exception),
                Title = context.Exception.Message,
                Detail = _environment.IsDevelopment()
                    ? context.Exception.StackTrace
                    : null
            };
            context.Result = new ObjectResult(details)
            {
                StatusCode = details.Status,
                ContentTypes = [MediaTypeNames.Application.ProblemJson]
            };
        }

        private static HttpStatusCode GetStatusCode(Exception ex) => ex switch
        {
            PersonNotFoundException => HttpStatusCode.NotFound,
            PersonIdNotDefinedException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        private void WriteToLog(ExceptionContext context)
        {
            ILogger logger = GetLogger(context);
            LogLevel logLevel = GetLogLevel(context.Exception);

            if (context.Exception is PersonException personException)
            {
                personException.WriteToLog(logger, logLevel);
            }
            else
            {
                logger.Log(logLevel, context.Exception.Message);
            }
        }

        private ILogger GetLogger(ExceptionContext context)
        {
            Type controllerType = context.ActionDescriptor is ControllerActionDescriptor descriptor
                ? descriptor.ControllerTypeInfo
                : GetType();
            Type loggerType = typeof(ILogger<>).MakeGenericType(controllerType);
            return (ILogger)context.HttpContext.RequestServices.GetRequiredService(loggerType);
        }

        private static LogLevel GetLogLevel(Exception ex) => ex switch
        {
            PersonException => LogLevel.Warning,
            _ => LogLevel.Error
        };
    }
}
