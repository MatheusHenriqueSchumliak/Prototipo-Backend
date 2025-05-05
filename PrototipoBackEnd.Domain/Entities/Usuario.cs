using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PrototipoBackEnd.Domain.Enumerables;

namespace PrototipoBackEnd.Domain.Entities
{
	public class Usuario
	{
		[BsonId]
		[BsonRepresentation(BsonType.String)] // Converte Guid para string
		public string Id { get; set; }
		public string Nome { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string SenhaHash { get; set; } = null!;

		[BsonRepresentation(BsonType.String)] // Salva como string no Mongo
		public UsuarioEnum Role { get; set; } = UsuarioEnum.Administrador;
		public bool IsAtivo { get; set; } = true;


		public Usuario() { } // para o Mongo ou ORM

		public static Usuario Criar(string nome, string email, string senhaHash, Enum role, bool isAtivo )
		{
			if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome vazio.");
			if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email vazio.");
			if (string.IsNullOrWhiteSpace(senhaHash)) throw new ArgumentException("Senha vazio.");			

			return new Usuario
			{
				Id = Guid.NewGuid().ToString(),
				Nome = nome.Trim(),
				Email = email.Trim().ToLower(),
				SenhaHash = senhaHash,
				Role = (UsuarioEnum)role,
				IsAtivo = true

			};
		}

	}
}
