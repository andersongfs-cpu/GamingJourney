using AutoMapper;
using GamingJourney.DTOs;
using GamingJourney.Models;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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


		// Cadastra novo usuário
		[HttpPost("registrar")]
		[ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Registrar(UsuarioRegistroDto dto)
		{
			try
			{
				var resultado = await _usuarioService.RegistrarAsync(dto);
				return CreatedAtAction(nameof(GetPorId), new { id = resultado.Id }, resultado);
			}
			catch (Exception ex)
			{
				return BadRequest(new { mensagem = ex.Message });
			}
		}

		// Login
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

		// Busca usuário por Id
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetPorId(int id)
		{
			var usuario = await _usuarioService.GetPorId(id);
			if (usuario == null) return NotFound();
			return Ok(usuario);
		}

		// Busca usuário por Nome ou Email
		[HttpGet]
		[ProducesResponseType(typeof(List<UsuarioExibicaoDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<List<UsuarioExibicaoDto>>> Listar(
		[FromQuery] string? nome,
		[FromQuery] string? email)
		{
			var lista = await _usuarioService.GetTodosAsync(nome, email);
			return Ok(lista);
		}

		// Edita Usuário
		[Authorize]
		[HttpPut("perfil")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> EditarAsync(UsuarioAtualizarDto dto)
		{
			var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(usuarioIdClaim))
			{
				return Unauthorized(new { message = "Token inválido ou expirado" });
			}

			if (!int.TryParse(usuarioIdClaim, out int idLogado))
			{
				return BadRequest(new { message = "ID de usuário inválido no Token." });
			}

			try
			{
				var usuarioEdit = await _usuarioService.AtualizarAsync(idLogado, dto);
				
				if (usuarioEdit == null)
				{
					return NotFound("Usuário não encontrado");
				}

				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		// Exclui usuário
		[Authorize]
		[HttpDelete("excluir-perfil")]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<IActionResult> ExcluirPerfil()
		{
			var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(usuarioIdClaim))
			{
				return Unauthorized(new { message = "Token inválido ou expirado" });
			}

			if (!int.TryParse(usuarioIdClaim, out int idLogado))
			{
				return BadRequest(new { message = "ID de usuário inválido no Token." });
			}

			try
			{
				var usuarioDel = await _usuarioService.DelUsuario(idLogado);
				
				if (usuarioDel == null) return NotFound("Usuário não encontrado.");

				return NoContent();
			}
			catch(Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
	}
}