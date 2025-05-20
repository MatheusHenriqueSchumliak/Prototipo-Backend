using Microsoft.AspNetCore.Mvc;
using PrototipoBackEnd.Application.Dtos;
using PrototipoBackEnd.Application.Interfaces;
using PrototipoBackEnd.Domain.Entities;

namespace PrototipoBackEnd.API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class UsuarioController : ControllerBase
	{
		#region Construtor 

		private readonly IConfiguration _configuration;
		private readonly IUsuarioService _usuarioService;
		private readonly ILogger<UsuarioController> _logger;

		public UsuarioController(IConfiguration configuration, ILogger<UsuarioController> logger, IUsuarioService usuarioService)
		{
			_configuration = configuration;
			_usuarioService = usuarioService;
			_logger = logger;
		}

		#endregion

		private static List<UsuarioDto> usuarios = new List<UsuarioDto>();

		#region Usuário CRUD PADRÃO

		// READ - Buscar todos os usuários (GET)
		[HttpGet]
		public async Task<ActionResult<List<UsuarioDto>>> BuscarTodos()
		{
			List<UsuarioDto> usuarios = await _usuarioService.BuscarTodos();

			return Ok(usuarios);
		}

		// READ - Buscar usuário por id (GET) {id}
		[HttpGet("{id}")]
		public async Task<ActionResult<UsuarioDto>> BuscarPorId(string id)
		{
			try
			{
				// Chama o serviço para buscar o usuário
				var usuario = await _usuarioService.BuscarPorId(id);

				if (usuario == null)
				{
					// Retorna NotFound se o usuário não for encontrado
					return NotFound(new { message = $"Usuário com ID {id} não encontrado." });
				}

				// Retorna OK com os dados do usuário encontrado
				return Ok(new { message = "Usuário encontrado", data = usuario });
			}
			catch (Exception ex)
			{
				// Caso ocorra algum erro, retorna BadRequest com a mensagem de erro
				return BadRequest(new { message = ex.Message });
			}
		}

		// CREATE - Adicionar um novo usuário (POST)
		[HttpPost]
		public async Task<ActionResult<UsuarioDto>> Adicionar([FromBody] UsuarioDto dto)
		{
			await _usuarioService.Adicionar(dto);

			//return CreatedAtAction(nameof(AdicionarUsuario), new { dto }, dto);
			return Ok(dto);

		}

		// UPDATE - Atualizar um usuário existente (PUT)
		[HttpPut("{id}")]
		public async Task<ActionResult<UsuarioDto>> Atualizar([FromForm] UsuarioDto dto, string id)
		{
			// Valida o modelo
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				await _usuarioService.Atualizar(dto, id);
				return Ok(new { message = "Usuário atualizado com sucesso" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// DELETE - Excluir um usuário existente (DELETE)
		[HttpDelete("{id}")]
		public async Task<ActionResult<UsuarioDto>> Excluir(string id)
		{
			// Simula uma operação assíncrona
			await Task.Delay(100);

			var usuarioId = await _usuarioService.BuscarPorId(id); // Método assíncrono para buscar o usuário

			if (usuarioId == null)
			{
				return NotFound();
			}

			await _usuarioService.Apagar(id);

			return NoContent();
		}

		#endregion
	}
}
