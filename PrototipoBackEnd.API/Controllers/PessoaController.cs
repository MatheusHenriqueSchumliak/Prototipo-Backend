using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrototipoBackEnd.API.Extensions;
using PrototipoBackEnd.API.Responses;
using PrototipoBackEnd.Application.Dtos.Pessoa;
using PrototipoBackEnd.Application.Interfaces;

namespace PrototipoBackEnd.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = Policies.Administrador)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    public class PessoaController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;

        public PessoaController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        [HttpGet("BuscarTodos")]
        [ProducesResponseType(typeof(ApiResponse<List<PessoaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarTodos()
        {
            var pessoas = await _pessoaService.BuscarTodos();
            return Ok(new ApiResponse<List<PessoaDto>>("Pessoas encontradas", pessoas));
        }

        [HttpGet("BuscarPorId/{id}")]
        [ProducesResponseType(typeof(ApiResponse<PessoaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarPorId(string id)
        {
            var pessoa = await _pessoaService.BuscarPorId(id);
            return Ok(new ApiResponse<PessoaDto>("Pessoa encontrada", pessoa));
        }

        [HttpPost("Adicionar")]
        [ProducesResponseType(typeof(ApiResponse<PessoaDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Adicionar([FromBody] PessoaDto dto)
        {
            var pessoa = await _pessoaService.Adicionar(dto);
            return CreatedAtAction(nameof(BuscarPorId), new { id = pessoa.Id },
                new ApiResponse<PessoaDto>("Pessoa criada com sucesso", pessoa));
        }

        [HttpPut("Atualizar/{id}")]
        [ProducesResponseType(typeof(ApiResponse<PessoaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar([FromBody] PessoaDto dto, string id)
        {
            var pessoa = await _pessoaService.Atualizar(dto, id);
            return Ok(new ApiResponse<PessoaDto>("Pessoa atualizada com sucesso", pessoa));
        }

        [HttpDelete("Excluir/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Excluir(string id)
        {
            await _pessoaService.Apagar(id);
            return NoContent();
        }
    }
}
