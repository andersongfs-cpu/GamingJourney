using Microsoft.AspNetCore.Mvc;
using GamingJourney.DTOs;
using GamingJourney.Services;
using AutoMapper;

namespace GamingJourney.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsuariosController : ControllerBase
	{
		private readonly UsuarioService _usuarioService;

		public UsuariosController(UsuarioService usuarioService)
		{
			_usuarioService = usuarioService;
		}


		[HttpPost("registrar")]
		[ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Registrar(UsuarioRegistroDto dto)
		{
			try
			{
				var resultado = await _usuarioService.RegistrarAsync(dto);
				return CreatedAtAction(nameof(Registrar), resultado);
			}
			catch (Exception ex)
			{
				return BadRequest(new { mensagem = ex.Message });
			}
		}		
	}
}