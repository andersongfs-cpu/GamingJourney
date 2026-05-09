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

		/// <summary>
		/// Lista todos os jogos com filtros opcionais.
		/// </summary>
		/// <remarks>
		/// Você pode buscar jogos por título, nome do gênero ou nome da plataforma. 
		/// Se não enviar nenhum parâmetro, o sistema traz a lista completa.
		/// </remarks>
		/// <param name="titulo">Opcional: Parte do nome do jogo.</param>
		/// <param name="genero">Opcional: Nome do gênero (ex: RPG, Ação).</param>
		/// <param name="plataforma">Opcional: Nome da plataforma (ex: PC, PS5).</param>
		[HttpGet]
		public async Task<ActionResult<List<JogoExibicaoDto>>> ExibirJogos(
		[FromQuery] string? titulo,
		[FromQuery] string? genero,
		[FromQuery] string? plataforma)
		{
			var jogos = await _jogoService.ExibirTodosAsync(titulo, genero, plataforma);
			return Ok(jogos);
		}

		/// <summary>
		/// Busca os detalhes de um jogo específico.
		/// </summary>
		/// <remarks>
		/// Use este endpoint para ver todas as informações de um jogo usando o ID dele.
		/// </remarks>
		/// <param name="id">ID do jogo que você está procurando.</param>
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(List<JogoResponseDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<JogoExibicaoDto>> ExibirJogosId(int id)
		{
			var jogos = await _jogoService.ExibirPorIdAsync(id);
			return Ok(jogos);
		}

		/// <summary>
		/// Adiciona um novo jogo à biblioteca.
		/// </summary>
		/// <remarks>
		/// Requer permissão de Admin ou GM. Informe os dados básicos e os IDs de gênero e plataforma.
		/// </remarks>
		/// <param name="dto">Dados do jogo que será cadastrado.</param>
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

		/// <summary>
		/// Edita as informações de um jogo já cadastrado.
		/// </summary>
		/// <remarks>
		/// Requer permissão de Admin ou GM. Use para corrigir nomes, descrições ou vínculos do jogo.
		/// </remarks>
		/// <param name="id">ID do jogo que você quer mudar.</param>
		/// <param name="dto">Novos dados para atualizar o jogo.</param>
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

		/// <summary>
		/// Apaga um jogo permanentemente do sistema.
		/// </summary>
		/// <remarks>
		/// Requer permissão de Admin ou GM. Cuidado: uma vez deletado, não dá para recuperar.
		/// </remarks>
		/// <param name="id">ID do jogo que será removido.</param>
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
