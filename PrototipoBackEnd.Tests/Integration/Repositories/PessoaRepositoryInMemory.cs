using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.Tests.Integration.Repositories
{
	public class PessoaRepositoryInMemory : IPessoaRepository
	{
		private readonly List<Pessoa> _pessoas = new();

		public Task<Pessoa> Adicionar(Pessoa entity)
		{
			_pessoas.Add(entity);
			return Task.FromResult(entity);
		}

		public Task<IEnumerable<Pessoa>> BuscarTodos()
			=> Task.FromResult(_pessoas.AsEnumerable());

		public Task<Pessoa> BuscarPorId(string id)
			=> Task.FromResult(_pessoas.FirstOrDefault(p => p.Id == id));

		public Task<Pessoa> Atualizar(Pessoa entity, string id)
		{
			var index = _pessoas.FindIndex(p => p.Id == id);
			if (index >= 0)
				_pessoas[index] = entity;
			return Task.FromResult(entity);
		}

		public Task<bool> Deletar(string id)
		{
			var pessoa = _pessoas.FirstOrDefault(p => p.Id == id);
			if (pessoa != null)
			{
				_pessoas.Remove(pessoa);
				return Task.FromResult(true);
			}
			return Task.FromResult(false);
		}
	}
}
