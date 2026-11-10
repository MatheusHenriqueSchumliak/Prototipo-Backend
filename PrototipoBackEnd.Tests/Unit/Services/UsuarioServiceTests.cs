using Moq;
using PrototipoBackEnd.Application.Dtos.Usuario;
using PrototipoBackEnd.Application.Exceptions;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Tests.Integration.Repositories;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repo = new();
        private readonly Mock<IPessoaRepository> _pessoas = new();
        private readonly SenhaService _senhaService = new();
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _repo.Setup(r => r.Atualizar(It.IsAny<Usuario>(), It.IsAny<string>()))
                 .ReturnsAsync((Usuario u, string _) => u);
            _repo.Setup(r => r.Adicionar(It.IsAny<Usuario>()))
                 .ReturnsAsync((Usuario u) => u);
            _pessoas.Setup(r => r.BuscarPorId(It.IsAny<string>()))
                 .ReturnsAsync((string id) => PessoaTestes.Criar(id));
            _pessoas.Setup(r => r.Atualizar(It.IsAny<Pessoa>(), It.IsAny<string>()))
                 .ReturnsAsync((Pessoa p, string _) => p);

            _service = new UsuarioService(_repo.Object, _pessoas.Object, _senhaService, new UnitOfWorkFake());
        }

        private Usuario UsuarioExistente(string email = "existente@teste.com", UsuarioEnum role = UsuarioEnum.Usuario)
        {
            var usuario = Usuario.Criar(Guid.NewGuid().ToString(), email, _senhaService.CriarHash("SenhaAntiga1"), role);
            _repo.Setup(r => r.BuscarPorId(usuario.Id)).ReturnsAsync(usuario);
            _repo.Setup(r => r.ObterUsuarioPorEmail(email)).ReturnsAsync(usuario);
            return usuario;
        }

        [Fact]
        public async Task Adicionar_NormalizaEmail_GeraHashBCrypt_ERespeitaRoleDoRequest()
        {
            Usuario? salvo = null;
            _repo.Setup(r => r.Adicionar(It.IsAny<Usuario>())).Callback<Usuario>(u => salvo = u).ReturnsAsync((Usuario u) => u);

            var resposta = await _service.Adicionar(new UsuarioRequestDto
            {
                PessoaId = Guid.NewGuid().ToString(),
                Email = "  Maria@Teste.COM ",
                Senha = "SenhaForte123",
                Role = UsuarioEnum.Usuario
            });

            Assert.NotNull(salvo);
            Assert.Equal("maria@teste.com", salvo!.Email);
            Assert.Equal(UsuarioEnum.Usuario, salvo.Role);
            Assert.NotEqual("SenhaForte123", salvo.SenhaHash);
            Assert.True(BCrypt.Net.BCrypt.Verify("SenhaForte123", salvo.SenhaHash));
            Assert.Equal("maria@teste.com", resposta.Email);
        }

        [Fact]
        public async Task Adicionar_EmailJaCadastrado_LancaConflict()
        {
            UsuarioExistente("dup@teste.com");

            await Assert.ThrowsAsync<ConflictException>(() => _service.Adicionar(new UsuarioRequestDto
            {
                PessoaId = Guid.NewGuid().ToString(),
                Email = "dup@teste.com",
                Senha = "SenhaForte123"
            }));

            _repo.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task Atualizar_SemSenha_MantemHash_IdEDataCriacao()
        {
            var usuario = UsuarioExistente();
            var id = usuario.Id;
            var criadoEm = usuario.DataCriacao;
            var hash = usuario.SenhaHash;

            await _service.Atualizar(new UsuarioAtualizarRequestDto { Email = "novo@teste.com", Role = UsuarioEnum.Administrador }, id);

            Assert.Equal(id, usuario.Id);
            Assert.Equal(criadoEm, usuario.DataCriacao);
            Assert.Equal(hash, usuario.SenhaHash);
            Assert.Equal("novo@teste.com", usuario.Email);
            Assert.Equal(UsuarioEnum.Administrador, usuario.Role);
            Assert.NotNull(usuario.DataAtualizacao);
        }

        [Fact]
        public async Task Atualizar_ComSenha_TrocaHash()
        {
            var usuario = UsuarioExistente();

            await _service.Atualizar(new UsuarioAtualizarRequestDto { Email = usuario.Email, Role = usuario.Role, Senha = "NovaSenha123" }, usuario.Id);

            Assert.True(BCrypt.Net.BCrypt.Verify("NovaSenha123", usuario.SenhaHash));
        }

        [Fact]
        public async Task Atualizar_EmailDeOutroUsuario_LancaConflict()
        {
            UsuarioExistente("outro@teste.com");
            var usuario = UsuarioExistente("meu@teste.com");

            await Assert.ThrowsAsync<ConflictException>(() =>
                _service.Atualizar(new UsuarioAtualizarRequestDto { Email = "outro@teste.com", Role = usuario.Role }, usuario.Id));

            _repo.Verify(r => r.Atualizar(It.IsAny<Usuario>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Atualizar_MesmoEmailDoProprioUsuario_NaoConflita()
        {
            var usuario = UsuarioExistente("proprio@teste.com");

            await _service.Atualizar(new UsuarioAtualizarRequestDto { Email = "PROPRIO@teste.com", Role = usuario.Role }, usuario.Id);

            _repo.Verify(r => r.Atualizar(usuario, usuario.Id), Times.Once);
        }
    }
}
