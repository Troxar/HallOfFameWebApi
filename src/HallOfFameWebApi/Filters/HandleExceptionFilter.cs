using HallOfFameWebApi.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
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

        private HttpStatusCode GetStatusCode(Exception ex) => ex switch
        {
            PersonNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };
    }
}
