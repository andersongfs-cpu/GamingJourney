using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamingJourney.Controllers
{
	[Route("Api/[controller]")]
	[ApiController]
	public class JogosController : MainController
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
		[ProducesResponseType(typeof(List<JogoResponseDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<JogoExibicaoDto>> ExibirJogosId(int id)
		{
			var jogos = await _jogoService.ExibirPorIdAsync(id);
			return Ok(jogos);
		}

		// Registra um novo jogo
		[Authorize(Roles = "Admin, GM")]
		[HttpPost("register")]
		[ProducesResponseType(typeof(JogoResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> RegistrarJogo(JogoRegistroDto dto)
		{
			var jogo = await _jogoService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(ExibirJogosId), new { id = jogo.Id }, jogo);
		}

		// Edita/Atualiza um jogo cadastrado
		[Authorize(Roles = "Admin, GM")]
		[HttpPut("{id:int}")]
		[ProducesResponseType(typeof(JogoResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<JogoResponseDto>> AtualizarJogos(int id, JogoAtualizarDto dto)
		{
			var jogo = await _jogoService.AtualizarAsync(id, dto);
			return Ok(jogo);
		}

		// Remove um jogo cadastrado
		[Authorize(Roles = "Admin, GM")]
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> DeletaJogo(int id)
		{
			await _jogoService.DeletarJogoAsync(id);
			return NoContent();
		}
	}
}
