namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class Midia
	{
		public IReadOnlyList<string> Imagens { get; }
		public Midia(IEnumerable<string> imagens)
		{
			if (imagens == null)
				throw new ArgumentException("Imagens não pode ser nulo.");

			var lista = imagens
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Trim())
				.ToList();

			if (lista.Count == 0)
				throw new ArgumentException("Pelo menos uma imagem é obrigatória.");

			if (lista.Count > 5)
				throw new ArgumentException("Máximo de 10 imagens permitido.");

			Imagens = lista;
		}

	}
}
