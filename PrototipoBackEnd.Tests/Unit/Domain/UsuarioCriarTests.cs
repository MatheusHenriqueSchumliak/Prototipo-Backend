using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Enumerables;
using Xunit;

namespace PrototipoBackEnd.Tests.Unit.Domain
{
    public class UsuarioCriarTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("nao-e-guid")]
        public void Criar_PessoaIdInvalido_Falha(string pessoaId)
        {
            Assert.Throws<ArgumentException>(() => Usuario.Criar(pessoaId, "a@b.com", "hash", UsuarioEnum.Usuario));
        }

        [Fact]
        public void Criar_PessoaIdValido_MantemOValorInformado()
        {
            var pessoaId = Guid.NewGuid().ToString();

            var usuario = Usuario.Criar(pessoaId, "A@B.com", "hash", UsuarioEnum.Usuario);

            Assert.Equal(pessoaId, usuario.PessoaId);
            Assert.Equal("a@b.com", usuario.Email);
        }
    }
}
