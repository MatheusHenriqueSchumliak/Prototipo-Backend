using MongoDB.Driver;
using PrototipoBackEnd.Application.Common.Helpers;
using PrototipoBackEnd.Application.Dtos.Usuario;
using PrototipoBackEnd.Application.Exceptions;
using PrototipoBackEnd.Application.Factories;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Interfaces.Repositories;

namespace PrototipoBackEnd.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        #region Construtor
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ISenhaService _senhaService;
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(IUsuarioRepository usuarioRepository, IPessoaRepository pessoaRepository, ISenhaService senhaService, IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _pessoaRepository = pessoaRepository;
            _senhaService = senhaService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        public async Task<List<UsuarioResponseDto>> BuscarTodos()
        {
            var usuarios = await _usuarioRepository.BuscarTodos();
            return usuarios.Where(u => u.IsAtivo).Select(UsuarioFactory.CriarDto).ToList();
        }

        public async Task<UsuarioResponseDto> BuscarPorId(string id)
        {
            IdValidator.Validar(id);
            var usuario = await _usuarioRepository.BuscarPorId(id);

            if (usuario == null || !usuario.IsAtivo)
                throw new NotFoundException("Usuário não foi encontrado.");

            return UsuarioFactory.CriarDto(usuario);
        }

        public async Task<UsuarioResponseDto> Adicionar(UsuarioRequestDto dto)
        {
            IdValidator.Validar(dto.PessoaId, "PessoaId inválido.");
            var pessoa = await _pessoaRepository.BuscarPorId(dto.PessoaId);
            if (pessoa == null || !pessoa.IsAtivo)
                throw new NotFoundException("Pessoa não foi encontrada.");

            if (pessoa.TemUsuario)
                throw new ConflictException("Pessoa já possui usuário.");

            await GarantirEmailDisponivel(dto.Email, idIgnorado: null);

            var hash = _senhaService.CriarHash(dto.Senha);
            var usuario = Usuario.Criar(dto.PessoaId, dto.Email, hash, dto.Role);
            pessoa.VincularUsuario();

            await SalvarTratandoDuplicidade(() => _unitOfWork.ExecutarAsync(async () =>
            {
                await _usuarioRepository.Adicionar(usuario);
                await _pessoaRepository.Atualizar(pessoa, pessoa.Id);
            }));

            return UsuarioFactory.CriarDto(usuario);
        }

        public async Task Atualizar(UsuarioAtualizarRequestDto dto, string id)
        {
            IdValidator.Validar(id);
            var usuario = await _usuarioRepository.BuscarPorId(id);

            if (usuario == null || !usuario.IsAtivo)
                throw new NotFoundException("Usuário não foi encontrado.");

            // Somente campos explícitos: Id e DataCriacao nunca são tocados.
            var emailNormalizado = dto.Email.Trim().ToLowerInvariant();
            if (emailNormalizado != usuario.Email)
            {
                await GarantirEmailDisponivel(emailNormalizado, usuario.Id);
                usuario.AlterarEmail(emailNormalizado);
            }

            if (dto.Role != usuario.Role)
                usuario.AlterarRole(dto.Role);

            if (!string.IsNullOrWhiteSpace(dto.Senha))
                usuario.AlterarSenha(_senhaService.CriarHash(dto.Senha));

            await SalvarTratandoDuplicidade(() => _usuarioRepository.Atualizar(usuario, id));
        }

        public async Task<bool> Apagar(string id)
        {
            IdValidator.Validar(id);

            var usuario = await _usuarioRepository.BuscarPorId(id);
            if (usuario == null || !usuario.IsAtivo)
                throw new NotFoundException("Usuário não foi encontrado.");

            await _unitOfWork.ExecutarAsync(async () =>
            {
                usuario.Inativar();
                await _usuarioRepository.Atualizar(usuario, id);

                var pessoa = await _pessoaRepository.BuscarPorId(usuario.PessoaId);
                if (pessoa != null)
                {
                    pessoa.DesvincularUsuario();
                    await _pessoaRepository.Atualizar(pessoa, pessoa.Id);
                }
            });
            return true;
        }

        #region Métodos Privados
        private async Task GarantirEmailDisponivel(string email, string? idIgnorado)
        {
            var existente = await _usuarioRepository.ObterUsuarioPorEmail(email);

            if (existente != null && existente.Id != idIgnorado)
                throw new ConflictException("E-mail já cadastrado.");
        }

        // A checagem acima evita o caso comum; o índice único do Mongo cobre a corrida entre duas requisições.
        private static async Task SalvarTratandoDuplicidade(Func<Task> salvar)
        {
            try
            {
                await salvar();
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                throw new ConflictException("E-mail já cadastrado.");
            }
        }
        #endregion

    }
}
