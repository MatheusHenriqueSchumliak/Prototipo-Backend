using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PrototipoBackEnd.Domain.Enumerables;

namespace PrototipoBackEnd.API.Extensions
{
    public static class Policies
    {
        public const string Administrador = "Administrador";
    }

    public static class JwtConfiguration
    {
        public const int TamanhoMinimoChave = 32;

        public static byte[] ObterChave(IConfiguration configuration)
        {
            var chave = configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(chave) || chave.Length < TamanhoMinimoChave)
                throw new InvalidOperationException(
                    $"Jwt:Key ausente ou com menos de {TamanhoMinimoChave} caracteres. Configure via user-secrets ou variável de ambiente (Jwt__Key).");

            return Encoding.UTF8.GetBytes(chave);
        }
    }

    public static class AuthExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Falha na inicialização se a chave for inválida.
            var key = JwtConfiguration.ObterChave(configuration);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }

        // Toda regra de acesso passa por políticas: para dar escrita ao usuário comum no futuro, basta criar/ajustar uma política aqui.
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Administrador, policy =>
                    policy.RequireClaim(ClaimTypes.Role, nameof(UsuarioEnum.Administrador)));
            });

            return services;
        }
    }
}
