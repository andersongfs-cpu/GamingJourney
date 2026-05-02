using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamingJourney.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsuariosController : MainController
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
			var usuario = await _usuarioService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(ExibirPorId), new { id = usuario.Id }, usuario);
		}

		// Login
		[HttpPost("login")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Login(UsuarioLoginDto dto)
		{
			var token = await _usuarioService.LoginAsync(dto);
			return Ok(new { token });
		}

		// Busca usuário por Id
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> ExibirPorId(int id)
		{
			var usuario = await _usuarioService.GetPorId(id);			
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
		public async Task<IActionResult> AtualizarUsuario(UsuarioAtualizarDto dto)
		{
			var usuarioId = ObterUsuarioId();
			await _usuarioService.AtualizarAsync(usuarioId, dto);
			return NoContent();
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
			var usuarioId = ObterUsuarioId();
			await _usuarioService.DelUsuario(usuarioId);
			return NoContent();
		}
	}
}