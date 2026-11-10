using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Interfaces;

namespace PrototipoBackEnd.API.Startup
{
    // Cria o índice único de e-mail e o admin inicial (idempotente) ao subir a aplicação.
    public class DatabaseInitializer : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IServiceScopeFactory scopeFactory, ILogger<DatabaseInitializer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var usuarioRepository = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();
            await usuarioRepository.GarantirIndicesAsync();

            var seed = scope.ServiceProvider.GetRequiredService<IAdminSeedService>();
            var criado = await seed.ExecutarAsync();

            _logger.LogInformation(criado ? "Administrador inicial criado." : "Seed do administrador ignorado (sem configuração ou já existente).");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
