namespace PrototipoBackEnd.Domain.ValueObjects
{
	public class Especialidade
	{
		public IReadOnlyList<string> Itens { get; }

		public Especialidade(IEnumerable<string> especialidades)
		{
			if (especialidades == null)
				throw new ArgumentException("Especialidades não pode ser nulo.");

			var lista = especialidades
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Select(x => x.Trim().ToLower())
				.Distinct()
				.ToList();

			if (lista.Count == 0)
				throw new ArgumentException("Pelo menos uma especialidade é obrigatória.");

			if (lista.Count > 10)
				throw new ArgumentException("Máximo de 5 especialidades permitido.");

			Itens = lista;
		}
	}
}
