namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class Material
	{
		public IReadOnlyList<string> Materiais { get; private set; } = new List<string>();
		private Material() { }

		public Material(IEnumerable<string> listaMateriais)
		{
			if (listaMateriais == null)
				throw new ArgumentException("Material não pode ser nulo.");

			var lista = listaMateriais
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
