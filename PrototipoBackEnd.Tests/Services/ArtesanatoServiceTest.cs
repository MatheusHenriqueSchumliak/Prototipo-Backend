using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Application.Services;
using PrototipoBackEnd.Domain.Entities;
using PrototipoBackEnd.Domain.Interfaces.Repositories;
using PrototipoBackEnd.Domain.Interfaces.Services;
using Xunit;

namespace PrototipoBackEnd.UnitTests.Services;

public class ArtesanatoServiceTest
{
	private readonly Mock<IArtesanatoRepository> _repo = new();
	private readonly Mock<IAmazonS3Service> _s3 = new();
	private readonly Mock<IMapper> _mapper = new();

	private ArtesanatoService CreateSut() => new(_repo.Object, _s3.Object, _mapper.Object);

	private static Artesanato NewArtesanato(string id, string usuarioId)
		=> new()
		{
			Id = id,
			UsuarioId = usuarioId,
			TituloArtesanato = "Objeto X",
			DescricaoArtesanato = "Descrição",
			Preco = 10m,
			QuantidadeArtesanato = 1,
			ImagemUrl = new List<string> { "https://old/img1.jpg" }
		};

	private static ArtesanatoDto NewDto(string usuarioId)
		=> new()
		{
			Id = Guid.NewGuid().ToString(),
			UsuarioId = usuarioId,
			TituloArtesanato = "Objeto X",
			DescricaoArtesanato = "Descrição",
			Preco = 10m,
			QuantidadeArtesanato = 1
		};

	private static IFormFile BuildFormFile(string fileName, string contentType, byte[] content)
	{
		var ms = new MemoryStream(content);
		var fileMock = new Mock<IFormFile>();
		fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
		fileMock.SetupGet(f => f.Length).Returns(ms.Length);
		fileMock.SetupGet(f => f.FileName).Returns(fileName);
		fileMock.SetupGet(f => f.ContentType).Returns(contentType);
		return fileMock.Object;
	}

	[Fact(DisplayName = "BuscarTodos retorna dtos")]
	public async Task BuscarTodos_DeveRetornarDtos()
	{
		var artesanatos = new List<Artesanato> { NewArtesanato(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) };
		var dtos = new List<ArtesanatoDto> { new() { Id = artesanatos[0].Id, UsuarioId = artesanatos[0].UsuarioId } };

		_repo.Setup(r => r.BuscarTodos()).ReturnsAsync(artesanatos);
		_mapper.Setup(m => m.Map<List<ArtesanatoDto>>(artesanatos)).Returns(dtos);

		var sut = CreateSut();

		var result = await sut.BuscarTodos();

		result.Should().BeEquivalentTo(dtos);
		_repo.Verify(r => r.BuscarTodos(), Times.Once);
		_mapper.Verify(m => m.Map<List<ArtesanatoDto>>(artesanatos), Times.Once);
	}

	[Fact(DisplayName = "BuscarPorId quando encontrado retorna dto")]
	public async Task BuscarPorId_Encontrado_DeveRetornarDto()
	{
		var id = Guid.NewGuid().ToString();
		var artesanato = NewArtesanato(id, Guid.NewGuid().ToString());
		var dto = new ArtesanatoDto { Id = id, UsuarioId = artesanato.UsuarioId };

		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync(artesanato);
		_mapper.Setup(m => m.Map<ArtesanatoDto>(artesanato)).Returns(dto);

		var sut = CreateSut();

		var result = await sut.BuscarPorId(id);

		result.Should().BeEquivalentTo(dto);
	}

	[Fact(DisplayName = "BuscarPorId quando não encontrado lança exceção")]
	public async Task BuscarPorId_NaoEncontrado_DeveLancar()
	{
		var id = Guid.NewGuid().ToString();
		_repo.Setup(r => r.BuscarPorId(id)).ReturnsAsync((Artesanato?)null);

		var sut = CreateSut();

		await Assert.ThrowsAsync<Exception>(() => sut.BuscarPorId(id));
	}

