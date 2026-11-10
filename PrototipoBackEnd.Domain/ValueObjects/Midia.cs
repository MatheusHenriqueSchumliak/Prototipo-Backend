namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class Midia
	{
		public IReadOnlyList<string> Imagens { get; private set; } = new List<string>();
		private Midia() { }

		public Midia(IEnumerable<string> urlsImagens)
		{
			if (urlsImagens == null)
				throw new ArgumentException("Imagens não pode ser nulo.");

			var lista = urlsImagens
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Trim())
				.ToList();

			if (lista.Count == 0)
				throw new ArgumentException("Pelo menos uma imagem é obrigatória.");

			if (lista.Count > 5)
				throw new ArgumentException("Máximo de 5 urlsImagens permitido.");

			Imagens = lista;
		}

	}
}
