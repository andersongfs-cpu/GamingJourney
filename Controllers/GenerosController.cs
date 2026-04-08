using AutoMapper;
using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace GamingJourney.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GenerosController : ControllerBase
	{
		private readonly GeneroService _generoService;

		public GenerosController(GeneroService generoService)
		{
			_generoService = generoService;
		}


		// Registra um novo gênero
		[HttpPost("registrar")]
		[ProducesResponseType(typeof(GeneroResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Registrar(GeneroRegistroDto dto)
		{
			try
			{
				var resultado = await _generoService.RegistrarAsync(dto);
				return CreatedAtAction(nameof(Registrar), resultado);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
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
			try
			{
				var genero = await _generoService.ExibirTodosIdAsync(id);
				if (genero == null) return NotFound();
				return Ok(genero);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// Remove gênero do BD por ID
		[Authorize(Roles = "Admin")]
		[HttpDelete("excluir-genero/{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> DeletarGenero(int id)
		{
			var genero = await _generoService.DeletarGeneroAsync(id);
			if (genero == null) return NotFound();

			return NoContent();
		}
	}
}
