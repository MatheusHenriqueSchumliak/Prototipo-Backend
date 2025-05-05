namespace PrototipoBackEnd.Domain.Interfaces.Repositories
{
	public interface IGenericRepository<T> where T : class
	{
		Task<IEnumerable<T>> BuscarTodos();
		Task<T> BuscarPorId(string id);
		Task<T> Adicionar(T entity);
		Task<T> Atualizar(T entity, string id);
		Task<bool> Deletar(string id);
	}
}
