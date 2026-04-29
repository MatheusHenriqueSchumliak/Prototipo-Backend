namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class Endereco
	{
		public string CEP { get; }
		public string Estado { get; }
		public string Cidade { get; }
		public string Rua { get; }
		public string Bairro { get; }
		public string Numero { get; }
		public string Complemento { get; }

		public Endereco() { }
	}
}
