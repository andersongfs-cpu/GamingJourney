using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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


		/// <summary>
		/// Cria uma nova conta de usuário.
		/// </summary>
		/// <remarks>
		/// Este endpoint possui limite de requisições por IP para evitar abusos. 
		/// Use para se cadastrar no sistema GamingJourney.
		/// </remarks>
		/// <param name="dto">Dados de cadastro (Nome, Email, Senha).</param>		
		[HttpPost("registrar")]
		[EnableRateLimiting("RegistroPolicy")]
		[ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Registrar(UsuarioRegistroDto dto)
		{
			var usuario = await _usuarioService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(ExibirPorId), new { id = usuario.Id }, usuario);
		}

		/// <summary>
		/// Realiza o login e retorna um token de acesso.
		/// </summary>
		/// <remarks>
		/// O token JWT retornado deve ser usado nos outros endpoints que exigem autorização.
		/// Também possui limite de tentativas por segurança.
		/// </remarks>
		/// <param name="dto">Email e senha do usuário.</param>
		[HttpPost("login")]
		[EnableRateLimiting("RegistroPolicy")]
		[ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Login(UsuarioLoginDto dto)
		{
			var token = await _usuarioService.LoginAsync(dto);
			return Ok(new { token });
		}

		/// <summary>
		/// Busca os dados públicos de um usuário pelo ID.
		/// </summary>
		/// <remarks>
		/// Útil para visualizar informações de perfil de outros usuários ou do sistema.
		/// </remarks>
		/// <param name="id">ID numérico do usuário.</param>
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> ExibirPorId(int id)
		{
			var usuario = await _usuarioService.GetPorId(id);			
			return Ok(usuario);
		}

		/// <summary>
		/// Lista usuários cadastrados com filtros opcionais.
		/// </summary>
		/// <remarks>
		/// Permite buscar usuários especificamente pelo nome ou pelo e-mail.
		/// </remarks>
		/// <param name="nome">Opcional: Parte do nome do usuário.</param>
		/// <param name="email">Opcional: E-mail exato para busca.</param>
		[HttpGet]
		[ProducesResponseType(typeof(List<UsuarioExibicaoDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<List<UsuarioExibicaoDto>>> Listar(
		[FromQuery] string? nome,
		[FromQuery] string? email)
		{
			var lista = await _usuarioService.GetTodosAsync(nome, email);
			return Ok(lista);
		}

		/// <summary>
		/// Edita os dados do seu próprio perfil.
		/// </summary>
		/// <remarks>
		/// Requer estar logado. O sistema identifica automaticamente qual usuário está logado através do token.
		/// </remarks>
		/// <param name="dto">Novos dados (como nome ou e-mail) para atualizar.</param>
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

		/// <summary>
		/// Encerra e apaga a sua própria conta.
		/// </summary>
		/// <remarks>
		/// Requer estar logado. Atenção: Esta ação é definitiva e removerá seu acesso ao sistema.
		/// </remarks>
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