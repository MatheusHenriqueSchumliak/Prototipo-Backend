using Microsoft.AspNetCore.Http;

namespace PrototipoBackEnd.Application.Dtos.Artesanato
{
	public class ArtesanatoDto
	{
		public string Id { get; set; } = string.Empty;
		public string PessoaId { get; set; } = string.Empty;
		public string ArtesaoId { get; set; } = string.Empty;
		public string Titulo { get; set; } = string.Empty;
		public string Descricao { get; set; } = string.Empty;
		public int TempoCriacao { get; set; }
		public int Quantidade { get; set; }
		public decimal? Preco { get; set; }
		public bool TemEstoque { get; set; }
		public bool SomenteEncomenda { get; set; }
		public bool AceitaEncomenda { get; set; }
		public CategoriaDto Categoria { get; set; } = new();
		public MaterialDto Material { get; set; } = new();
		public MidiaDto Midia { get; set; } = new();
		public DateTime DataCriacao { get; set; }
		public DateTime? DataAtualizacao { get; set; }
		public DateTime? DataRemocao { get; set; }
		public bool IsAtivo { get; set; }
	}
}
