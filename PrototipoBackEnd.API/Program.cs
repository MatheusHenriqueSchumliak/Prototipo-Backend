using FluentValidation;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using PrototipoBackEnd.API.Extensions;
using PrototipoBackEnd.API.Middlewares;
using PrototipoBackEnd.API.Responses;
using PrototipoBackEnd.API.Startup;
using PrototipoBackEnd.Infrastructure.IoC;

namespace PrototipoBackEnd.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddJwtAuthentication(builder.Configuration)
                .AddAuthorizationPolicies();

            builder.Services.AddHttpContextAccessor();

            builder.Services
                .AddInfrastructure(builder.Configuration)
                .AddSwaggerDocumentation()
                .AddFrontendCors();

            builder.Services.AddHostedService<DatabaseInitializer>();

            builder.Services.AddValidatorsFromAssemblyContaining<PrototipoBackEnd.Application.Validators.PessoaDtoValidator>();

            builder.Services
                .AddControllers(options => options.Filters.Add<PrototipoBackEnd.API.Filters.FluentValidationFilter>())
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // Erros de validação no formato { message, errors } (sem ProblemDetails)
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var erros = context.ModelState
                            .Where(e => e.Value?.Errors.Count > 0)
                            .SelectMany(e => e.Value!.Errors.Select(er => new FieldError(e.Key, er.ErrorMessage)))
                            .ToList();

                        return new BadRequestObjectResult(new ErrorResponse("Dados inválidos.", erros));
                    };
                });

            var app = builder.Build();

            // Primeiro da pipeline: captura exceções de todo o resto e responde no formato { message, errors? }
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Habilita Swagger também quando a chave EnableSwaggerInProduction = true
            var enableSwaggerInProd = builder.Configuration.GetValue<bool>("EnableSwaggerInProduction");
            if (builder.Environment.IsDevelopment() || enableSwaggerInProd)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrototipoBackEnd API V1");

                    // Oculta a seção "Models" (Schemas) do Swagger UI
                    c.DefaultModelsExpandDepth(-1);
                    c.DefaultModelExpandDepth(-1);
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Frontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", () => "PrototipoBackEnd API rodando no Azure 🚀");

            app.MapControllers();

            app.Run();
        }
    }
}
