using PrototipoBackEnd.Infrastructure.Configurations;
using PrototipoBackEnd.Infrastructure.Mapping;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace PrototipoBackEnd.Infrastructure.Context
{
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;

		public MongoDbContext(IOptions<MongoDbSettings> settings)
		{
			var client = new MongoClient(settings.Value.ConnectionString);
			_database = client.GetDatabase(settings.Value.DatabaseName);
		}

		public IMongoDatabase GetDatabase() => _database;

		public IMongoCollection<T> GetCollection<T>()
		{
			var collectionName = MongoCollectionMapper.GetCollectionName<T>();
			return _database.GetCollection<T>(collectionName);
		}

	}
}
