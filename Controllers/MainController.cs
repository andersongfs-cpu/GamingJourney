using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamingJourney.Controllers
{
	[ApiController]
	public abstract class MainController : ControllerBase
	{
		protected int ObterUsuarioId()
		{
			var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (int.TryParse(claim, out int id)) return id;

			throw new UnauthorizedAccessException("Usuário não identificado no Token.");
		}
	}
}
