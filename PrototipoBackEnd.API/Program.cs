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

			Console.WriteLine("MONGO: " + (builder.Configuration["MongoDbSettings:ConnectionString"] ?? "NULL"));

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

					//options.Events = new JwtBearerEvents
					//{
					//	OnTokenValidated = context =>
					//	{
					//		// Verifica se o token possui a claim de role "Administrador"
					//		var hasAdminRole = context.Principal?.Identity is ClaimsIdentity claimsIdentity &&
					//						   claimsIdentity.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Administrador");

					//		// Se quiser aplicar isso globalmente, mantenha. Se for só para certas rotas, remova.
					//		// Aqui falha o token se não for administrador
					//		if (!hasAdminRole)
					//			context.Fail("Unauthorized - Apenas administradores podem acessar.");

					//		return Task.CompletedTask;
					//	}
					//};
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
				.AddSwaggerDocumentation()
				.AddFrontendCors();

			builder.Services.AddControllers();

			var app = builder.Build();

			// Habilita Swagger também quando a chave EnableSwaggerInProduction = true
			var enableSwaggerInProd = builder.Configuration.GetValue<bool>("EnableSwaggerInProduction");
			if (builder.Environment.IsDevelopment() || enableSwaggerInProd)
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrototipoBackEnd API V1");
					// se quiser o UI na raiz, descomente:
					// c.RoutePrefix = string.Empty;
				});
			}

			app.UseCustomMiddlewares();

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
