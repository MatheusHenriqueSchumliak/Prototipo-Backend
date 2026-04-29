namespace PrototipoBackEnd.Application.Dtos.Pessoa
{
	public class PessoaDto
	{
		public string Id { get; set; } = string.Empty;
		public string NomeCompleto { get; set; } = string.Empty;
		public DateTime DataNascimento { get; set; }
		public EnderecoDto Endereco { get; set; } = new();
		public ContatoDto Contato { get; set; } = new();
		public bool TemUsuario { get; set; }
		public bool EhArtesao { get; set; }
		public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
		public DateTime? DataAtualizacao { get; set; } = null!;
		public DateTime? DataRemocao { get; set; } = null!;
		public bool IsAtivo { get; set; }
	}
}
