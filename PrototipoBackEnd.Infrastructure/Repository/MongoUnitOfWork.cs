using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Infrastructure.Context;
using MongoDB.Driver;

namespace PrototipoBackEnd.Infrastructure.Repository
{
    // Requer MongoDB em replica set (ex.: Atlas) para suportar transações.
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly MongoDbContext _context;

        public MongoUnitOfWork(MongoDbContext context)
        {
            _context = context;
        }

        public async Task ExecutarAsync(Func<Task> operacao)
        {
            if (_context.SessaoAtual != null)
            {
                await operacao();
                return;
            }

            using var sessao = await _context.Client.StartSessionAsync();
            _context.SessaoAtual = sessao;
            try
            {
                await sessao.WithTransactionAsync(async (_, _) =>
                {
                    await operacao();
                    return true;
                });
            }
            finally
            {
                _context.SessaoAtual = null;
            }
        }
    }
}
