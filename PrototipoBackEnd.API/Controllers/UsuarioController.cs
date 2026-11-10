using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrototipoBackEnd.API.Extensions;
using PrototipoBackEnd.API.Responses;
using PrototipoBackEnd.Application.Dtos.Usuario;
using PrototipoBackEnd.Application.Interfaces;

namespace PrototipoBackEnd.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = Policies.Administrador)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet("BuscarTodos")]
        [ProducesResponseType(typeof(ApiResponse<List<UsuarioResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarTodos()
        {
            var usuarios = await _usuarioService.BuscarTodos();
            return Ok(new ApiResponse<List<UsuarioResponseDto>>("Usuários encontrados", usuarios));
        }

        [HttpGet("BuscarPorId/{id}")]
        [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarPorId(string id)
        {
            var usuario = await _usuarioService.BuscarPorId(id);
            return Ok(new ApiResponse<UsuarioResponseDto>("Usuário encontrado", usuario));
        }

        [HttpPost("Adicionar")]
        [ProducesResponseType(typeof(ApiResponse<UsuarioResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Adicionar([FromBody] UsuarioRequestDto dto)
        {
            var usuario = await _usuarioService.Adicionar(dto);
            return CreatedAtAction(nameof(BuscarPorId), new { id = usuario.Id },
                new ApiResponse<UsuarioResponseDto>("Usuário criado com sucesso", usuario));
        }

        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Atualizar([FromBody] UsuarioAtualizarRequestDto dto, string id)
        {
            await _usuarioService.Atualizar(dto, id);
            return Ok(new ApiResponse<object>("Usuário atualizado com sucesso", null));
        }

        [HttpDelete("Excluir/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(string id)
        {
            await _usuarioService.Apagar(id);
            return NoContent();
        }
    }
}
