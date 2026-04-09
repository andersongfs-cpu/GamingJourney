using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamingJourney.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlataformasController : ControllerBase
	{
		private readonly PlataformaService _plataformaService;

		public PlataformasController(PlataformaService plataformaService)
		{
			_plataformaService = plataformaService;
		}

		// Registra uma nova plataforma
		[HttpPost("registrar")]
		[ProducesResponseType(typeof(PlataformaResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> RegistrarPlataforma(PlataformaRegistroDto dto)
		{
			var plataforma = await _plataformaService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(ExibirTodosId), new { id = plataforma.Id }, plataforma);
		}
		
		// Lista plataformas cadastradas por nome
		[HttpGet]
		[ProducesResponseType(typeof(List<PlataformaExibicaoDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<List<PlataformaExibicaoDto>>> ExibirTodos([FromQuery] string? nome)
		{
			var plataformas = await _plataformaService.ExibirTodosAsync(nome);
			return Ok(plataformas);
		}

		// Lista plataformas cadastradas por Id
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(PlataformaExibicaoDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<PlataformaExibicaoDto>> ExibirTodosId(int id)
		{
			var plataforma = await _plataformaService.ExibirTodosIdAsync(id);
			if (plataforma == null) return NotFound("Plataforma não encontrada.");

			return Ok(plataforma);
		}

		// Edita/Put plataforma por Id
		[HttpPut("{id:int}")]
		[ProducesResponseType(typeof(PlataformaExibicaoDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<PlataformaExibicaoDto>> AtualizarPlataforma(int id, PlataformaAtualizarDto dto)
		{
			var plataformaEdit = await _plataformaService.AtualizarAsync(id, dto);
			if (plataformaEdit == null) return NotFound("Plataforma não encontrada.");

			return Ok(plataformaEdit);
		}

		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeletarPlataforma(int id)
		{
			var plataforma = await _plataformaService.DeletarPlataformaAsync(id);
			if (plataforma == null) return NotFound("Plataforma não encontrada.");

			return NoContent();
		}
	}
}
