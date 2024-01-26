using HallOfFameWebApi.Services;
using HallOfFameWebApi.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HallOfFameWebApi.Filters
{
    public class EnsurePersonExistsAttribute : TypeFilterAttribute
    {
        public EnsurePersonExistsAttribute() : base(typeof(EnsurePersonExistsFilter)) { }

        public class EnsurePersonExistsFilter : IAsyncActionFilter
        {
            private readonly IPersonService _service;

            public EnsurePersonExistsFilter(IPersonService service)
            {
                _service = service;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context,
                ActionExecutionDelegate next)
            {
                if (!context.ActionArguments.TryGetValue("id", out var id)
                    || id is not long personId)
                {
                    throw new PersonIdNotDefinedException();
                };

                if (!await _service.DoesPersonExist(personId))
                {
                    throw new PersonNotFoundException(personId);
                }

                await next();
            }
        }
    }
}
