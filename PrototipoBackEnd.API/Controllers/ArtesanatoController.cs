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
	public class ArtesanatoController : ControllerBase
	{
		#region Construtor 

		private readonly IConfiguration _configuration;
		private readonly IArtesanatoService _artesanatoService;
		private readonly ILogger<ArtesanatoController> _logger;
		private readonly IAmazonS3Service _amazonS3Service;

		public ArtesanatoController(IConfiguration configuration, ILogger<ArtesanatoController> logger, IArtesanatoService artesanatoService, IAmazonS3Service amazonS3Service)
		{
			_amazonS3Service = amazonS3Service;
			_configuration = configuration;
			_artesanatoService = artesanatoService;
			_logger = logger;
		}

		#endregion

		private static List<ArtesanatoDto> artesanatos = new List<ArtesanatoDto>();

		#region Usuário CRUD PADRÃO

		// READ - Buscar todos os Artesanatos (GET)
		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<List<ArtesanatoDto>>> BuscarTodos()
		{
			List<ArtesanatoDto> artesanatos = await _artesanatoService.BuscarTodos();

			return Ok(artesanatos);
		}

		// READ - Buscar Artesanato por id (GET) {id}
		[AllowAnonymous]
		[HttpGet("{id}")]
		public async Task<ActionResult<ArtesanatoDto>> BuscarPorId(string id)
		{
			try
			{
				// Chama o serviço para buscar o Artesanato
				var artesanato = await _artesanatoService.BuscarPorId(id);

				if (artesanato == null)
				{
					// Retorna NotFound se o Artesanato não for encontrado
					return NotFound(new { message = $"Usuário com ID {id} não encontrado." });
				}

				// Retorna OK com os dados do Artesanato encontrado
				return Ok(new { message = "Usuário encontrado", data = artesanato });
			}
			catch (Exception ex)
			{
				// Caso ocorra algum erro, retorna BadRequest com a mensagem de erro
				return BadRequest(new { message = ex.Message });
			}
		}

		[AllowAnonymous]
		[HttpGet("{artesaoId}")]
		public async Task<ActionResult<ArtesanatoDto>> BuscarPorArtesaoId(string artesaoId)
		{
			try
			{
				// Chama o serviço para buscar o Artesanato
				var artesanato = await _artesanatoService.BuscarPorArtesaoId(artesaoId);

				if (artesanato == null)
				{
					// Retorna NotFound se o Artesanato não for encontrado
					return NotFound(new { message = $"Artesanato com ID {artesaoId} não encontrado." });
				}

				// Retorna OK com os dados do Artesanato encontrado
				return Ok(new { message = "Artesanato encontrado", data = artesanato });
			}
			catch (Exception ex)
			{
				// Caso ocorra algum erro, retorna BadRequest com a mensagem de erro
				return BadRequest(new { message = ex.Message });
			}
		}

		[AllowAnonymous]
		[HttpGet("{artesaoId}")]
		public async Task<ActionResult<ArtesanatoDto>> BuscarTodosPorArtesaoId(string artesaoId)
		{
			try
			{
				// Chama o serviço para buscar o Artesanato
				var artesanatos = await _artesanatoService.BuscarTodosPorArtesaoId(artesaoId);

				if (artesanatos == null)
				{
					// Retorna NotFound se o Artesanato não for encontrado
					return NotFound(new { message = $"Artesanatos com o ArtesãoID {artesaoId} não encontrado." });
				}

				// Retorna OK com os dados do Artesanato encontrado
				return Ok(new { message = "Artesanatos encontrados", data = artesanatos });
			}
			catch (Exception ex)
			{
				// Caso ocorra algum erro, retorna BadRequest com a mensagem de erro
				return BadRequest(new { message = ex.Message });
			}
		}

		// CREATE - Adicionar um novo Artesanato (POST)
		[HttpPost]
		public async Task<ActionResult<ArtesanatoDto>> Adicionar([FromForm] ArtesanatoDto dto, List<IFormFile> imagem)
		{
			try
			{
				var result = await _artesanatoService.Adicionar(dto, imagem);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// UPDATE - Atualizar um Artesanato existente (PUT)
		[HttpPut("{id}")]
		public async Task<ActionResult<ArtesanatoDto>> Atualizar([FromForm] ArtesanatoDto dto, string id)
		{
			// Valida o modelo
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _artesanatoService.Atualizar(dto, id);
				return Ok(new { message = "Usuário atualizado com sucesso" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// DELETE - Excluir um Artesanato existente (DELETE)
		[HttpDelete("{id}")]
		public async Task<ActionResult<ArtesanatoDto>> Excluir(string id)
		{
			// Simula uma operação assíncrona
			await Task.Delay(100);

			var artesanatoId = await _artesanatoService.BuscarPorId(id); // Método assíncrono para buscar o Artesanato

			if (artesanatoId == null)
			{
				return NotFound();
			}

			await _artesanatoService.Apagar(id);

			return NoContent();
		}

		#endregion

		#region Filtros 

		#endregion

	}
}
