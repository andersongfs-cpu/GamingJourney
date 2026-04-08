namespace GamingJourney.Models
{
	public class Plataforma
	{
		public int Id { get; set; }
		public string Nome { get; set; } = string.Empty;
		public string? LogoUrl { get; set; }
		public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
	}
}
