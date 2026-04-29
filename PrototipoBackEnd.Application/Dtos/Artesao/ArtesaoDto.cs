using PrototipoBackEnd.Application.Dtos.Pessoa;

namespace PrototipoBackEnd.Application.Dtos.Artesao
{
	public class ArtesaoDto
	{
		public string Id { get; set; } = string.Empty;
		public string PessoaId { get; set; } = string.Empty;
		public string Nome { get; set; } = string.Empty;
		public string Descricao { get; set; } = string.Empty;
		public string Foto { get; set; } = string.Empty;
		public bool RecebeEncomenda { get; set; }
		public bool EnviaEncomenda { get; set; }
		public bool LocalFisico { get; set; }
		public bool FeiraMunicipal { get; set; }
		public EspecialidadeDto Especialidade { get; set; } = new();
		public EnderecoDto? Endereco { get; set; }
		public RedesSociaisDto RedesSociais { get; set; } = new();
		public DateTime DataCriacao { get; set; }
		public DateTime? DataAtualizacao { get; set; }
		public DateTime? DataRemocao { get; set; }
		public bool IsAtivo { get; set; }
	}
}
