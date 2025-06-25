using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Infrastructure.Mapping
{
	public class MongoCollectionMapper
	{
		private static readonly Dictionary<Type, string> CollectionMap = new()
		{
			{ typeof(Usuario), "Usuarios" },
			{ typeof(Artesao), "Artesaos" },
			{ typeof(Artesanato), "Artesanatos" },
			//{ typeof(Endereco), "Enderecos" },
		};

		public static string GetCollectionName(Type entityType)
		{
			return CollectionMap.TryGetValue(entityType, out var name)
				? name
				: entityType.Name;
		}

		public static string GetCollectionName<T>() => GetCollectionName(typeof(T));

	}
}
