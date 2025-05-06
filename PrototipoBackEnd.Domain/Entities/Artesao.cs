using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PrototipoBackEnd.Domain.Entities
{
	public class Artesao
	{
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public string Id { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string UsuarioId { get; set; }

		public string NomeArtesao { get; set; } = null!;
		public string Telefone { get; set; } = null!;
		public string WhatsApp { get; set; } = null!;
		public string DescricaoPerfil { get; set; } = null!;

		public bool ReceberEncomendas { get; set; } = false;
		public bool EnviaEncomendas { get; set; } = false;

		public string? FotoUrl { get; set; } = null!;

		// Endereço
		public string CEP { get; set; } = null!;
		public string Estado { get; set; } = null!;
		public string Cidade { get; set; } = null!;
		public string Rua { get; set; } = null!;
		public string Bairro { get; set; } = null!;
		public string Complemento { get; set; } = null!;
		public string Numero { get; set; } = null!;
		public bool SemNumero { get; set; } = false;

		// Relacionamento com artesanatos
		public List<string> ArtesanatoIds { get; set; } = new(); // Referência a IDs de artesanatos

		public Artesao() { }

		public static Artesao Criar(
			string usuarioId,
			string nomeArtesao,
			string telefone,
			string whatsapp,
			string descricaoPerfil,
			bool receberEncomendas,
			bool enviaEncomendas,
			string fotoUrl,
			string cep,
			string estado,
			string cidade,
			string rua,
			string bairro,
			string complemento,
			string numero,
			bool semNumero)
		{
			return new Artesao
			{
				Id = Guid.NewGuid().ToString(),
				UsuarioId = usuarioId,
				NomeArtesao = nomeArtesao,
				Telefone = telefone,
				WhatsApp = whatsapp,
				DescricaoPerfil = descricaoPerfil,
				ReceberEncomendas = receberEncomendas,
				EnviaEncomendas = enviaEncomendas,
				FotoUrl = fotoUrl,
				CEP = cep,
				Estado = estado,
				Cidade = cidade,
				Rua = rua,
				Bairro = bairro,
				Complemento = complemento,
				Numero = numero,
				SemNumero = semNumero
			};
		}
	}
}
