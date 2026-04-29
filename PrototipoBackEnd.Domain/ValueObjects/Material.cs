namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class Material
	{
		public IReadOnlyList<string> Materiais { get; }
		public Material(IEnumerable<string> materiais)
		{
			if (materiais == null)
				throw new ArgumentException("Material não pode ser nulo.");

			var lista = materiais
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Trim().ToLower())
				.Distinct()
				.ToList();

			if (lista.Count == 0)
				throw new ArgumentException("Pelo menos um material é obrigatório.");

			Materiais = lista;
		}
	}
}
