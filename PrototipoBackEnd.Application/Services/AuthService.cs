using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Application.Dtos.Auth;
using PrototipoBackEnd.Application.Exceptions;

namespace PrototipoBackEnd.Application.Services
{
    public class AuthService : IAuthService
    {
        #region Construtor
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly ISenhaService _senhaService;
        private readonly IPessoaRepository _pessoaRepository;

        public AuthService(IUsuarioRepository usuarioRepository, ITokenService tokenService, ISenhaService senhaService, IPessoaRepository pessoaRepository)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _senhaService = senhaService;
            _pessoaRepository = pessoaRepository;
        }
        #endregion

        public async Task<LoginResponseDto> Login(LoginRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
                throw new ValidationException("E-mail e senha são obrigatórios.");

            var usuario = await _usuarioRepository.ObterUsuarioPorEmail(request.Email);

            // Mensagem genérica: não revela se o e-mail existe ou se a senha está errada.
            if (usuario == null || !usuario.IsAtivo || !_senhaService.VerificarHash(request.Senha, usuario.SenhaHash))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            var pessoa = await _pessoaRepository.BuscarPorId(usuario.PessoaId);

            return new LoginResponseDto
            {
                Token = _tokenService.GenerateToken(usuario),
                Nome = pessoa?.NomeCompleto ?? string.Empty,
                Email = usuario.Email,
                Role = usuario.Role.ToString()
            };
        }

    }
}
