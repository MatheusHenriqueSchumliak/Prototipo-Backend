using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrototipoBackEnd.API.Extensions;
using PrototipoBackEnd.API.Responses;
using PrototipoBackEnd.Application.Dtos.Artesao;
using PrototipoBackEnd.Application.Interfaces;

namespace PrototipoBackEnd.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = Policies.Administrador)]
    public class ArtesaoController : ControllerBase
    {
        private readonly IArtesaoService _artesaoService;

        public ArtesaoController(IArtesaoService artesaoService)
        {
            _artesaoService = artesaoService;
        }

        [AllowAnonymous]
        [HttpGet("BuscarTodos")]
        [ProducesResponseType(typeof(ApiResponse<List<ArtesaoDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarTodos(
            [FromQuery] string? nome = null,
            [FromQuery] string? nichoAtuacao = null,
            [FromQuery] bool? receberEncomendas = null,
            [FromQuery] bool? enviaEncomendas = null)
        {
            var artesaos = await _artesaoService.BuscarComFiltro(nome, nichoAtuacao, receberEncomendas, enviaEncomendas);
            return Ok(new ApiResponse<List<ArtesaoDto>>("Artesãos encontrados", artesaos));
        }

        [AllowAnonymous]
        [HttpGet("BuscarPorId/{id}")]
        [ProducesResponseType(typeof(ApiResponse<ArtesaoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarPorId(string id)
        {
            var artesao = await _artesaoService.BuscarPorId(id);
            return Ok(new ApiResponse<ArtesaoDto>("Artesão encontrado", artesao));
        }

        [HttpPost("Adicionar")]
        [ProducesResponseType(typeof(ApiResponse<ArtesaoDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Adicionar([FromForm] ArtesaoDto dto, IFormFile imagem)
        {
            var result = await _artesaoService.Adicionar(dto, imagem);
            return CreatedAtAction(nameof(BuscarPorId), new { id = result.Id },
                new ApiResponse<ArtesaoDto>("Artesão criado com sucesso", result));
        }

        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(typeof(ApiResponse<ArtesaoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar([FromForm] ArtesaoDto dto, string id, IFormFile? imagem = null)
        {
            var result = await _artesaoService.Atualizar(dto, id, imagem);
            return Ok(new ApiResponse<ArtesaoDto>("Artesão atualizado com sucesso", result));
        }

        [HttpDelete("Excluir/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(string id)
        {
            await _artesaoService.Apagar(id);
            return NoContent();
        }
    }
}
