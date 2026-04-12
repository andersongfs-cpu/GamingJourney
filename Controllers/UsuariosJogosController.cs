using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GamingJourney.Controllers;
using System.Security.Claims;


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

		[HttpPost("adicionar")]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> AdicionarJogoAColecao(UsuarioJogoColecaoDto dto)
		{
			var usuarioId = ObterUsuarioId();

			var resultado = await _usuarioJogoService.AdicionarJogoAColecaoAsync(usuarioId, dto);

			return Ok(resultado);
		}
	}
}
