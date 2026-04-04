using AutoMapper;
using GamingJourney.DTOs;
using GamingJourney.Models;
using GamingJourney.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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

		[HttpPost("login")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Login(UsuarioLoginDto dto)
		{
			try
			{
				var token = await _usuarioService.LoginAsync(dto);
				return Ok(new { token });
			}
			catch (Exception ex)
			{
				return BadRequest(new { mensagem = ex.Message });
			}
		}

		[HttpGet("usuarios")]
		public async Task<ActionResult<List<UsuarioExibicaoDto>>> ExibirUsuarios(
		[FromQuery] string? nome, 
		[FromQuery] string? email)
		{
			var lista = await _usuarioService.GetTodos(nome, email);
			return Ok(lista);
		}
	}
}