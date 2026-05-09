using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamingJourney.Controllers
{
	[ApiController]
	public abstract class MainController : ControllerBase
	{
		/// <summary>
		/// Extrai o ID do usuário de dentro do Token JWT.
		/// </summary>
		/// <returns>O ID numérico do usuário logado.</returns>
		/// <exception cref="UnauthorizedAccessException">Lançada se o token for inválido ou o ID não existir.</exception>
		protected int ObterUsuarioId()
		{
			var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (int.TryParse(claim, out int id)) return id;

			throw new UnauthorizedAccessException("Usuário não identificado no Token.");
		}
	}
}
