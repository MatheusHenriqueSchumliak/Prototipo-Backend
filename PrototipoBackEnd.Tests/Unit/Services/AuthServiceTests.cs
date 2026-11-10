using Moq;
using PrototipoBackEnd.Application.Dtos.Auth;
using PrototipoBackEnd.Application.Exceptions;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Interfaces.Services;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repo = new();
        private readonly Mock<ITokenService> _token = new();
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _service = new AuthService(_repo.Object, _token.Object, new SenhaService(), new Mock<PrototipoBackEnd.Domain.Interfaces.Repositories.IPessoaRepository>().Object);
        }

        [Theory]
        [InlineData("", "senha")]
        [InlineData("a@b.com", "")]
        [InlineData("  ", "  ")]
        public async Task Login_CamposAusentes_LancaValidation(string email, string senha)
        {
            await Assert.ThrowsAsync<ValidationException>(() => _service.Login(new LoginRequestDto { Email = email, Senha = senha }));
        }

        [Fact]
        public async Task Login_UsuarioInexistenteOuSenhaErrada_LancaMesmaMensagemGenerica()
        {
            var usuario = Usuario.Criar(Guid.NewGuid().ToString(), "a@b.com", new SenhaService().CriarHash("correta123"), UsuarioEnum.Usuario);
            _repo.Setup(r => r.ObterUsuarioPorEmail("a@b.com")).ReturnsAsync(usuario);
            _repo.Setup(r => r.ObterUsuarioPorEmail("x@b.com")).ReturnsAsync((Usuario)null!);

            var inexistente = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Login(new LoginRequestDto { Email = "x@b.com", Senha = "qualquer" }));
            var errada = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.Login(new LoginRequestDto { Email = "a@b.com", Senha = "errada" }));

            Assert.Equal(inexistente.Message, errada.Message);
        }

        [Fact]
        public async Task Login_Valido_RetornaTokenEEmailERole()
        {
            var usuario = Usuario.Criar(Guid.NewGuid().ToString(), "a@b.com", new SenhaService().CriarHash("correta123"), UsuarioEnum.Administrador);
            _repo.Setup(r => r.ObterUsuarioPorEmail("a@b.com")).ReturnsAsync(usuario);
            _token.Setup(t => t.GenerateToken(usuario)).Returns("jwt");

            var resposta = await _service.Login(new LoginRequestDto { Email = "a@b.com", Senha = "correta123" });

            Assert.Equal("jwt", resposta.Token);
            Assert.Equal("Administrador", resposta.Role);
            Assert.Equal("a@b.com", resposta.Email);
        }
    }
}
