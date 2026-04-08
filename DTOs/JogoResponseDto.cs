namespace GamingJourney.DTOs
{
	public class JogoResponseDto
	{
		public int Id { get; set; }
		public string Titulo { get; set; } = string.Empty;
		public string? CapaUrl { get; set; }
		public List<string> Generos { get; set; } = new List<string>();
		public List<string> Plataformas { get; set; } = new List<string>();
	}
}
