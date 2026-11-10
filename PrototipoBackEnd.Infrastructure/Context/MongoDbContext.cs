using PrototipoBackEnd.Infrastructure.Configurations;
using PrototipoBackEnd.Infrastructure.Mapping;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace PrototipoBackEnd.Infrastructure.Context
{
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;

		public IMongoClient Client { get; }

		// Sessão da transação em andamento (null fora de uma unidade de trabalho)
		public IClientSessionHandle? SessaoAtual { get; set; }

		public MongoDbContext(IOptions<MongoDbSettings> settings)
		{
			Client = new MongoClient(settings.Value.ConnectionString);
			_database = Client.GetDatabase(settings.Value.DatabaseName);
		}

		public IMongoDatabase GetDatabase() => _database;

		public IMongoCollection<T> GetCollection<T>()
		{
			var collectionName = MongoCollectionMapper.GetCollectionName<T>();
			return _database.GetCollection<T>(collectionName);
		}

	}
}
