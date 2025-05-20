namespace PrototipoBackEnd.API.Extensions
{
	public static class CorsExtensions
	{
		public static IServiceCollection AddFrontendCors(this IServiceCollection services)
		{
			// Configuração de CORS
			services.AddCors(options =>
			{
				options.AddPolicy("PermitirFrontend", policy =>
				{
					policy.WithOrigins("http://localhost:5173") // Porta onde o React roda
						  .AllowAnyHeader()
						  .AllowAnyMethod();
				});
			});

			return services;
		}
	}
}
