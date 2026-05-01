using GamingJourney.DTOs;
using GamingJourney.Services;
using GamingJourney.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamingJourney.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GenerosController : MainController
	{
		private readonly GeneroService _generoService;

		public GenerosController(GeneroService generoService)
		{
			_generoService = generoService;
		}


		// Registra um novo gênero
		[Authorize(Roles = "Admin, GM")]
		[HttpPost("register")]
		[ProducesResponseType(typeof(GeneroResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> Registrar(GeneroRegistroDto dto)
		{
			var resultado = await _generoService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(Registrar), new { id = resultado.Id }, resultado);
		}


		// Lista generos cadastrados por Nome
		[HttpGet]
		[ProducesResponseType(typeof(List<GeneroExibicaoDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<List<GeneroExibicaoDto>>> ExibirGeneros([FromQuery] string? nome)
		{
			var generos = await _generoService.ExibirTodosAsync(nome);
			return Ok(generos);
		}

		// Lista generos cadastrados por Id
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(GeneroExibicaoDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<GeneroExibicaoDto>> ExibirGenerosId(int id)
		{
			var generosId = await _generoService.ExibirPorIdAsync(id);
			return Ok(generosId);

		}

		// Mais tarde alterar para desativar apenas
		// Remove gênero do BD por ID
		[Authorize(Roles = "Admin, GM")]
		[HttpDelete("genre-delete/{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> DeletarGenero(int id)
		{
			await _generoService.DeletarGeneroAsync(id);
			return NoContent();
		}

		// Edita/Put gênero por Id
		[Authorize(Roles = "Admin, GM")]
		[HttpPut("{id:int}")]
		[ProducesResponseType(typeof(GeneroExibicaoDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<GeneroExibicaoDto>> AtualizarGenero(int id, GeneroAtualizarDto dto)
		{
			var generoEdit = await _generoService.AtualizarAsync(id, dto);			
			return Ok(generoEdit);
		}
	}
}
