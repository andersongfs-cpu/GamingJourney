namespace GamingJourney.DTOs
{
	public class JogoAtualizarDto
	{
		public string? Titulo { get; set; }
		public string? CapaUrl { get; set; }
		public List<int>? GenerosIds { get; set; }
		public List<int>? PlataformasIds { get; set; }
	}
}
