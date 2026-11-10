using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrototipoBackEnd.API.Extensions;
using PrototipoBackEnd.API.Responses;
using PrototipoBackEnd.Application.Dtos.Artesanato;
using PrototipoBackEnd.Application.Interfaces;

namespace PrototipoBackEnd.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = Policies.Administrador)]
    public class ArtesanatoController : ControllerBase
    {
        private readonly IArtesanatoService _artesanatoService;

        public ArtesanatoController(IArtesanatoService artesanatoService)
        {
            _artesanatoService = artesanatoService;
        }

        [AllowAnonymous]
        [HttpGet("BuscarTodos")]
        [ProducesResponseType(typeof(ApiResponse<List<ArtesanatoDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarTodos()
        {
            var artesanatos = await _artesanatoService.BuscarTodos();
            return Ok(new ApiResponse<List<ArtesanatoDto>>("Artesanatos encontrados", artesanatos));
        }

        [AllowAnonymous]
        [HttpGet("BuscarPorId/{id}")]
        [ProducesResponseType(typeof(ApiResponse<ArtesanatoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarPorId(string id)
        {
            var artesanato = await _artesanatoService.BuscarPorId(id);
            return Ok(new ApiResponse<ArtesanatoDto>("Artesanato encontrado", artesanato));
        }

        [AllowAnonymous]
        [HttpGet("PorArtesao/{artesaoId}")]
        [ProducesResponseType(typeof(ApiResponse<ArtesanatoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarPorArtesaoId(string artesaoId)
        {
            var artesanato = await _artesanatoService.BuscarPorArtesaoId(artesaoId);
            return Ok(new ApiResponse<ArtesanatoDto>("Artesanato encontrado", artesanato));
        }

        [AllowAnonymous]
        [HttpGet("TodosPorArtesao/{artesaoId}")]
        [ProducesResponseType(typeof(ApiResponse<List<ArtesanatoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarTodosPorArtesaoId(string artesaoId)
        {
            var artesanatos = await _artesanatoService.BuscarTodosPorArtesaoId(artesaoId);
            return Ok(new ApiResponse<List<ArtesanatoDto>>("Artesanatos encontrados", artesanatos));
        }

        [HttpPost("Adicionar")]
        [ProducesResponseType(typeof(ApiResponse<ArtesanatoDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Adicionar([FromForm] ArtesanatoDto dto, List<IFormFile> imagem)
        {
            var result = await _artesanatoService.Adicionar(dto, imagem);
            return CreatedAtAction(nameof(BuscarPorId), new { id = result.Id },
                new ApiResponse<ArtesanatoDto>("Artesanato criado com sucesso", result));
        }

        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar([FromForm] ArtesanatoDto dto, string id, List<IFormFile>? imagem = null)
        {
            await _artesanatoService.Atualizar(dto, id, imagem);
            return Ok(new ApiResponse<object>("Artesanato atualizado com sucesso", null));
        }

        [HttpDelete("Excluir/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(string id)
        {
            await _artesanatoService.Apagar(id);
            return NoContent();
        }
    }
}
