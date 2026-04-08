namespace GamingJourney.Models
{
	public class Jogo
	{
		public int Id{ get; set; }
		public string Titulo { get; set; } = string.Empty;		
		public string? CapaUrl { get; set; }
		public ICollection<Plataforma> Plataformas { get; set; } = new List<Plataforma>();
		public ICollection<Genero> Generos { get; set; } = new List<Genero>();
	}
}
