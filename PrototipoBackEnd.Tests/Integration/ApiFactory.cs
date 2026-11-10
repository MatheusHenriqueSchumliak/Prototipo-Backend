using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PrototipoBackEnd.API;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Tests.Integration.Repositories;
using System.Net.Http.Headers;

namespace PrototipoBackEnd.Tests.Integration
{
    // Sobe a API real (pipeline, auth, políticas, controllers) trocando só a persistência e o S3 por fakes.
    public class ApiFactory : WebApplicationFactory<Program>
    {
        public const string JwtKey = "chave-de-teste-com-mais-de-32-caracteres-123456";

        public UsuarioRepositoryInMemory Usuarios { get; } = new();
        public PessoaRepositoryInMemory Pessoas { get; } = new();

        public string NovaPessoaId()
        {
            var pessoa = PessoaTestes.Criar();
            Pessoas.Adicionar(pessoa).GetAwaiter().GetResult();
            return pessoa.Id;
        }

        static ApiFactory()
        {
            // Variáveis de ambiente são lidas já em Program.Main (antes de qualquer override do host).
            Environment.SetEnvironmentVariable("Jwt__Key", JwtKey);
            Environment.SetEnvironmentVariable("Jwt__Issuer", "PrototipoTestes");
            Environment.SetEnvironmentVariable("Jwt__Audience", "PrototipoTestes");
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                Substituir<IUsuarioRepository>(services, Usuarios);
                Substituir<IUnitOfWork>(services, new UnitOfWorkFake());
                Substituir<IPessoaRepository>(services, Pessoas);
                Substituir<IArtesaoRepository>(services, new Mock<IArtesaoRepository>().Object);
                Substituir<IArtesanatoRepository>(services, new Mock<IArtesanatoRepository>().Object);
                Substituir<IAmazonS3Service>(services, new Mock<IAmazonS3Service>().Object);
            });
        }

        private static void Substituir<T>(IServiceCollection services, T instancia) where T : class
        {
            services.RemoveAll<T>();
            services.AddSingleton(instancia);
        }

        public HttpClient CriarClienteComRole(UsuarioEnum role)
        {
            var usuario = Usuario.Criar(Guid.NewGuid().ToString(), $"{role}-{Guid.NewGuid():N}@teste.com", "hash", role);
            using var scope = Services.CreateScope();
            var token = scope.ServiceProvider.GetRequiredService<ITokenService>().GenerateToken(usuario);

            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
