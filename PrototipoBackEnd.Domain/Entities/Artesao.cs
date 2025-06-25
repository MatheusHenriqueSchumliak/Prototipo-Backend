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
		public required string UsuarioId { get; set; }

		public string NomeCompleto { get; set; } = null!;
		public int Idade { get; set; }
		public string NomeArtesao { get; set; } = null!;
		public string Telefone { get; set; } = null!;
		public string WhatsApp { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Instagram { get; set; } = null!;
		public string Facebook { get; set; } = null!;
		public string DescricaoPerfil { get; set; } = null!;
		public bool ReceberEncomendas { get; set; } = false;
		public bool EnviaEncomendas { get; set; } = false;
		public string? FotoUrl { get; set; } = null!;
		public string? NichoAtuacao { get; set; } = null!;
		public bool LocalFisico { get; set; } = false;
		public bool FeiraMunicipal { get; set; } = false;
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
		[BsonIgnore]
		public List<Artesanato>? Artesanatos { get; set; }
		public Artesao() { }

		public static Artesao Criar(
			string usuarioId,
			string nomeArtesao,
			int idade,
			string nomeCompleto,
			string telefone,
			string whatsapp,
			string email,
			string instagram,
			string facebook,
			string descricaoPerfil,
			bool receberEncomendas,
			bool enviaEncomendas,
			string fotoUrl,
			string nichoAtuacao,
			bool localFisico,
			bool feiraMunicipal,
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
				NomeCompleto = nomeCompleto,
				Idade = idade,
				NomeArtesao = nomeArtesao,
				Telefone = telefone,
				WhatsApp = whatsapp,
				Email = email,
				Instagram = instagram,
				Facebook = facebook,
				DescricaoPerfil = descricaoPerfil,
				ReceberEncomendas = receberEncomendas,
				EnviaEncomendas = enviaEncomendas,
				FotoUrl = fotoUrl,
				NichoAtuacao = nichoAtuacao,
				LocalFisico = localFisico,
				FeiraMunicipal = feiraMunicipal,
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
