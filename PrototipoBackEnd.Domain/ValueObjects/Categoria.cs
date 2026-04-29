namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class Categoria
	{
		public IReadOnlyList<string> Itens { get; }
		public Categoria(IEnumerable<string> categorias)
		{
			if (categorias == null)
				throw new ArgumentException("Categorias não pode ser nulo.");

			var lista = categorias
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Trim().ToLower())
				.Distinct()
				.ToList();

			if (lista.Count == 0)
				throw new ArgumentException("Pelo menos uma categoria é obrigatória.");

			Itens = lista;
		}
	}
}
