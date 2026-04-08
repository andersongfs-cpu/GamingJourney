namespace GamingJourney.DTOs
{
	public class JogoRegistroDto
	{
		public string Titulo { get; set; } = string.Empty;
		public string? CapaUrl { get; set; }
		public List<int> GenerosIds { get; set; } = new List<int>();
		public List<int> PlataformasIds { get; set; } = new List<int>();
	}
}
