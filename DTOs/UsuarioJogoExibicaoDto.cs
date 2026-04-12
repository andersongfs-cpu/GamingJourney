using GamingJourney.Models;

namespace GamingJourney.DTOs
{
	public class UsuarioJogoExibicaoDto
	{
		public string UsuarioNome { get; set; } = string.Empty;
		public string JogoNome { get; set; } = string.Empty;
		public decimal Nota { get; set; }
		public StatusJogo Status { get; set; } = 0;
		public string? CapaUrl { get; set; }
	}
}