	[Fact(DisplayName = "BuscarPorArtesaoId quando encontrado retorna dto")]
	public async Task BuscarPorArtesaoId_Encontrado_DeveRetornarDto()
	{
		var artesaoId = Guid.NewGuid().ToString();
		var artesanato = NewArtesanato(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
		var dto = new ArtesanatoDto { Id = artesanato.Id, UsuarioId = artesanato.UsuarioId };

		_repo.Setup(r => r.BuscarPorArtesaoId(artesaoId)).ReturnsAsync(artesanato);
		_mapper.Setup(m => m.Map<ArtesanatoDto>(artesanato)).Returns(dto);

		var sut = CreateSut();

		var result = await sut.BuscarPorArtesaoId(artesaoId);

		result.Should().BeEquivalentTo(dto);
	}

	[Fact(DisplayName = "BuscarTodosPorArtesaoId retorna lista vazia quando não há itens")]
	public async Task BuscarTodosPorArtesaoId_RetornaListaVazia()
	{
		var artesaoId = Guid.NewGuid().ToString();
		_repo.Setup(r => r.BuscarTodosPorArtesaoId(artesaoId)).ReturnsAsync(new List<Artesanato>());

		var sut = CreateSut();

		var result = await sut.BuscarTodosPorArtesaoId(artesaoId);

		result.Should().BeEmpty();
		_repo.Verify(r => r.BuscarTodosPorArtesaoId(artesaoId), Times.Once);
	}

	[Fact(DisplayName = "Adicionar com imagens faz upload, adiciona e retorna dto")]
	public async Task Adicionar_ComImagens_DeveFazerUploadEAdicionar()
	{
		var usuarioId = Guid.NewGuid().ToString();
		var dto = NewDto(usuarioId);
		var files = new List<IFormFile>
		{
			BuildFormFile("img1.jpg", "image/jpeg", new byte[] {1,2}),
			BuildFormFile("img2.png", "image/png", new byte[] {3,4})
		};

		var urls = new List<string> { "https://s3/img1.jpg", "https://s3/img2.png" };

		// Configura upload para retornar cada url
		_s3.SetupSequence(s => s.Upload(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
			.ReturnsAsync(urls[0])
			.ReturnsAsync(urls[1]);

		// O mapper deve transformar dto em entidade e entidade em dto
		_mapper.Setup(m => m.Map<Artesanato>(It.IsAny<ArtesanatoDto>()))
			.Returns((ArtesanatoDto d) => new Artesanato { Id = d.Id, UsuarioId = d.UsuarioId, ImagemUrl = d.ImagemUrl });
		// Corrigido: retornar Task<Artesanato> corretamente
		_repo.Setup(r => r.Adicionar(It.IsAny<Artesanato>())).ReturnsAsync((Artesanato a) => a);
		_mapper.Setup(m => m.Map<ArtesanatoDto>(It.IsAny<Artesanato>()))
			.Returns((Artesanato a) => new ArtesanatoDto { Id = a.Id, UsuarioId = a.UsuarioId, ImagemUrl = a.ImagemUrl });

		var sut = CreateSut();

		var result = await sut.Adicionar(dto, files);

		_s3.Verify(s => s.Upload(It.IsAny<Stream>(), "img1.jpg", "image/jpeg"), Times.Once);
		_s3.Verify(s => s.Upload(It.IsAny<Stream>(), "img2.png", "image/png"), Times.Once);
		_repo.Verify(r => r.Adicionar(It.IsAny<Artesanato>()), Times.Once);

		result.ImagemUrl.Should().BeEquivalentTo(urls);
	}

	[Fact(DisplayName = "Apagar chama repositório e retorna true")]
	public async Task Apagar_DeveChamarRepositorioERetornarTrue()
	{
		var id = Guid.NewGuid().ToString();
		// Corrigido: retornar Task<bool> corretamente
		_repo.Setup(r => r.Deletar(id)).ReturnsAsync(true);

		var sut = CreateSut();

		var result = await sut.Apagar(id);

		result.Should().BeTrue();
		_repo.Verify(r => r.Deletar(id), Times.Once);
	}
}