using GamingJourney.DTOs;
using GamingJourney.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamingJourney.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class GenerosController : MainController
	{
		private readonly GeneroService _generoService;

		public GenerosController(GeneroService generoService)
		{
			_generoService = generoService;
		}


		/// <summary>
		/// Cadastra um novo gênero de jogo no sistema.
		/// </summary>
		/// <remarks>
		/// Requer privilégio de Admin ou GM.
		/// Certifique-se de que o nome do gênero seja único para evitar erros de duplicidade.
		/// </remarks>
		/// <param name="dto">Objeto contendo o nome e informações do novo gênero.</param>
		/// <returns>O gênero recém-criado com seu respectivo ID.</returns>
		[Authorize(Roles = "Admin, GM")]
		[HttpPost("register")]
		[ProducesResponseType(typeof(GeneroResponseDto), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> Registrar(GeneroRegistroDto dto)
		{
			var resultado = await _generoService.RegistrarAsync(dto);
			return CreatedAtAction(nameof(Registrar), new { id = resultado.Id }, resultado);
		}


		/// <summary>
		/// Recupera a lista de gêneros cadastrados, com suporte a filtro por nome.
		/// </summary>
		/// <remarks> Se o parâmetro 'nome' for nulo, o sistema retorna todos os registros. </remarks>
		/// <param name="nome">Opcional: termo para filtrar gêneros que contenham parte deste nome.</param>
		[HttpGet]
		[ProducesResponseType(typeof(List<GeneroExibicaoDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<List<GeneroExibicaoDto>>> ExibirGeneros([FromQuery] string? nome)
		{
			var generos = await _generoService.ExibirTodosAsync(nome);
			return Ok(generos);
		}

		/// <summary>
		/// Busca um gênero específico pelo ID.
		/// </summary>
		/// <remarks>
		/// Se o ID não existir no banco, ele vai retornar um erro 404 (Não Encontrado).
		/// </remarks>
		/// <param name="id">ID numérico do gênero.</param>
		[HttpGet("{id:int}")]
		[ProducesResponseType(typeof(GeneroExibicaoDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<GeneroExibicaoDto>> ExibirGenerosId(int id)
		{
			var generosId = await _generoService.ExibirPorIdAsync(id);
			return Ok(generosId);
		}

		/// <summary>
		/// Remove um gênero do sistema.
		/// </summary>
		/// <remarks>
		/// Atenção: Essa ação é permanente e vai deletar o gênero do banco de dados.
		/// Requer permissão de Admin ou GM.
		/// </remarks>
		/// <param name="id">ID do gênero que será apagado.</param>
		[Authorize(Roles = "Admin, GM")]
		[HttpDelete("genre-delete/{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> DeletarGenero(int id)
		{
			await _generoService.DeletarGeneroAsync(id);
			return NoContent();
		}

		/// <summary>
		/// Atualiza as informações de um gênero existente.
		/// </summary>
		/// <remarks>
		/// Use este endpoint para alterar dados como o nome do gênero. 
		/// Requer permissão de Admin ou GM.
		/// </remarks>
		/// <param name="id">O ID do gênero que você quer editar.</param>
		/// <param name="dto">Os novos dados para a atualização.</param>
		/// <returns>O gênero já atualizado.</returns>
		[Authorize(Roles = "Admin, GM")]
		[HttpPut("{id:int}")]
		[ProducesResponseType(typeof(GeneroExibicaoDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<ActionResult<GeneroExibicaoDto>> AtualizarGenero(int id, GeneroAtualizarDto dto)
		{
			var generoEdit = await _generoService.AtualizarAsync(id, dto);			
			return Ok(generoEdit);
		}
	}
}
