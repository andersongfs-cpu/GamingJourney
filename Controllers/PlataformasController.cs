using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamingJourney.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlataformasController : MainController
	{
		private readonly PlataformaService _plataformaService;

		public PlataformasController(PlataformaService plataformaService)
		{
			_plataformaService = plataformaService;
		}

		/// <summary>
		/// Cadastra uma nova plataforma (PC, Console, etc).
		/// </summary>
		/// <remarks>
		/// Requer permissão de Admin ou GM. Informe o nome da plataforma para registro.
		/// </remarks>
		/// <param name="dto">Dados da plataforma que será criada.</param>
		[Authorize(Roles = "Admin,GM")]
		[HttpPost("register")]
		[ProducesResponseType(typeof(PlataformaResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> RegistrarPlataforma(PlataformaRegistroDto dto)
		{
			var plataforma = await _plataformaService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(ExibirPorId), new { id = plataforma.Id }, plataforma);
		}

		/// <summary>
		/// Lista todas as plataformas ou filtra por nome.
		/// </summary>
		/// <remarks>
		/// Se não passar nenhum nome no filtro, ele traz todas as plataformas do banco.
		/// </remarks>
		/// <param name="nome">Opcional: Nome ou parte do nome da plataforma.</param>
		[HttpGet]
		[ProducesResponseType(typeof(List<PlataformaExibicaoDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<List<PlataformaExibicaoDto>>> ExibirTodos([FromQuery] string? nome)
		{
			var plataformas = await _plataformaService.ExibirTodosAsync(nome);
			return Ok(plataformas);
		}

		/// <summary>
		/// Busca os detalhes de uma plataforma pelo ID.
		/// </summary>
		/// <remarks>
		/// Útil para verificar os dados específicos de uma única plataforma cadastrada.
		/// </remarks>
		/// <param name="id">ID numérico da plataforma.</param>
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(PlataformaResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<PlataformaResponseDto>> ExibirPorId(int id)
		{
			var plataforma = await _plataformaService.ExibirPorIdAsync(id);
			return Ok(plataforma);
		}

		/// <summary>
		/// Altera o nome ou dados de uma plataforma existente.
		/// </summary>
		/// <remarks>
		/// Requer permissão de Admin ou GM. Identifique a plataforma pelo ID e envie os novos dados.
		/// </remarks>
		/// <param name="id">ID da plataforma que você quer editar.</param>
		/// <param name="dto">Novas informações para atualizar.</param>
		[Authorize(Roles = "Admin,GM")]
		[HttpPut("{id:int}")]
		[ProducesResponseType(typeof(PlataformaResponseDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<PlataformaResponseDto>> AtualizarPlataforma(int id, PlataformaAtualizarDto dto)
		{
			var plataformaEdit = await _plataformaService.AtualizarAsync(id, dto);
			return Ok(plataformaEdit);
		}

		/// <summary>
		/// Remove uma plataforma do sistema.
		/// </summary>
		/// <remarks>
		/// Requer permissão de Admin ou GM. Cuidado: a exclusão é definitiva.
		/// </remarks>
		/// <param name="id">ID da plataforma que será apagada.</param>
		[Authorize(Roles = "Admin,GM")]
		[HttpDelete("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> DeletarPlataforma(int id)
		{
			await _plataformaService.DeletarPlataformaAsync(id);
			return NoContent();
		}
	}
}
