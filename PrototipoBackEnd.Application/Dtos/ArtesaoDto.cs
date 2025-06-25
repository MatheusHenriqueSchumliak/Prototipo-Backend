namespace PrototipoBackEnd.Application.Dtos
{
	public class ArtesaoDto
	{
		public string Id { get; set; }
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
		public string NichoAtuacao { get; set; } = null!;

		public bool LocalFisico { get; set; } = false;
		public bool FeiraMunicipal { get; set; } = false;
		// Endereço
		public string CEP { get; set; } = null!;
		public string Estado { get; set; } = null!;
		public string Cidade { get; set; } = null!;
		public string Rua { get; set; } = null!;
		public string Bairro { get; set; } = null!;
		public string? Complemento { get; set; } = null!;
		public string Numero { get; set; } = null!;
		public bool SemNumero { get; set; } = false;

		// Relacionamento com artesanatos
		public List<string> ArtesanatoIds { get; set; } = new(); // Referência a IDs de artesanatos



	}
}
