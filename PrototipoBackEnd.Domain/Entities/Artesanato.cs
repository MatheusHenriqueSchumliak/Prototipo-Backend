using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace PrototipoBackEnd.Domain.Entities
{
	public class Artesanato
	{
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public required string Id { get; set; }
		[BsonRepresentation(BsonType.String)]
		public required string UsuarioId { get; set; }
		public string? ArtesaoId { get; set; } = null!;
		public List<string>? ImagemUrl { get; set; } = null!;
		public bool? SobEncomenda { get; set; } = null!;
		public bool? AceitaEncomenda { get; set; } = null!;
		public List<string>? CategoriaTags { get; set; } = null!;
		public string? TituloArtesanato { get; set; } = null!;
		public decimal? Preco { get; set; } = null!;
		public int QuantidadeArtesanato { get; set; }
		public string? DescricaoArtesanato { get; set; } = null!;
		public string? MateriaisUtilizados { get; set; } = null!;		
		public DateTime? DataCriacao { get; set; } = null!;
		public int? TempoCriacaoHr { get; set; } = null!;

		[BsonIgnore]
		public Artesao? Artesao { get; set; }
		public Artesanato() { }
	}
}
