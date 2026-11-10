using Microsoft.Extensions.Configuration;
using PrototipoBackEnd.API.Extensions;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Api
{
    public class ConfiguracaoSegurancaTests
    {
        private static IConfiguration Config(string? chave)
            => new ConfigurationBuilder()
                .AddInMemoryCollection(chave == null ? new Dictionary<string, string?>() : new Dictionary<string, string?> { ["Jwt:Key"] = chave })
                .Build();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("curta")]
        [InlineData("1234567890123456789012345678901")] // 31 caracteres
        public void ObterChave_AusenteOuCurta_Lanca(string? chave)
        {
            Assert.Throws<InvalidOperationException>(() => JwtConfiguration.ObterChave(Config(chave)));
        }

        [Fact]
        public void ObterChave_Com32Caracteres_Retorna()
        {
            var bytes = JwtConfiguration.ObterChave(Config("12345678901234567890123456789012"));

            Assert.Equal(32, bytes.Length);
        }

        [Theory]
        [InlineData("http://localhost:3000", true)]
        [InlineData("http://localhost:5173", true)]
        [InlineData("http://localhost", true)]
        [InlineData("https://prototipo-frontend-sage.vercel.app", true)]
        [InlineData("https://prototipo-frontend-evil.vercel.app", false)]
        [InlineData("https://evil.com/prototipo-frontend-sage.vercel.app", false)]
        [InlineData("http://localhost.evil.com", false)]
        [InlineData("https://localhost:3000", false)]
        [InlineData("http://prototipo-frontend-sage.vercel.app", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Cors_OrigemPermitida(string? origem, bool esperado)
        {
            Assert.Equal(esperado, CorsExtensions.OrigemPermitida(origem));
        }
    }
}
