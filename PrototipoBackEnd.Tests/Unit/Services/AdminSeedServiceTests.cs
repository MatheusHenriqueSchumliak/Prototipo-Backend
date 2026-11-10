using Moq;
using PrototipoBackEnd.Application.Configurations;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Services
{
    public class AdminSeedServiceTests
    {
        private readonly Mock<IUsuarioRepository> _usuarios = new();
        private readonly Mock<IPessoaRepository> _pessoas = new();

        private AdminSeedService Criar(AdminSeedOptions options)
            => new(_usuarios.Object, _pessoas.Object, new SenhaService(), options);

        [Theory]
        [InlineData(null, null)]
        [InlineData("admin@teste.com", null)]
        [InlineData(null, "SenhaForte123")]
        public async Task SemConfiguracao_NaoFazNada(string? email, string? senha)
        {
            var criado = await Criar(new AdminSeedOptions { Email = email, Senha = senha }).ExecutarAsync();

            Assert.False(criado);
            _usuarios.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _pessoas.Verify(r => r.Adicionar(It.IsAny<Pessoa>()), Times.Never);
        }

        [Fact]
        public async Task ComConfiguracao_CriaPessoaVinculadaEAdminComHash()
        {
            Pessoa? pessoa = null;
            Usuario? usuario = null;
            _pessoas.Setup(r => r.Adicionar(It.IsAny<Pessoa>())).Callback<Pessoa>(p => pessoa = p).ReturnsAsync((Pessoa p) => p);
            _usuarios.Setup(r => r.Adicionar(It.IsAny<Usuario>())).Callback<Usuario>(u => usuario = u).ReturnsAsync((Usuario u) => u);
            _usuarios.Setup(r => r.ObterUsuarioPorEmail(It.IsAny<string>())).ReturnsAsync((Usuario)null!);

            var criado = await Criar(new AdminSeedOptions { Email = "Admin@Teste.com", Senha = "SenhaForte123", Nome = "Chefe" }).ExecutarAsync();

            Assert.True(criado);
            Assert.NotNull(pessoa);
            Assert.NotNull(usuario);
            Assert.Equal("Chefe", pessoa!.NomeCompleto);
            Assert.True(pessoa.TemUsuario);
            Assert.Equal(pessoa.Id, usuario!.PessoaId);
            Assert.Equal(UsuarioEnum.Administrador, usuario.Role);
            Assert.Equal("admin@teste.com", usuario.Email);
            Assert.True(BCrypt.Net.BCrypt.Verify("SenhaForte123", usuario.SenhaHash));
        }

        [Fact]
        public async Task AdminJaExistente_EIdempotente()
        {
            var existente = Usuario.Criar(Guid.NewGuid().ToString(), "admin@teste.com", "hash", UsuarioEnum.Administrador);
            _usuarios.Setup(r => r.ObterUsuarioPorEmail("admin@teste.com")).ReturnsAsync(existente);

            var criado = await Criar(new AdminSeedOptions { Email = "admin@teste.com", Senha = "SenhaForte123" }).ExecutarAsync();

            Assert.False(criado);
            _usuarios.Verify(r => r.Adicionar(It.IsAny<Usuario>()), Times.Never);
            _pessoas.Verify(r => r.Adicionar(It.IsAny<Pessoa>()), Times.Never);
        }

        [Fact]
        public async Task SenhaCurta_FalhaNaInicializacao()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                Criar(new AdminSeedOptions { Email = "admin@teste.com", Senha = "curta" }).ExecutarAsync());
        }
    }
}
