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
		public bool SemNumero { get; }

		private Endereco()
		{
			CEP = string.Empty;
			Estado = string.Empty;
			Cidade = string.Empty;
			Rua = string.Empty;
			Bairro = string.Empty;
			Numero = string.Empty;
			Complemento = string.Empty;
			SemNumero = false;
		}

		public Endereco(string cep, string estado, string cidade, string rua, string bairro, string numero, string? complemento, bool semNumero)
		{
			if (string.IsNullOrWhiteSpace(cep))
				throw new ArgumentException("CEP vazio.");

			var cepDigits = string.Concat(cep.Where(char.IsDigit));
			if (cepDigits.Length != 8)
				throw new ArgumentException("CEP inválido.");

			if (string.IsNullOrWhiteSpace(estado))
				throw new ArgumentException("Estado vazio.");

			estado = estado.Trim().ToUpper();
			if (estado.Length != 2)
				throw new ArgumentException("Estado deve ter 2 caracteres.");

			if (string.IsNullOrWhiteSpace(cidade))
				throw new ArgumentException("Cidade vazia.");

			if (string.IsNullOrWhiteSpace(rua))
				throw new ArgumentException("Rua vazia.");

			if (string.IsNullOrWhiteSpace(bairro))
				throw new ArgumentException("Bairro vazio.");

			if (string.IsNullOrWhiteSpace(numero))
				throw new ArgumentException("Número vazio.");

			CEP = cepDigits;
			Estado = estado;
			Cidade = cidade.Trim();
			Rua = rua.Trim();
			Bairro = bairro.Trim();
			Numero = numero.Trim();
			Complemento = complemento?.Trim() ?? string.Empty;
			SemNumero = semNumero;
		}

	}
}
