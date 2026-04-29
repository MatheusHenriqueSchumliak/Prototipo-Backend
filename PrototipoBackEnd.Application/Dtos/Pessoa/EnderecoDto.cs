namespace PrototipoBackEnd.Application.Dtos.Pessoa
{
	public class EnderecoDto
	{
		public string CEP { get; set; } = string.Empty;
		public string Estado { get; set; } = string.Empty;
		public string Cidade { get; set; } = string.Empty;
		public string Rua { get; set; } = string.Empty;
		public string Bairro { get; set; } = string.Empty;
		public string Numero { get; set; } = string.Empty;
		public string Complemento { get; set; } = string.Empty;
		public bool SemNumero { get; set; }
	}
}
