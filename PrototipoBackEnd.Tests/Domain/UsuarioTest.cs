using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace PrototipoBackEnd.UnitTests.Domain;

public class UsuarioTest
{
	#region Validações / Sucesso
	[Fact]
	public void CriarUsuario_Valido_DeveCriarComSucesso()
	{
		// Arrange
		var nome = "João Silva";
		var email = "TEST@EXAMPLE.COM ";
		var senhaHash = "hash123";
		Enum role = UsuarioEnum.Usuario;

		//Act
		var usuario = Usuario.Criar(nome, email, senhaHash, role, true);

		//Assert
		usuario.Should().NotBeNull();
		usuario.Id.Should().NotBeNullOrWhiteSpace();
		usuario.Nome.Should().Be("João Silva");
		usuario.Email.Should().Be("test@example.com");
		usuario.SenhaHash.Should().Be(senhaHash);
		usuario.Role.Should().Be(UsuarioEnum.Usuario);
		usuario.IsAtivo.Should().BeTrue(); // método define true independente do parâmetro passado
	}
	#endregion

	#region Validações / Falhas

	[Theory(DisplayName = "Criar Usuario com nome inválido lança ArgumentException")]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void CriarUsuario_NomeInvalido_DeveLancarArgumentException(string nomeInvalido)
	{
		// Act
		Action act = () => Usuario.Criar(nomeInvalido, "a@b.com", "hash", UsuarioEnum.Usuario, true);

		// Assert
		act.Should().Throw<ArgumentException>().WithMessage("*Nome*");
	}

	#endregion
}
