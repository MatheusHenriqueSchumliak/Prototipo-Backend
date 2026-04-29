using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Infrastructure.Context;
using PrototipoBackEnd.Domain.Entities;
using MongoDB.Driver;

namespace PrototipoBackEnd.Infrastructure.Repository
{
	public class PessoaRepository : GenericRepository<Pessoa>, IPessoaRepository
	{
		#region Construtor
		private readonly IMongoCollection<Pessoa> _pessoaCollection;
		public PessoaRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
		{
			_pessoaCollection = mongoDbContext.GetDatabase().GetCollection<Pessoa>("Pessoas");
		}
		#endregion
	}
}
