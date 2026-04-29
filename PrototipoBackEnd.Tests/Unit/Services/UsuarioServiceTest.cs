using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Domain.Enumerables;
using PrototipoBackEnd.Domain.Interfaces;
using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Domain.Entities;
using FluentAssertions;
using AutoMapper;
using Xunit;
using Moq;

namespace PrototipoBackEnd.UnitTests.Unit.Services;

public class UsuarioServiceTest
{
	private readonly Mock<ISenhaService> _senhaService = new();
	private readonly Mock<IUsuarioRepository> _repo = new();
	private readonly Mock<IMapper> _mapper = new();

	private UsuarioService CreateSut() => new(_repo.Object, _senhaService.Object, _mapper.Object);

	private static Usuario NewUsuario(string id, string nome, string email, string senhaHash) =>
		new() { Id = id, Nome = nome, Email = email, SenhaHash = senhaHash, Role = UsuarioEnum.Usuario, IsAtivo = true };

	private static UsuarioDto NewUsuarioDto(string id, string nome, string email, string senha) =>
		new() { Id = id, Nome = nome, Email = email, SenhaHash = senha, Role = UsuarioEnum.Usuario, IsAtivo = true };

	[Fact]
	public async Task BuscarTodos_DeveRetornarDtos()
	{
		// Arrange
		var usuarios = new List<Usuario> { NewUsuario(Guid.NewGuid().ToString(), "N", "e@e", "h") };
		var dtos = new List<UsuarioDto> { NewUsuarioDto(usuarios[0].Id, usuarios[0].Nome, usuarios[0].Email, usuarios[0].SenhaHash) };

		_repo.Reset();
		_repo.Setup(r => r.BuscarTodos()).ReturnsAsync(usuarios);
		_mapper.Setup(m => m.Map<List<UsuarioDto>>(It.IsAny<IEnumerable<Usuario>>())).Returns(dtos);

		var sut = CreateSut();

		// Act
		var result = await sut.BuscarTodos();

		// Assert
		result.Should().BeEquivalentTo(dtos);
		_repo.Verify(r => r.BuscarTodos(), Times.Once);
		_mapper.Verify(m => m.Map<List<UsuarioDto>>(It.IsAny<IEnumerable<Usuario>>()), Times.Once);
	}

	[Fact]
	public async Task BuscarPorId_QuandoEncontrado_DeveRetornarDto()
	{
		var id = Guid.NewGuid().ToString();
		var usuario = NewUsuario(id, "N", "e@e", "h");
		var dto = NewUsuarioDto(id, usuario.Nome, usuario.Email, usuario.SenhaHash);

		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync(usuario);
		_mapper.Setup(m => m.Map<UsuarioDto>(usuario)).Returns(dto);

		var sut = CreateSut();

		var result = await sut.BuscarPorId(id);

		result.Should().BeEquivalentTo(dto);
	}

	[Fact]
	public async Task BuscarPorId_QuandoNaoEncontrado_DeveLancarException()
	{
		var id = Guid.NewGuid().ToString();
		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync((Usuario?)null);

		var sut = CreateSut();

		await Assert.ThrowsAsync<Exception>(async () => await sut.BuscarPorId(id));
	}

	[Fact]
	public async Task Adicionar_DeveCriarHashEChamarRepositorio()
	{
		var dto = NewUsuarioDto(null!, "Nome", "email@x", "plain");
		_senhaService.Setup(s => s.CriarHash("plain")).Returns("hashed");
		_repo.Setup(r => r.Adicionar(It.IsAny<Usuario>())).ReturnsAsync((Usuario u) => u);

		var sut = CreateSut();

		await sut.Adicionar(dto);

		_senhaService.Verify(s => s.CriarHash("plain"), Times.Once);
		_repo.Verify(r => r.Adicionar(It.Is<Usuario>(u => u.SenhaHash == "hashed" && u.Email == dto.Email)), Times.Once);
	}

	[Fact]
	public async Task Atualizar_QuandoIdNulo_DeveLancar()
	{
		var sut = CreateSut();
		var dto = NewUsuarioDto(null!, "N", "e", "s");

		await Assert.ThrowsAsync<Exception>(async () => await sut.Atualizar(dto, null!));
	}

	[Fact]
	public async Task Atualizar_QuandoNaoEncontrado_DeveLancar()
	{
		var id = Guid.NewGuid().ToString();
		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync((Usuario?)null);
		var sut = CreateSut();
		var dto = NewUsuarioDto(id, "N", "e", "s");

		await Assert.ThrowsAsync<Exception>(async () => await sut.Atualizar(dto, id));
	}

	[Fact]
	public async Task Atualizar_QuandoSenhaMudou_DeveCriarHashEAtualizarRepositorio()
	{
		var id = Guid.NewGuid().ToString();
		var existing = NewUsuario(id, "N", "e@e", "oldhash");
		var dto = NewUsuarioDto(id, "N", "e@e", "newpass");

		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync(existing);
		_senhaService.Setup(s => s.CriarHash("newpass")).Returns("newhash");
		_mapper.Setup(m => m.Map<Usuario>(dto)).Returns(new Usuario { Id = id, Nome = dto.Nome, Email = dto.Email, SenhaHash = "newhash", Role = dto.Role, IsAtivo = dto.IsAtivo });
		_repo.Setup(r => r.Atualizar(It.IsAny<Usuario>(), id)).ReturnsAsync((Usuario u, string _) => u);

		var sut = CreateSut();

		await sut.Atualizar(dto, id);

		_senhaService.Verify(s => s.CriarHash("newpass"), Times.Once);
		_repo.Verify(r => r.Atualizar(It.Is<Usuario>(u => u.SenhaHash == "newhash"), id), Times.Once);
	}

	[Fact]
	public async Task Apagar_QuandoIdNulo_DeveLancar()
	{
		var sut = CreateSut();
		await Assert.ThrowsAsync<Exception>(async () => await sut.Apagar(null!));
	}

	[Fact]
	public async Task Apagar_DeveChamarRepositorioERetornarTrue()
	{
		var id = Guid.NewGuid().ToString();
		_repo.Setup(r => r.Deletar(id)).ReturnsAsync(true);
		var sut = CreateSut();

		var result = await sut.Apagar(id);

		result.Should().BeTrue();
		_repo.Verify(r => r.Deletar(id), Times.Once);
	}
}