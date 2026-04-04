using AutoMapper;
using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Mvc;


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

		[HttpGet("generos")]
		public async Task<ActionResult<List<GeneroExibicaoDto>>> ExibirGeneros([FromQuery] string? nome)
		{
			var lista = await _generoService.GetTodos(nome);
			return Ok(lista);
		}

		
	}
}
