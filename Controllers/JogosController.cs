using AutoMapper;
using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamingJourney.Controllers
{
	[Route("Api/[controller]")]
	[ApiController]
	public class JogosController : ControllerBase
	{
		private readonly JogoService _jogoService;
		private readonly GeneroService _generoService;
		private readonly PlataformaService _plataformaService;

		public JogosController(JogoService jogoService, GeneroService generoService, PlataformaService plataformaService)
		{
			_jogoService = jogoService;
			_generoService = generoService;
			_plataformaService = plataformaService;
		}

		// Lista jogos cadastrados por nome
		[HttpGet]
		public async Task<ActionResult<List<JogoExibicaoDto>>> ExibirJogos(
		[FromQuery] string? titulo,
		[FromQuery] string? genero,
		[FromQuery] string? plataforma)
		{
			var jogos = await _jogoService.ExibirTodosAsync(titulo, genero, plataforma);
			return Ok(jogos);
		}

		// Lista jogos castrados por Id
		[HttpGet("{id:int}")]
		public async Task<ActionResult<JogoExibicaoDto>> ExibirJogosId(int id)
		{
			var jogos = await _jogoService.ExibirTodosIdAsync(id);
			if (jogos == null) return NotFound("Jogo não cadastrado.");

			return Ok(jogos);
		}

		// Registra um novo jogo
		[HttpPost("registrar")]
		public async Task<IActionResult> RegistrarJogo(JogoRegistroDto dto)
		{
			var jogo = await _jogoService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(ExibirJogosId), new { id = jogo.Id }, jogo);
		}

		// Edita/Atualiza um jogo cadastrado
		[HttpPut("{id:int}")]
		[ProducesResponseType(typeof(JogoExibicaoDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<JogoExibicaoDto>> AtualizarJogos(int id, JogoAtualizarDto dto)
		{
			var jogo = await _jogoService.AtualizarAsync(id, dto);

			if (jogo == null) return NotFound("Jogo não encontrado.");

			return Ok(jogo);
		}

		// Remove um jogo cadastrado
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeletaJogo(int id)
		{
			var jogo = await _jogoService.DeletarJogoAsync(id);

			if (jogo == null) return NotFound("Jogo não encontrado.");

			return NoContent();
		}
	}
}
