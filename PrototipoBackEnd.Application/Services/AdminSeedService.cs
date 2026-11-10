using MongoDB.Driver;
using PrototipoBackEnd.Application.Configurations;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.ValueObjects;

namespace PrototipoBackEnd.Application.Services
{
    public class AdminSeedService : IAdminSeedService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ISenhaService _senhaService;
        private readonly AdminSeedOptions _options;

        public AdminSeedService(
            IUsuarioRepository usuarioRepository,
            IPessoaRepository pessoaRepository,
            ISenhaService senhaService,
            AdminSeedOptions options)
        {
            _usuarioRepository = usuarioRepository;
            _pessoaRepository = pessoaRepository;
            _senhaService = senhaService;
            _options = options;
        }

        public async Task<bool> ExecutarAsync()
        {
            if (string.IsNullOrWhiteSpace(_options.Email) || string.IsNullOrWhiteSpace(_options.Senha))
                return false;

            if (_options.Senha.Length < 8)
                throw new InvalidOperationException("AdminSeed:Senha deve ter pelo menos 8 caracteres.");

            var existente = await _usuarioRepository.ObterUsuarioPorEmail(_options.Email);
            if (existente != null)
                return false;

            // Dados de Pessoa obrigatórios no domínio; são placeholders e podem ser editados depois.
            var pessoa = Pessoa.Criar(
                string.IsNullOrWhiteSpace(_options.Nome) ? "Administrador" : _options.Nome,
                new DateTime(1990, 1, 1),
                new Endereco("00000000", "SP", "Não informado", "Não informado", "Não informado", "0", null, true),
                new Contato("00000000", "00000000", _options.Email));
            pessoa.VincularUsuario();

            var usuario = Usuario.Criar(pessoa.Id, _options.Email, _senhaService.CriarHash(_options.Senha), UsuarioEnum.Administrador);

            await _pessoaRepository.Adicionar(pessoa);

            try
            {
                await _usuarioRepository.Adicionar(usuario);
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                // Outra instância criou o admin ao mesmo tempo: remove a Pessoa órfã e segue.
                await _pessoaRepository.Deletar(pessoa.Id);
                return false;
            }

            return true;
        }
    }
}
