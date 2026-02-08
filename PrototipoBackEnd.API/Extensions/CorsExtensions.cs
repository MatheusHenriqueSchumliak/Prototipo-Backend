namespace PrototipoBackEnd.API.Extensions
{
	public static class CorsExtensions
	{
		public static IServiceCollection AddFrontendCors(this IServiceCollection services)
		{
			// Configuração de CORS
			services.AddCors(options =>
			{
				options.AddPolicy("Frontend", policy =>
				{
					policy
						.SetIsOriginAllowed(origin =>
						{
							// Localhost para desenvolvimento
							if (origin.StartsWith("http://localhost")) return true;

							// Qualquer subdomínio do Vercel do seu projeto
							if (origin.Contains("prototipo-frontend") && origin.EndsWith(".vercel.app"))
								return true;

							return false;
						})
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			return services;
		}
	}
}
