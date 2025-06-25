using Microsoft.AspNetCore.Authentication.JwtBearer;
using PrototipoBackEnd.API.WebApplicationExtensions;
using PrototipoBackEnd.Infrastructure.IoC;
using PrototipoBackEnd.API.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace PrototipoBackEnd.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "sua_chave_padrao");

					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = builder.Configuration["Jwt:Issuer"],
						ValidAudience = builder.Configuration["Jwt:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ClockSkew = TimeSpan.Zero // ⬅️ IMPORTANTE para não permitir tolerância padrão de 5 minutos!
					};

					options.Events = new JwtBearerEvents
					{
						OnTokenValidated = context =>
						{
							// Verifica se o token possui a claim de role "Administrador"
							var hasAdminRole = context.Principal?.Identity is ClaimsIdentity claimsIdentity &&
											   claimsIdentity.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Administrador");

							// Se quiser aplicar isso globalmente, mantenha. Se for só para certas rotas, remova.
							// Aqui falha o token se não for administrador
							if (!hasAdminRole)
								context.Fail("Unauthorized - Apenas administradores podem acessar.");

							return Task.CompletedTask;
						}
					};
				});

			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
				//options.AddPolicy("Usuario", policy => policy.RequireRole("Usuario"));
			}); 
			builder.Services.AddHttpContextAccessor();
			// Adiciona a injeção de dependência da camada Infrastructure
			builder.Services
				.AddInfrastructure(builder.Configuration)
				.AddConfiguredControllers()
				.AddSwaggerDocumentation()
				.AddFrontendCors();

			var app = builder.Build();

			app.UseCustomMiddlewares();

			app.MapControllers();

			app.Run();
		}
	}
}
