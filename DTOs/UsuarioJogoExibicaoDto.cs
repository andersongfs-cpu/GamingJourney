using GamingJourney.Models;

namespace GamingJourney.DTOs
{
	public class UsuarioJogoExibicaoDto
	{
		public string? JogoNome { get; set; }
		public decimal? Nota { get; set; }
		public string? Plataforma { get; set; }
		public string? Genero{ get; set; }
		public StatusJogo Status { get; set; } = 0;
		public string? CapaUrl { get; set; }
	}
}
