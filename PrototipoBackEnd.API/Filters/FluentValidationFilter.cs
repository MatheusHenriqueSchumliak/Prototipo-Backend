using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using ValidationException = PrototipoBackEnd.Application.Exceptions.ValidationException;

namespace PrototipoBackEnd.API.Filters
{
    // Executa os validators FluentValidation dos argumentos da action; o middleware global converte em 400.
    public class FluentValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argumento in context.ActionArguments.Values)
            {
                if (argumento == null) continue;

                var tipoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
                if (context.HttpContext.RequestServices.GetService(tipoValidator) is not IValidator validator)
                    continue;

                var resultado = await validator.ValidateAsync(new ValidationContext<object>(argumento));
                if (!resultado.IsValid)
                    throw new ValidationException(string.Join(" ", resultado.Errors.Select(e => e.ErrorMessage).Distinct()));
            }

            await next();
        }
    }
}
