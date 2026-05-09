using GamingJourney.DTOs;
using GamingJourney.Models;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamingJourney.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class UsuariosJogosController : MainController
	{
		private readonly UsuarioJogoService _usuarioJogoService;
		public UsuariosJogosController(UsuarioJogoService usuarioJogoService)
		{
			_usuarioJogoService = usuarioJogoService;
		}

		/// <summary>
		/// Adiciona um jogo à sua biblioteca pessoal.
		/// </summary>
		/// <remarks>
		/// Use este endpoint para vincular um jogo existente ao seu perfil e começar a registrar seu progresso.
		/// </remarks>
		/// <param name="dto">Contém o ID do jogo e as informações iniciais (nota, status, etc).</param>
		[HttpPost("adicionar")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AdicionarJogoAColecao(UsuarioJogoColecaoDto dto)
		{
			var usuarioId = ObterUsuarioId();
			var resultado = await _usuarioJogoService.AdicionarJogoAColecaoAsync(usuarioId, dto);
			return Ok(resultado);
		}

		/// <summary>
		/// Lista todos os jogos da sua coleção com filtros opcionais.
		/// </summary>
		/// <remarks>
		/// Permite buscar dentro da sua lista por nome, gênero, nota, plataforma ou status (ex: Jogando, Zerado).
		/// </remarks>
		/// <param name="nomeJogo">Opcional: Filtrar pelo nome do jogo.</param>
		/// <param name="generoJogo">Opcional: Filtrar por um gênero específico.</param>
		/// <param name="nota">Opcional: Filtrar jogos com uma nota específica.</param>
		/// <param name="plataforma">Opcional: Filtrar por plataforma.</param>
		/// <param name="status">Opcional: Filtrar pelo status atual do jogo na sua lista.</param>
		[HttpGet]
		public async Task<ActionResult<List<UsuarioJogoExibicaoDto>>> ExibirListaDeJogos(
		[FromQuery] string? nomeJogo,
		[FromQuery] string? generoJogo,
		[FromQuery] decimal? nota,
		[FromQuery] string? plataforma,
		[FromQuery] StatusJogo? status)
		{
			var usuarioId = ObterUsuarioId();
			var resultado = await _usuarioJogoService.ExibirListaDeJogosAsync
				(usuarioId, nomeJogo, generoJogo, nota, plataforma, status);

			return Ok(resultado);
		}

		/// <summary>
		/// Remove um jogo da sua biblioteca pessoal.
		/// </summary>
		/// <remarks>
		/// Você pode identificar o jogo que quer remover passando o ID ou o nome exato dele.
		/// </remarks>
		/// <param name="jogoId">Opcional: ID do jogo que será removido.</param>
		/// <param name="nomeJogo">Opcional: Nome do jogo que será removido.</param>		
		[HttpDelete("remover")]
		public async Task<IActionResult> DeletarJogo([FromQuery] int? jogoId, [FromQuery] string? nomeJogo)
		{
			var usuarioId = ObterUsuarioId();
			await _usuarioJogoService.DeletarJogoAsync(usuarioId, jogoId, nomeJogo);

			return NoContent();
		}

		/// <summary>
		/// Atualiza seus dados de um jogo (nota, status ou comentário).
		/// </summary>
		/// <remarks>
		/// Use para mudar sua nota ou atualizar se você já zerou o jogo, por exemplo. 
		/// Identifique o jogo pelo ID ou nome e envie as novas informações no corpo da requisição.
		/// </remarks>
		/// <param name="jogoId">Opcional: ID do jogo para editar.</param>
		/// <param name="nomeJogo">Opcional: Nome do jogo para editar.</param>
		/// <param name="editDto">Novos dados para atualização.</param>
		[HttpPut("editar")]
		public async Task<IActionResult> AtualizarJogo([FromQuery] int? jogoId, [FromQuery] string? nomeJogo, [FromBody] UsuarioJogoAtualizarDto editDto)
		{
			var usuarioId = ObterUsuarioId();
			var resultado = await _usuarioJogoService.AtualizarJogoAsync(usuarioId, jogoId, nomeJogo, editDto);
			return Ok(resultado);
		}
	}
}
