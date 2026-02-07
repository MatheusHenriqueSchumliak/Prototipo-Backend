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
						.WithOrigins(
							"http://localhost:5173",
							"https://prototipo-frontend-git-main-mhs421.vercel.app",
							"https://prototipo-frontend-git-feature-mhs421.vercel.app"
						)
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

			return services;
		}
	}
}
