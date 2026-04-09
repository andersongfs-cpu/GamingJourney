namespace GamingJourney.DTOs
{
	public class JogoExibicaoDto
	{
		public string Titulo { get; set; } = string.Empty;
		public string? CapaUrl { get; set; }
		public List<GeneroExibicaoDto> Generos { get; set; } = new();
		public List<PlataformaExibicaoDto> Plataformas { get; set; } = new();
	}
}
