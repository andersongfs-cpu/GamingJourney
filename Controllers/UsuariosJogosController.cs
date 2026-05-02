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

		// Adicionar jogo a lista do usuário
		[HttpPost("adicionar")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AdicionarJogoAColecao(UsuarioJogoColecaoDto dto)
		{
			var usuarioId = ObterUsuarioId();

			var resultado = await _usuarioJogoService.AdicionarJogoAColecaoAsync(usuarioId, dto);

			return Ok(resultado);
		}

		// Mostra a lista de jogos do usuário
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

		// Deleta jogo da lista do usuário		
		[HttpDelete("remover")]
		public async Task<IActionResult> DeletarJogo([FromQuery] int? jogoId, [FromQuery] string? nomeJogo)
		{
			var usuarioId = ObterUsuarioId();
			await _usuarioJogoService.DeletarJogoAsync(usuarioId, jogoId, nomeJogo);

			return NoContent();
		}

		// Edita jogo da lista do usuário
		[HttpPut("editar")]
		public async Task<IActionResult> AtualizarJogo([FromQuery] int? jogoId, [FromQuery] string? nomeJogo, [FromBody] UsuarioJogoAtualizarDto editDto)
		{
			var usuarioId = ObterUsuarioId();
			var resultado = await _usuarioJogoService.AtualizarJogoAsync(usuarioId, jogoId, nomeJogo, editDto);
			return Ok(resultado);
		}
	}
}
