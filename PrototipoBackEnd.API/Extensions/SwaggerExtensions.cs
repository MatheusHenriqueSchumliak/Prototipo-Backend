using Microsoft.OpenApi.Models;

namespace PrototipoBackEnd.API.Extensions
{
	public static class SwaggerExtensions
	{
		public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
		{
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "Prototipo API",
					Version = "v1"
				});

				options.AddSecurityDefinition(name: "Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Informe apenas o token JWT (o prefixo 'Bearer' é adicionado automaticamente)."

				});

				options.AddSecurityRequirement(new OpenApiSecurityRequirement()
				  {
					{
					  new OpenApiSecurityScheme
					  {
						Reference = new OpenApiReference
						{
						  Type = ReferenceType.SecurityScheme,
						  Id = "Bearer"
						}
					  },
					  Array.Empty<string>()
					}
				});
			});

			return services;
		}
	}
}
