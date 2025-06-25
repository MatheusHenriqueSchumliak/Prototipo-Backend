using PrototipoBackEnd.Domain.Interfaces.Services;
using PrototipoBackEnd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using PrototipoBackEnd.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace PrototipoBackEnd.API.Controllers
{
	[Authorize(Policy = "Administrador")]
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ArtesaoController : ControllerBase
	{
		#region Construtor 

		private readonly IConfiguration _configuration;
		private readonly IArtesaoService _artesaoService;
		private readonly ILogger<ArtesaoController> _logger;
		private readonly IAmazonS3Service _amazonS3Service;

		public ArtesaoController(IConfiguration configuration, ILogger<ArtesaoController> logger, IArtesaoService artesaoService, IAmazonS3Service amazonS3Service)
		{
			_amazonS3Service = amazonS3Service;
			_configuration = configuration;
			_artesaoService = artesaoService;
			_logger = logger;
		}

		#endregion

		private static List<ArtesaoDto> artesaos = new List<ArtesaoDto>();

		#region Usuário CRUD PADRÃO

		// READ - Buscar todos os Artesãos (GET)
		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<List<ArtesaoDto>>> BuscarTodos(
			[FromQuery] string? nome = null,
			[FromQuery] string? nichoAtuacao = null,
			[FromQuery] bool? receberEncomendas = null,
			[FromQuery] bool? enviaEncomendas = null)
		{
			// Sempre chama o método com filtro, mesmo que todos os parâmetros sejam nulos
			var artesaos = await _artesaoService.BuscarComFiltro(nome, nichoAtuacao, receberEncomendas, enviaEncomendas);
			return Ok(artesaos);
		}

		// READ - Buscar Artesão por id (GET) {id}
		[AllowAnonymous]
		[HttpGet("{id}")]
		public async Task<ActionResult<ArtesaoDto>> BuscarPorId(string id)
		{
			try
			{
				// Chama o serviço para buscar o Artesão
				var artesao = await _artesaoService.BuscarPorId(id);

				if (artesao == null)
				{
					// Retorna NotFound se o Artesão não for encontrado
					return NotFound(new { message = $"Usuário com ID {id} não encontrado." });
				}

				// Retorna OK com os dados do Artesão encontrado
				return Ok(new { message = "Artesão encontrado", data = artesao });
			}
			catch (Exception ex)
			{
				// Caso ocorra algum erro, retorna BadRequest com a mensagem de erro
				return BadRequest(new { message = ex.Message });
			}
		}

		// CREATE - Adicionar um novo Artesão (POST)
		[HttpPost]
		public async Task<ActionResult<ArtesaoDto>> Adicionar([FromForm] ArtesaoDto dto, IFormFile imagem)
		{
			try
			{
				var result = await _artesaoService.Adicionar(dto, imagem);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// UPDATE - Atualizar um Artesão existente (PUT)
		[HttpPut("{id}")]
		public async Task<ActionResult<ArtesaoDto>> Atualizar([FromForm] ArtesaoDto dto, string id, IFormFile? imagem = null)
		{
			// Valida o modelo
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _artesaoService.Atualizar(dto, id, imagem);
				return Ok(new { message = "Usuário atualizado com sucesso" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// DELETE - Excluir um Artesão existente (DELETE)
		[HttpDelete("{id}")]
		public async Task<ActionResult<ArtesaoDto>> Excluir(string id)
		{
			// Simula uma operação assíncrona
			await Task.Delay(100);

			var artesaoId = await _artesaoService.BuscarPorId(id); // Método assíncrono para buscar o Artesão

			if (artesaoId == null)
			{
				return NotFound();
			}

			await _artesaoService.Apagar(id);

			return NoContent();
		}

		#endregion

		#region Filtros 

		#endregion

	}
}
