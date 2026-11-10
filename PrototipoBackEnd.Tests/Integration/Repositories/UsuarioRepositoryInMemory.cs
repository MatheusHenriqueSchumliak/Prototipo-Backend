using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Interfaces;

namespace PrototipoBackEnd.Tests.Integration.Repositories
{
    public class UsuarioRepositoryInMemory : IUsuarioRepository
    {
        private readonly List<Usuario> _usuarios = new();

        public Task<Usuario> Adicionar(Usuario entity)
        {
            _usuarios.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<IEnumerable<Usuario>> BuscarTodos()
            => Task.FromResult(_usuarios.ToList().AsEnumerable());

        public Task<Usuario> BuscarPorId(string id)
        {
            if (!Guid.TryParse(id, out _))
                throw new ArgumentException("ID inválido.");

            return Task.FromResult(_usuarios.FirstOrDefault(u => u.Id == id)!);
        }

        public Task<Usuario> Atualizar(Usuario entity, string id)
        {
            var index = _usuarios.FindIndex(u => u.Id == id);
            if (index >= 0)
                _usuarios[index] = entity;
            return Task.FromResult(entity);
        }

        public Task<bool> Deletar(string id)
            => Task.FromResult(_usuarios.RemoveAll(u => u.Id == id) > 0);

        public Task<Usuario> ObterUsuarioPorEmail(string email)
            => Task.FromResult(_usuarios.FirstOrDefault(u => u.Email == email.Trim().ToLowerInvariant())!);

        public Task<Usuario?> BuscarPorPessoaId(string pessoaId)
            => Task.FromResult(_usuarios.FirstOrDefault(u => u.PessoaId == pessoaId && u.IsAtivo));

        public Task GarantirIndicesAsync() => Task.CompletedTask;
    }
}
