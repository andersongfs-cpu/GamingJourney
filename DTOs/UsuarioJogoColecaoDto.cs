using GamingJourney.Models;

namespace GamingJourney.DTOs
{
	public class UsuarioJogoColecaoDto
	{
		public int JogoId { get; set; }
		public decimal? Nota { get; set; }
		public StatusJogo Status { get; set; } = 0;
	}
}