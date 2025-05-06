using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace PrototipoBackEnd.API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ArtesaoController : ControllerBase
	{
		#region Construtor 

		private readonly IConfiguration _configuration;
		private readonly IArtesaoService _artesaoService;
		private readonly ILogger<ArtesaoController> _logger;

		public ArtesaoController(IConfiguration configuration, ILogger<ArtesaoController> logger, IArtesaoService artesaoService)
		{
			_configuration = configuration;
			_artesaoService = artesaoService;
			_logger = logger;
		}

		#endregion
		private static List<ArtesaoDto> artesaos = new List<ArtesaoDto>();

		#region Usuário CRUD PADRÃO

		// READ - Buscar todos os Artesãos (GET)
		[HttpGet]
		public async Task<ActionResult<List<ArtesaoDto>>> BuscarTodosArtesaos()
		{
			List<ArtesaoDto> artesaos = await _artesaoService.BuscarTodos();

			return Ok(artesaos);
		}

		// READ - Buscar Artesão por id (GET) {id}
		[HttpGet("{id}")]
		public async Task<ActionResult<ArtesaoDto>> BuscarArtesao(string id)
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
				return Ok(new { message = "Usuário encontrado", data = artesao });
			}
			catch (Exception ex)
			{
				// Caso ocorra algum erro, retorna BadRequest com a mensagem de erro
				return BadRequest(new { message = ex.Message });
			}
		}

		// CREATE - Adicionar um novo Artesão (POST)
		[HttpPost]
		public async Task<ActionResult<ArtesaoDto>> AdicionarArtesao([FromForm] ArtesaoDto dto, IFormFile imagem)
		{
			await _artesaoService.Adicionar(dto);

			return CreatedAtAction(nameof(AdicionarArtesao), new { dto }, dto);
		}

		// UPDATE - Atualizar um Artesão existente (PUT)
		[HttpPut("{id}")]
		public async Task<ActionResult<ArtesaoDto>> AtualizarArtesao([FromForm] ArtesaoDto dto, string id)
		{
			// Valida o modelo
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _artesaoService.Atualizar(dto, id);
				return Ok(new { message = "Usuário atualizado com sucesso" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// DELETE - Excluir um Artesão existente (DELETE)
		[HttpDelete("{id}")]
		public async Task<ActionResult<ArtesaoDto>> ExcluirArtesao(string id)
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
	}
}
