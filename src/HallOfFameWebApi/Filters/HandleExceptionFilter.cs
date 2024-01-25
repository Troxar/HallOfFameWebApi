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
            bool isDevelopment = _environment.IsDevelopment();
            var details = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = isDevelopment
                    ? context.Exception.Message
                    : "Something went wrong",
                Detail = isDevelopment
                    ? context.Exception.StackTrace
                    : null
            };
            context.Result = new ObjectResult(details)
            {
                StatusCode = details.Status,
                ContentTypes = [MediaTypeNames.Application.ProblemJson]
            };
        }
    }
}
