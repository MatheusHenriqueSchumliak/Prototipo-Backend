namespace PrototipoBackEnd.API.Extensions
{
    public static class CorsExtensions
    {
        public const string FrontendProducao = "https://prototipo-frontend-sage.vercel.app";

        public static bool OrigemPermitida(string? origin)
        {
            if (string.IsNullOrWhiteSpace(origin) || !Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                return false;

            // Produção: origem exata
            if (string.Equals(origin.TrimEnd('/'), FrontendProducao, StringComparison.OrdinalIgnoreCase))
                return true;

            // Desenvolvimento: http://localhost em qualquer porta
            return uri.Scheme == Uri.UriSchemeHttp
                && string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase);
        }

        public static IServiceCollection AddFrontendCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy.SetIsOriginAllowed(OrigemPermitida)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
